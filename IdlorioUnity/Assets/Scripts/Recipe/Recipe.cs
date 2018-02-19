using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Recipe
{
    public List<Ingredient> _inputs = new List<Ingredient>();
    
    public List<Item> _outputs = new List<Item>();

    private static List<Recipe> m_recipies = null;
    public static List<Recipe> Recipies
    {
        get
        {
            if (m_recipies == null)
                InitRecipes();
            return m_recipies;
        }
    }
    
    public static void InitRecipes()
    {
        m_recipies = new List<Recipe>();

        //Mixed Ore to Copper and Tin Ore
        Recipe r = new Recipe();
        Ingredient i = new Ingredient();
        i._baseItem = BaseItems.MixedOre;
        i._properties.Add(Properties.Centrifuged, new Range(1));
        r._inputs.Add(i);

        Item o = new Item();
        o._baseItem = BaseItems.TinOre;
        r._outputs.Add(o);

        o = new Item();
        o._baseItem = BaseItems.CopperOre;
        r._outputs.Add(o);
        m_recipies.Add(r);

        //Copper and Tin to Bronze
        r = new Recipe();
        i = new Ingredient();
        i._baseItem = BaseItems.CopperOre;
        i._properties.Add(Properties.Heated, new Range(2));
        r._inputs.Add(i);

        i = new Ingredient();
        i._baseItem = BaseItems.TinOre;
        i._properties.Add(Properties.Heated, new Range(2));
        r._inputs.Add(i);

        o = new Item();
        o._baseItem = BaseItems.Bronze;
        r._outputs.Add(o);
        m_recipies.Add(r);

        //Copper to wire
        r = new Recipe();
        i = new Ingredient();
        i._baseItem = BaseItems.CopperOre;
        i._properties.Add(Properties.Heated, new Range(1));
        i._properties.Add(Properties.Stretched, new Range(1));
        i._properties.Add(Properties.Cut, new Range(1));
        r._inputs.Add(i);

        o = new Item();
        o._baseItem = BaseItems.CopperWire;
        r._outputs.Add(o);
        m_recipies.Add(r);

        //Copper ore to double heated copper ore
        r = new Recipe();
        i = new Ingredient();
        i._baseItem = BaseItems.CopperOre;
        i._properties.Add(Properties.Heated, new Range(0, Int32.MaxValue));
        r._inputs.Add(i);

        o = new Item();
        o._baseItem = BaseItems.CopperOre;
        o._properties.Add(Properties.Heated, 1);
        r._outputs.Add(o);
        m_recipies.Add(r);

        o = new Item();
        o._baseItem = BaseItems.CopperOre;
        o._properties.Add(Properties.Heated, 1);
        r._outputs.Add(o);
        m_recipies.Add(r);
    }

    public override string ToString()
    {
        return String.Join(", ", _outputs) + " from " + String.Join(", ", _inputs);
    }
}
