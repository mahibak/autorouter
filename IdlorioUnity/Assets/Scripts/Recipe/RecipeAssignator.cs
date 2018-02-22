using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;

class RecipeAssignator
{
    static Conveyor[] GetInputSetupForRecipe(Machine m, Recipe r)
    {
        List<Conveyor> connectedInputs = m.GetConnectedInputs().ToList();
        List<Conveyor> connectedOutputs = m.GetConnectedOutputs().ToList();

        if (connectedInputs.Count != r._inputs.Count)
            return null; //The machine doesn't have the right inputs connected

        foreach (Conveyor i in connectedInputs)
        {
            if (i._item == null)
                return null; //Required input doesn't have any item in it
        }

        if (connectedOutputs.Count != r._outputs.Count)
            return null; //The machine doesn't have the right outputs connected

        for (int i = 0; i < r._outputs.Count; i++)
            if (!m._outputs[i].IsConnected)
                return null; //Not all outputs are connected

        foreach (Conveyor[] inputPermutation in connectedInputs.GetPermutations().Select(x => x.ToArray()))
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
    
    static Recipe GetSupportedRecipe(Machine m, out Conveyor[] inputsUsedForRecipe)
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
            List<Conveyor> connectedInputs = m.GetConnectedInputs().ToList();

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
        m._recipe = GetSupportedRecipe(m, out m._inputConveyorsUsedForRecipe);

        List<Conveyor> connectedOutputs = m.GetConnectedOutputs().ToList();
        
        if (m._recipe == null)
        {
            if(m.IsStorage)
            {
                if ((m._storageMode & Machine.StorageModes.Out) != 0)
                {
                    foreach (Conveyor c in connectedOutputs)
                    {
                        c._item = m._itemInStorage;
                    }
                }
                else
                {
                    foreach (Conveyor c in connectedOutputs)
                    {
                        c._item = null;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < m._recipe._outputs.Count; i++)
            {
                connectedOutputs[i]._item = m._recipe._outputs[i];
            }
        }
    }
}
