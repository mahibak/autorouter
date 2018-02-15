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
                b._desiredItemsPerSecond = b._itemsPerSecond;
            }
            else
            {
                b._desiredItemsPerSecond = 0;

                bool atLeastOneRequiredOutput = false;
                foreach (MachineConnector output in b._outputSlots)
                {
                    if (output._otherMachine == null)
                    {
                        if (output._requiredForMachineOperation)
                        {
                            b._desiredItemsPerSecond = 0;
                            break;
                        }
                    }
                    else
                    {
                        b._desiredItemsPerSecond += Mathf.Min(output._otherMachine._desiredItemsPerSecond, output._otherMachine._maximumItemsPerSecond);
                        atLeastOneRequiredOutput = true;
                    }
                }

                if (!atLeastOneRequiredOutput)
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
            float maximumProductionSpeed = Mathf.Min(b._desiredItemsPerSecond, b._maximumItemsPerSecond);

            foreach(MachineConnector input in b._inputSlots)
            {
                if(input._otherMachine == null)
                {
                    if (input._requiredForMachineOperation)
                    {
                        maximumProductionSpeed = 0;
                        break;
                    }
                }
                else
                {
                    maximumProductionSpeed = System.Math.Min(maximumProductionSpeed, Mathf.Min(b._desiredItemsPerSecond, b._maximumItemsPerSecond) * System.Math.Min(1, input._otherMachine._maximumItemsPerSecond / input._otherMachine._desiredItemsPerSecond));
                }
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
