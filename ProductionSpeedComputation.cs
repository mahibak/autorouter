using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    class ProductionSpeedComputation
    {
        static void UpdateDesiredProductionSpeed(List<Machine> machines)
        {
            List<Machine> buildingsInReversedProcessingOrder = new List<Machine>();

            buildingsInReversedProcessingOrder.AddRange(machines);

            for (int i = 0; i < buildingsInReversedProcessingOrder.Count; i++)
            {
                Machine machinesBeingProcessed = buildingsInReversedProcessingOrder[i];

                foreach (Machine childMachine in machinesBeingProcessed._outputSlots.Select(x => x.BuildingInput?.Building).Where(childBuilding => childBuilding != null))
                {
                    //Our desired output per second depends on all of these child's demand, make sure that their desired output per second is computed before us
                    if (buildingsInReversedProcessingOrder.IndexOf(childMachine) < i)
                    {
                        buildingsInReversedProcessingOrder.Remove(childMachine);
                        buildingsInReversedProcessingOrder.Add(childMachine);
                        i--;
                    }
                }
            }

            buildingsInReversedProcessingOrder.Reverse();

            foreach (Machine b in buildingsInReversedProcessingOrder)
            {
                //Each building's desired production speed can be computed here, and all dependencies are resolved in order.

                if(b._outputSlots.Count == 0)
                {
                    b._desiredItemsPerSecond = Double.PositiveInfinity;
                    continue;
                }
                List<Machine> childrenBuilding = b._outputSlots.Select(output => output.BuildingInput?.Building).Where(building => building != null).ToList();
                if (childrenBuilding.Count == 0)
                    b._desiredItemsPerSecond = 0;
                else
                {
                    b._desiredItemsPerSecond = childrenBuilding.Sum(childBuilding => childBuilding._desiredItemsPerSecond);
                    if (Double.IsInfinity(b._desiredItemsPerSecond))
                        b._desiredItemsPerSecond = b._maximumItemsPerSecond;
                }
            }
        }

        static void UpdatePossibleProductionSpeed(List<Machine> buildings)
        {
            List<Machine> buildingsInReversedProcessingOrder = new List<Machine>();

            buildingsInReversedProcessingOrder.AddRange(buildings);

            for (int i = 0; i < buildingsInReversedProcessingOrder.Count; i++)
            {
                Machine buildingBeingProcessed = buildingsInReversedProcessingOrder[i];

                foreach (Machine parentBuilding in buildingBeingProcessed._inputSlots.Select(x => x.BuildingOutput?.Building).Where(parentBuilding => parentBuilding != null))
                {
                    //Our desired output per second depends on all of the parent's possible rate, make sure that their output per second is computed before us
                    if (buildingsInReversedProcessingOrder.IndexOf(parentBuilding) < i)
                    {
                        buildingsInReversedProcessingOrder.Remove(parentBuilding);
                        buildingsInReversedProcessingOrder.Add(parentBuilding);
                        i--;
                    }
                }
            }

            buildingsInReversedProcessingOrder.Reverse();

            foreach (Machine b in buildingsInReversedProcessingOrder)
            {
                double maximumProductionSpeed = b._desiredItemsPerSecond;

                foreach(Machine parentBuilding in b._inputSlots.Select(input => input.BuildingOutput?.Building))
                {
                    if (parentBuilding != null && parentBuilding._desiredItemsPerSecond != 0)
                        maximumProductionSpeed = Math.Min(maximumProductionSpeed, b._desiredItemsPerSecond * Math.Min(1, parentBuilding._maximumItemsPerSecond / parentBuilding._desiredItemsPerSecond));
                    else
                        maximumProductionSpeed = 0; //An input is missing
                }

                b._itemsPerSecond = maximumProductionSpeed;
            }
        }

        public static void UpdateProductionSpeed(List<Machine> buildings)
        {
            UpdateDesiredProductionSpeed(buildings);
            UpdatePossibleProductionSpeed(buildings);
        }
    }
}
