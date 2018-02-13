using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    class Random
    {
        static System.Random r = new System.Random();
        public static double NextDouble()
        {
            return r.NextDouble();
        }

        public static int Next(int lowerInclusive, int upperExclusive)
        {
            return r.Next(lowerInclusive, upperExclusive);
        }

        public static int Next()
        {
            return r.Next();
        }
    }
}
