using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Ingredient
{
    public BaseItems _baseItem;

    public Dictionary<Properties, Range> _properties = new Dictionary<Properties, Range>();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(_baseItem.ToString());

        bool first = true;
        foreach (Properties p in _properties.Keys)
        {
            Range r = _properties[p];
            
            sb.Append(" " + r.ToString() + " " + p.ToString());
            if (!first)
                sb.Append(", ");
            first = false;
        }

        return sb.ToString();
    }
}