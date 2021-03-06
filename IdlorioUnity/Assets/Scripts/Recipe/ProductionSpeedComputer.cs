﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProductionSpeedComputer
{
    public static void UpdateItemsPerSecond(Machine machine)
    {
        machine._inputSatisfaction = GetSatisfactionRatio(machine);

        machine._itemsPerSecondFromProduction = machine._inputSatisfaction * machine.PossibleDesiredItemsPerSecond;

        machine._itemsPerSecondToStorage = GetItemsPerSecondToStorage(machine, machine._itemsPerSecondFromProduction);

        machine._itemsPerSecondToOutputs = machine._itemsPerSecondFromProduction - machine._itemsPerSecondToStorage;

        if (machine._desiredItemsPerSecondToOutputs == 0)
            machine._outputSatisfaction = 0;
        else
            machine._outputSatisfaction = machine._itemsPerSecondToOutputs / machine._desiredItemsPerSecondToOutputs;

        foreach (Conveyor output in machine.GetConnectedOutputs())
        {
            output._itemsPerSecond = machine._itemsPerSecondToOutputs;
        }
    }

    private static float GetSatisfactionRatio(Machine machine)
    {
        if (machine.IsStorage)
        {
            List<Conveyor> connectedInputs = machine.GetConnectedInputs().ToList();

            if (connectedInputs.Count == 0)
                return 0;

            return System.Math.Min(1.0f, connectedInputs.Min(x => x.Satisfaction));
        }
        else
        {
            if (machine._inputConveyorsUsedForRecipe == null)
                return 0;

            return System.Math.Min(1.0f, machine._inputConveyorsUsedForRecipe.Min(x => x.Satisfaction));
        }
    }
    
    private static float GetItemsPerSecondToStorage(Machine machine, float itemsPerSecondFromProduction)
    {
        if (itemsPerSecondFromProduction < machine._desiredItemsPerSecondToOutputs)
        {
            //Production output doesn't match demand, try to output from storage too
            float desiredOutputFromStorage = machine._desiredItemsPerSecondToOutputs - itemsPerSecondFromProduction;

            if ((machine._storageMode & Machine.StorageModes.Out) == 0 || !machine.IsStorage || machine.StorageLeft == 0)
                desiredOutputFromStorage = 0; //Cannot output from storage

            return -Mathf.Min(desiredOutputFromStorage, machine._maximumItemsPerSecond, machine._maximumItemsPerSecond - itemsPerSecondFromProduction);
        }
        else
        {
            //Production output matches demand, send whatever we can to storage
            if (machine.IsStorage && machine.StorageLeft > 0)
            {
                return Mathf.Max(0, itemsPerSecondFromProduction - machine._desiredItemsPerSecondToOutputs);
            }
            else
            {
                return 0;
            }
        }
    }
}