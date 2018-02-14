using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    class ProductionSpeedComputation
    {
        static void UpdateDesiredProductionSpeed(List<Building> buildings)
        {
            List<Building> buildingsInReversedProcessingOrder = new List<Building>();

            buildingsInReversedProcessingOrder.AddRange(buildings.Where(building => building.Inputs.All(input => input.BuildingOutput == null)));

            for (int i = 0; i < buildingsInReversedProcessingOrder.Count; i++)
            {
                Building buildingBeingProcessed = buildingsInReversedProcessingOrder[i];

                foreach (Building childBuilding in buildingBeingProcessed.Outputs.Select(x => x.BuildingInput?.Building).Where(childBuilding => childBuilding != null))
                {
                    //Our desired output per second depends on all of these child's demand, make sure that their desired output per second is computed before us
                    if (!buildingsInReversedProcessingOrder.Contains(childBuilding))
                    {
                        buildingsInReversedProcessingOrder.Add(childBuilding);
                    }
                    else if (buildingsInReversedProcessingOrder.IndexOf(childBuilding) < i)
                    {
                        buildingsInReversedProcessingOrder.Remove(childBuilding);
                        buildingsInReversedProcessingOrder.Add(childBuilding);
                        i--;
                    }
                }
            }

            buildingsInReversedProcessingOrder.Reverse();

            foreach (Building b in buildingsInReversedProcessingOrder)
            {
                //Each building's desired production speed can be computed here, and all dependencies are resolved in order.

                if(b.Outputs.Count == 0)
                {
                    b.DesiredItemsPerSecond = Double.PositiveInfinity;
                    continue;
                }
                List<Building> childrenBuilding = b.Outputs.Select(output => output.BuildingInput?.Building).Where(building => building != null).ToList();
                if (childrenBuilding.Count == 0)
                    b.DesiredItemsPerSecond = 0;
                else
                {
                    b.DesiredItemsPerSecond = childrenBuilding.Sum(childBuilding => childBuilding.DesiredItemsPerSecond);
                    if (Double.IsInfinity(b.DesiredItemsPerSecond))
                        b.DesiredItemsPerSecond = b.MaximumItemsPerSecond;
                }
            }
        }

        static void UpdatePossibleProductionSpeed(List<Building> buildings)
        {
            List<Building> buildingsInReversedProcessingOrder = new List<Building>();

            buildingsInReversedProcessingOrder.AddRange(buildings.Where(building => building.Outputs.All(output => output.BuildingInput == null)));

            for (int i = 0; i < buildingsInReversedProcessingOrder.Count; i++)
            {
                Building buildingBeingProcessed = buildingsInReversedProcessingOrder[i];

                foreach (Building parentBuilding in buildingBeingProcessed.Inputs.Select(x => x.BuildingOutput?.Building).Where(parentBuilding => parentBuilding != null))
                {
                    //Our desired output per second depends on all of the parent's possible rate, make sure that their output per second is computed before us
                    if (!buildingsInReversedProcessingOrder.Contains(parentBuilding))
                    {
                        buildingsInReversedProcessingOrder.Add(parentBuilding);
                    }
                    else if (buildingsInReversedProcessingOrder.IndexOf(parentBuilding) < i)
                    {
                        buildingsInReversedProcessingOrder.Remove(parentBuilding);
                        buildingsInReversedProcessingOrder.Add(parentBuilding);
                        i--;
                    }
                }
            }

            buildingsInReversedProcessingOrder.Reverse();

            foreach (Building b in buildingsInReversedProcessingOrder)
            {
                double maximumProductionSpeed = b.DesiredItemsPerSecond;

                foreach(Building parentBuilding in b.Inputs.Select(input => input.BuildingOutput?.Building))
                {
                    if (parentBuilding != null && parentBuilding.DesiredItemsPerSecond != 0)
                        maximumProductionSpeed = Math.Min(maximumProductionSpeed, b.DesiredItemsPerSecond * Math.Min(1, parentBuilding.MaximumItemsPerSecond / parentBuilding.DesiredItemsPerSecond));
                    else
                        maximumProductionSpeed = 0; //An input is missing
                }

                b.ItemsPerSecond = maximumProductionSpeed;
            }
        }

        public static void UpdateProductionSpeed(List<Building> buildings)
        {
            UpdateDesiredProductionSpeed(buildings);
            UpdatePossibleProductionSpeed(buildings);
        }
    }
}
