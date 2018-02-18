using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionSpeedComputation
{
    public static void UpdateDesiredItemsPerSecond(List<Machine> machines)
    {
        List<Machine> orderedMachineList = GetOrderedMachineListForComputingDesiredProductionSpeed(machines);
        
        foreach (Machine b in orderedMachineList)
        {
            b._desiredItemsPerSecond = 0;
            bool allRequiredOutputsConnected = true;

            foreach (MachineConnector output in b._outputSlots)
            {
                if (output._otherConnector == null)
                {
                    b._desiredItemsPerSecond = 0;

                    //Unconnected output
                    if (output._requiredForMachineOperation)
                    {
                        allRequiredOutputsConnected = false;
                        break;
                    }
                }
                else
                {
                    //Connected output
                    output._desiredItemsPerSecond = output._otherConnector._desiredItemsPerSecond;

                    b._desiredItemsPerSecond += output._desiredItemsPerSecond;
                }
            }

            if (allRequiredOutputsConnected)
            {
                if ((b._storageMode & Machine.StorageModes.In) != 0)
                {
                    //Storage input mode
                    if (b._desiredItemsPerSecond < b._maximumItemsPerSecondProduction)
                    {
                        //We may be able to produce more than what's needed, try to store some
                        if (b._desiredItemsPerSecond + (b._storageCapacity - b._itemsInStorage) > b._maximumItemsPerSecondProduction)
                        {
                            //Production limited
                            b._desiredItemsPerSecondToStorage = b._maximumItemsPerSecondProduction - b._desiredItemsPerSecond;
                        }
                        else
                        {
                            //Storage limited
                            b._desiredItemsPerSecondToStorage = b._storageCapacity - b._itemsInStorage;
                        }

                        b._desiredItemsPerSecond += b._desiredItemsPerSecondToStorage;
                    }
                }
                else
                {
                    //Can't store anything
                    b._desiredItemsPerSecondToStorage = 0;
                }
            }
            else
            {
                //Some required outputs are disconnected
                b._desiredItemsPerSecondToStorage = 0;
            }

            foreach (MachineConnector input in b._inputSlots)
                input._desiredItemsPerSecond = Mathf.Min(b._desiredItemsPerSecond, b._maximumItemsPerSecondProduction);
        }
    }

    private static List<Machine> GetOrderedMachineListForComputingDesiredProductionSpeed(List<Machine> machines)
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

        return machinesInReversedProcessingOrder;
    }

    public static void UpdatePossibleProductionSpeed(List<Machine> machines)
    {
        List<Machine> machinesInReversedProcessingOrder = GetOrderedMachineListForComputingProductionSpeed(machines);

        foreach (Machine b in machinesInReversedProcessingOrder)
        {
            b._inputSatisfactionRatio = 1;

            foreach (MachineConnector input in b._inputSlots)
            {
                if (input._otherMachine == null)
                {
                    //Input is disconnected
                    input._itemsPerSecond = 0;

                    if (input._requiredForMachineOperation)
                    {
                        //Required input is disconnected
                        b._inputSatisfactionRatio = 0;
                        input._itemsPerSecond = 0;
                        break;
                    }
                }
                else
                {
                    //Input is connected
                    input._itemsPerSecond = input._otherConnector._itemsPerSecond;
                    input._satisfaction = input._otherConnector._satisfaction;

                    b._inputSatisfactionRatio = Mathf.Min(b._inputSatisfactionRatio, input._satisfaction);
                }
            }

            b._itemsPerSecondFromProduction = b._inputSatisfactionRatio * Mathf.Min(b._desiredItemsPerSecond, b._maximumItemsPerSecondProduction);

            float desiredItemsPerSecondToOutputs = b._desiredItemsPerSecond - b._desiredItemsPerSecondToStorage;

            if (b._itemsPerSecondFromProduction < desiredItemsPerSecondToOutputs)
            {
                //Production output doesn't match demand, try to output from storage too
                float desireOutputFromStorage = b._desiredItemsPerSecond - b._itemsPerSecondFromProduction;

                if ((b._storageMode & Machine.StorageModes.Out) == 0)
                    desireOutputFromStorage = 0;

                b._itemsPerSecondToStorage = -Mathf.Min(desireOutputFromStorage, b._itemsInStorage, b._maximumItemsPerSecondFromStorage, b._maximumItemsPerSecondOutput - b._itemsPerSecondFromProduction);
            }
            else
            {
                //Production output matches demand, send whatever we can to storage
                b._itemsPerSecondToStorage = Mathf.Max(0, b._itemsPerSecondFromProduction - desiredItemsPerSecondToOutputs);
            }

            b._itemsPerSecondToOutputs = b._itemsPerSecondFromProduction - b._itemsPerSecondToStorage;

            if (desiredItemsPerSecondToOutputs == 0)
                b._outputSatisfaction = 0;
            else
                b._outputSatisfaction = b._itemsPerSecondToOutputs / desiredItemsPerSecondToOutputs;

            foreach (MachineConnector output in b._outputSlots)
            {
                output._satisfaction = b._outputSatisfaction;
                output._itemsPerSecond = output._desiredItemsPerSecond * output._satisfaction;
            }
        }
    }

    private static List<Machine> GetOrderedMachineListForComputingProductionSpeed(List<Machine> machines)
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

        return machinesInReversedProcessingOrder;
    }

    public static void UpdateProductionSpeed(List<Machine> machines)
    {
        UpdateDesiredItemsPerSecond(machines);
        UpdatePossibleProductionSpeed(machines);
    }
}
