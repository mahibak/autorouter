using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineComputations
{
    static void UpdateDesiredItemsPerSecond(List<Machine> machines)
    {
        List<Machine> orderedMachineList = GetOrderedMachineListBottomFirst(machines);
        
        foreach (Machine m in orderedMachineList)
        {
            DesiredProductionSpeedComputer.UpdateDesiredItemsPerSecond(m);
        }
    }

    static List<Machine> GetOrderedMachineListBottomFirst(List<Machine> machines)
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

    static void UpdatePossibleProductionSpeed(List<Machine> machines)
    {
        List<Machine> machinesInReversedProcessingOrder = GetOrderedMachineListTopDown(machines);

        foreach (Machine machine in machinesInReversedProcessingOrder)
        {
            ProductionSpeedComputer.UpdateItemsPerSecond(machine);
        }
    }

    static List<Machine> GetOrderedMachineListTopDown(List<Machine> machines)
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

    public static void UpdateRecipes(List<Machine> machines)
    {
        machines = GetOrderedMachineListTopDown(machines);

        foreach(Machine m in machines)
            RecipeAssignator.AssignRecipe(m);
    }
    
    public static void UpdateMachineLinks(List<Machine> machines)
    {
        UpdateDesiredItemsPerSecond(machines);
        UpdatePossibleProductionSpeed(machines);
    }
}
