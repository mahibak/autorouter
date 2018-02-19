using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Range
{
    public int _min;
    public int _max;
    
    public Range(int specificValue)
    {
        this._min = specificValue;
        this._max = specificValue;
    }

    public Range(int min, int max)
    {
        this._min = min;
        this._max = max;
    }

    public override string ToString()
    {
        if (_min == _max)
            return _min.ToString();
        else if (_max == Int32.MaxValue)
            return "at least " + _min;
        else if (_min == Int32.MinValue)
            return "at most " + _max;
        else
            return _min + " to " + _max;
    }
}