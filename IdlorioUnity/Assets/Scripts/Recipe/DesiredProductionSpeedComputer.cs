using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class DesiredProductionSpeedComputer
{
    static void SetDesiredItemsPerSecondFromOutputs(Machine machine)
    {
        foreach (MachineConnector output in machine.GetConnectedOutputs())
        {
            output._desiredItemsPerSecond = output._otherConnector._desiredItemsPerSecond;

            machine._desiredItemsPerSecondToOutputs += output._desiredItemsPerSecond;
        }
    }

    static void SetDesiredItemsPerSecondToStorage(Machine machine)
    {
        if ((machine._storageMode & Machine.StorageModes.In) != 0 && machine.IsStorage && machine.StorageLeft > 0)
        {
            //Storage input mode
            if (machine._desiredItemsPerSecondToOutputs < machine._maximumItemsPerSecond)
            {
                //We can request more to put in storage, because priority is given to the outputs
                machine._desiredItemsPerSecondToStorage = machine._maximumItemsPerSecond - machine._desiredItemsPerSecondToOutputs;
            }
            else
            {
                //Outputs saturate the input, don't send any to storage not to starve the outputs
                machine._desiredItemsPerSecondToStorage = 0;
            }
        }
        else
        {
            //Can't store anything
            machine._desiredItemsPerSecondToStorage = 0;
        }
    }

    public static void UpdateDesiredItemsPerSecond(Machine machine)
    {
        SetDesiredItemsPerSecondFromOutputs(machine);
        SetDesiredItemsPerSecondToStorage(machine);
        
        if(machine._inputsUsedForRecipe != null)
        {
            foreach (MachineConnector input in machine._inputsUsedForRecipe)
            {
                input._desiredItemsPerSecond = machine.PossibleDesiredItemsPerSecond;
            }
        }
        else if(machine.IsStorage)
        {
            foreach (MachineConnector input in machine._inputSlots)
            {
                input._desiredItemsPerSecond = machine._desiredItemsPerSecondToStorage;
            }
        }
    }
}