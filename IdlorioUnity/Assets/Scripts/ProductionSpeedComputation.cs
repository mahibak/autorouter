using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionSpeedComputation
{
    public static void UpdateDesiredItemsPerSecond(List<Machine> machines)
    {
        List<Machine> orderedMachineList = GetOrderedMachineListBottomFirst(machines);
        
        foreach (Machine b in orderedMachineList)
        {
            b._desiredItemsPerSecond = 0;

            foreach (MachineConnector output in b._outputSlots)
            {
                if (output._otherConnector != null)
                {
                    //Connected output
                    output._desiredItemsPerSecond = output._otherConnector._desiredItemsPerSecond;

                    b._desiredItemsPerSecond += output._desiredItemsPerSecond;
                }
            }
            
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
            
            foreach (MachineConnector input in b._inputSlots)
            {
                if (b._productionEnabled)
                    input._desiredItemsPerSecond = Mathf.Min(b._desiredItemsPerSecond, b._maximumItemsPerSecondProduction);
                else
                    input._desiredItemsPerSecond = 0;
            }
        }
    }

    private static List<Machine> GetOrderedMachineListBottomFirst(List<Machine> machines)
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
        List<Machine> machinesInReversedProcessingOrder = GetOrderedMachineListTopDown(machines);

        foreach (Machine b in machinesInReversedProcessingOrder)
        {
            b._inputSatisfactionRatio = 1;

            if (b._inputsUsedForRecipe != null)
            {
                foreach (MachineConnector input in b._inputsUsedForRecipe)
                {
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

    private static List<Machine> GetOrderedMachineListTopDown(List<Machine> machines)
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

    private static void UpdateRecipes(List<Machine> machines)
    {
        machines = GetOrderedMachineListTopDown(machines);

        foreach(Machine m in machines)
        {
            List<System.Tuple<Recipe, MachineConnector[]>> possibleRecipes = new List<System.Tuple<Recipe, MachineConnector[]>>();
                
            //Try to find a new recipe
            foreach (Recipe r in Recipe.Recipies)
            {
                if (m._itemsInStorage > 0)
                {
                    //Todo...
                }
                MachineConnector[] inputSetup = m.GetInputSetupForRecipe(r);
                if (inputSetup != null)
                    possibleRecipes.Add(new System.Tuple<Recipe, MachineConnector[]>(r, inputSetup));
            }
                
            if (possibleRecipes.Count == 0)
            {
                MachineConnector firstConnectedInput = m.GetConnectedInputsThatGiveItems().FirstOrDefault();

                if (firstConnectedInput == null)
                {
                    m._recipe = null;
                }
                else
                {
                    Recipe r = new Recipe();
                    r._outputs.Add(m.ApplyTransformation(firstConnectedInput._item));
                    m._recipe = r;
                }
            }
            else
            {
                m._recipe = possibleRecipes[0].Item1;
                m._inputsUsedForRecipe = possibleRecipes[0].Item2;
            }
            
            if(m._recipe == null)
            {
                if(m._itemsInStorage > 0)
                {
                    foreach(MachineConnector c in m._outputSlots)
                    {
                        c._item = m._itemInStorage;
                        if(c._otherConnector != null)
                            c._otherConnector._item = m._itemInStorage;
                    }
                }
                else
                {
                    m._recipe = null;
                    m._itemInStorage = null;

                    foreach (MachineConnector c in m._outputSlots)
                    {
                        c._item = null;
                        if (c._otherConnector != null)
                            c._otherConnector._item = null;
                    }
                }

                m._productionEnabled = false;
            }
            else
            {
                foreach (MachineConnector c in m._outputSlots)
                {
                    c._item = null;
                    if (c._otherConnector != null)
                        c._otherConnector._item = null;
                }

                for (int i = 0; i < m._outputSlots.Length; i++)
                {
                    m._outputSlots[i]._item = m._recipe._outputs[i % m._recipe._outputs.Count];
                    if (m._outputSlots[i]._otherConnector != null)
                        m._outputSlots[i]._otherConnector._item = m._outputSlots[i]._item;
                }

                m._productionEnabled = true;
            }
        }
    }

    public static void UpdateMachineLinks(List<Machine> machines)
    {
        //UpdateRecipes(machines);
        UpdateDesiredItemsPerSecond(machines);
        UpdatePossibleProductionSpeed(machines);
    }
}
