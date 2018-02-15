using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionSpeedComputation
{
    static void UpdateDesiredProductionSpeed(List<Machine> machines)
    {
        List<Machine> machinesInReversedProcessingOrder = new List<Machine>();

        machinesInReversedProcessingOrder.AddRange(machines);

        for (int i = 0; i < machinesInReversedProcessingOrder.Count; i++)
        {
            Machine machinesBeingProcessed = machinesInReversedProcessingOrder[i];

            foreach (Machine childMachine in machinesBeingProcessed._outputSlots.Select(output => output._otherMachine).Where(childMachine => childMachine != null))
            {
                //Our desired output per second depends on all of these child's demand, make sure that their desired output per second is computed before us
                if (machinesInReversedProcessingOrder.IndexOf(childMachine) < i)
                {
                    machinesInReversedProcessingOrder.Remove(childMachine);
                    machinesInReversedProcessingOrder.Add(childMachine);
                    i--;
                }
            }
        }

        machinesInReversedProcessingOrder.Reverse();

        foreach (Machine b in machinesInReversedProcessingOrder)
        {
            //Each machine's desired production speed can be computed here, and all dependencies are resolved in order.

            if (b._outputSlots.Length == 0)
            {
                b._desiredItemsPerSecond = System.Single.PositiveInfinity;
                continue;
            }
            List<Machine> childrenMachine = b._outputSlots.Select(output => output._otherMachine).Where(machine => machine != null).ToList();
            if (childrenMachine.Count == 0)
                b._desiredItemsPerSecond = 0;
            else
            {
                b._desiredItemsPerSecond = childrenMachine.Sum(childMachine => childMachine._desiredItemsPerSecond);
                if (System.Single.IsInfinity(b._desiredItemsPerSecond))
                    b._desiredItemsPerSecond = b._maximumItemsPerSecond;
            }
        }
    }

    static void UpdatePossibleProductionSpeed(List<Machine> machines)
    {
        List<Machine> machinesInReversedProcessingOrder = new List<Machine>();

        machinesInReversedProcessingOrder.AddRange(machines);

        for (int i = 0; i < machinesInReversedProcessingOrder.Count; i++)
        {
            Machine machineBeingProcessed = machinesInReversedProcessingOrder[i];

            foreach (Machine parentMachine in machineBeingProcessed._inputSlots.Select(x => x._otherMachine).Where(parentMachine => parentMachine != null))
            {
                //Our desired output per second depends on all of the parent's possible rate, make sure that their output per second is computed before us
                if (machinesInReversedProcessingOrder.IndexOf(parentMachine) < i)
                {
                    machinesInReversedProcessingOrder.Remove(parentMachine);
                    machinesInReversedProcessingOrder.Add(parentMachine);
                    i--;
                }
            }
        }

        machinesInReversedProcessingOrder.Reverse();

        foreach (Machine b in machinesInReversedProcessingOrder)
        {
            float maximumProductionSpeed = b._desiredItemsPerSecond;

            foreach (Machine parentMachine in b._inputSlots.Select(input => input._otherMachine))
            {
                if (parentMachine != null && parentMachine._desiredItemsPerSecond != 0)
                    maximumProductionSpeed = System.Math.Min(maximumProductionSpeed, b._desiredItemsPerSecond * System.Math.Min(1, parentMachine._maximumItemsPerSecond / parentMachine._desiredItemsPerSecond));
                else
                    maximumProductionSpeed = 0; //An input is missing
            }

            b._itemsPerSecond = maximumProductionSpeed;
        }
    }

    public static void UpdateProductionSpeed(List<Machine> machines)
    {
        UpdateDesiredProductionSpeed(machines);
        UpdatePossibleProductionSpeed(machines);
    }
}
