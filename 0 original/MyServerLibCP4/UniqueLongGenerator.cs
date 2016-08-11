using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyServerLibCP4
{
    public class UniqueLongGenerator
    {
        private List<long> reuseList = new List<long>();

        private long multiple = 1;
        private long currentNumber = 0;

        public UniqueLongGenerator(long start, long multi)
        {
            multiple = multi;
            currentNumber = start;
        }

        public void reset(long start)
        {
            reuseList.Clear();
            currentNumber = start;
        }

        public long retrieve()
        {
            if (0 >= reuseList.Count)
            {
                currentNumber += multiple;
                return currentNumber;
            }

            long n = reuseList.ElementAt(0);
            reuseList.RemoveAt(0);
            return n;
        }

        public bool release(long n, bool bCheck = false)
        {
            reuseList.Add(n);
            return true;
        }
    }
}
