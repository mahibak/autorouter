using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;

class RecipeAssignator
{
    static MachineConnector[] GetInputSetupForRecipe(Machine m, Recipe r)
    {
        List<MachineConnector> connectedInputs = m.GetConnectedInputs().ToList();
        List<MachineConnector> connectedOutputs = m.GetConnectedOutputs().ToList();

        if (connectedInputs.Count != r._inputs.Count)
            return null; //The machine doesn't have the right inputs connected

        foreach (MachineConnector i in connectedInputs)
        {
            if (i._item == null)
                return null; //Required input doesn't have any item in it
        }

        if (connectedOutputs.Count != r._outputs.Count)
            return null; //The machine doesn't have the right outputs connected

        for (int i = 0; i < r._outputs.Count; i++)
            if (!m._outputSlots[i].IsConnected)
                return null; //Not all outputs are connected

        foreach (MachineConnector[] inputPermutation in connectedInputs.GetPermutations().Select(x => x.ToArray()))
        {
            bool goodRecipe = true;

            for (int i = 0; i < inputPermutation.Length; i++)
            {
                //Each transformed input must exactly match the recipe
                if (!m.ApplyTransformation(inputPermutation[i]._item).Satisfies(r._inputs[i]))
                {
                    goodRecipe = false;
                    break;
                }
            }

            if (goodRecipe)
                return inputPermutation;
        }

        return null;
    }
    
    static Recipe GetSupportedRecipe(Machine m, out MachineConnector[] inputsUsedForRecipe)
    {
        //Storage dosen't have recipes
        if (m.IsStorage)
        {
            inputsUsedForRecipe = null;
            return null;
        }
        
        //Try to find an existing recipe
        foreach (Recipe r in Recipe.Recipies)
        {
            inputsUsedForRecipe = GetInputSetupForRecipe(m, r);

            if (inputsUsedForRecipe != null)
                return r;
        }

        //Fall back to a simple recipe
        if(m._addedProperty != Properties.None)
        {
            List<MachineConnector> connectedInputs = m.GetConnectedInputs().ToList();

            if(connectedInputs.Count == 1 && connectedInputs[0]._item != null)
            {
                Recipe r = new Recipe();

                r._outputs.Add(m.ApplyTransformation(connectedInputs[0]._item));

                Ingredient i = new Ingredient();
                i._baseItem = r._outputs[0]._baseItem;
                foreach(Properties p in r._outputs[0]._properties.Keys)
                    i._properties.Add(p, new Range(r._outputs[0]._properties[p]));
                r._inputs.Add(i);
                
                inputsUsedForRecipe = GetInputSetupForRecipe(m, r);

                if (inputsUsedForRecipe != null)
                    return r;
            }
        }
        
        inputsUsedForRecipe = null;
        return null;
    }

    public static void AssignRecipe(Machine m)
    {
        m._recipe = GetSupportedRecipe(m, out m._inputsUsedForRecipe);
        
        foreach (MachineConnector c in m._outputSlots)
        {
            c._item = null;
            if (c._otherConnector != null)
                c._otherConnector._item = null;
        }
        
        if (m._recipe == null)
        {
            if(m.IsStorage)
            {
                if ((m._storageMode & Machine.StorageModes.Out) != 0)
                {
                    foreach (MachineConnector c in m._outputSlots.Where(x => x.IsConnected))
                    {
                        c._item = m._itemInStorage;
                        c._otherConnector._item = c._item;
                    }
                }
                else
                {
                    foreach (MachineConnector c in m._outputSlots.Where(x => x.IsConnected))
                    {
                        c._item = null;
                        c._otherConnector._item = c._item;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < m._recipe._outputs.Count; i++)
            {
                m._outputSlots[i]._item = m._recipe._outputs[i];
                m._outputSlots[i]._otherConnector._item = m._outputSlots[i]._item;
            }
        }
    }
}
