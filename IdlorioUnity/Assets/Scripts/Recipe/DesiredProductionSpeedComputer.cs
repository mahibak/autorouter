using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class DesiredProductionSpeedComputer
{
    static float GetDesiredItemsPerSecondFromOutputs(Machine machine)
    {
        List<Conveyor> connectedOutputConveyors = machine.GetConnectedOutputs().ToList();
        if (connectedOutputConveyors.Count == 0)
            return 0;
        else
            return connectedOutputConveyors.Min(x => x._desiredItemsPerSecond);
    }
    
    static float GetDesiredItemsPerSecondToStorage(Machine machine)
    {
        if ((machine._storageMode & Machine.StorageModes.In) != 0 && machine.IsStorage && machine.StorageLeft > 0)
        {
            //Storage input mode
            if (machine._desiredItemsPerSecondToOutputs < machine._maximumItemsPerSecond)
            {
                //We can request more to put in storage, because priority is given to the outputs
                return machine._maximumItemsPerSecond - machine._desiredItemsPerSecondToOutputs;
            }
            else
            {
                //Outputs saturate the input, don't send any to storage not to starve the outputs
                return 0;
            }
        }
        else
        {
            //Can't store anything
            return 0;
        }
    }

    public static void UpdateDesiredItemsPerSecond(Machine machine)
    {
        machine._desiredItemsPerSecondToOutputs = GetDesiredItemsPerSecondFromOutputs(machine);

        machine._desiredItemsPerSecondToStorage = GetDesiredItemsPerSecondToStorage(machine);
        
        if(machine._inputConveyorsUsedForRecipe != null)
        {
            foreach (Conveyor input in machine._inputConveyorsUsedForRecipe)
            {
                input._desiredItemsPerSecond = machine.PossibleDesiredItemsPerSecond;
            }
        }
        else if(machine.IsStorage)
        {
            foreach (Conveyor input in machine.GetConnectedInputs())
            {
                input._desiredItemsPerSecond = machine._desiredItemsPerSecondToStorage;
            }
        }
    }
}