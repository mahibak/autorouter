using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Item
{
    public BaseItems _baseItem;

    public Dictionary<Properties, int> _properties = new Dictionary<Properties, int>();

    public bool Satisfies(Ingredient inputRequirements)
    {
        if (_baseItem != inputRequirements._baseItem)
            return false;

        if (inputRequirements._properties.Count != _properties.Count)
            return false;

        foreach (Properties p in _properties.Keys)
        {
            if (!inputRequirements._properties.ContainsKey(p))
                return false;

            if (inputRequirements._properties[p]._max < _properties[p])
                return false;

            if (inputRequirements._properties[p]._min > _properties[p])
                return false;
        }

        return true;
    }
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(_baseItem.ToString());

        foreach(Properties p in _properties.Keys)
        {
            int r = _properties[p];
            
            if (r == 1)
                sb.Append(" " + p.ToString());
            else
                sb.Append(" " + r.ToString() + "*" + p.ToString());
        }

        return sb.ToString();
    }

    public Item GetCopy()
    {
        Item ret = new Item();
        ret._baseItem = _baseItem;

        foreach (Properties p in _properties.Keys)
        {
            ret._properties.Add(p, _properties[p]);
        }

        return ret;
    }
}