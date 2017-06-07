using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace MyServerLibCP4
{
    public class UniqueNumberGenerator
    {
        ConcurrentBag<Int64> UIDSet = new ConcurrentBag<Int64>();
        Int64 StartNumber = 1;
        Int64 MaxCount = 1;

        public UniqueNumberGenerator(Int64 startNumber, Int64 maxCount)
        {
            StartNumber = startNumber;
            MaxCount = maxCount;

            Generate();
        }

        public void Reset()
        {
            Generate();
        }

        public Int64 Retrieve()
        {
            if(UIDSet.TryTake(out Int64 result) == false)
            {
                return 0;
            }

            return result;
        }

        public bool Release(Int64 UID)
        {
            UIDSet.Add(UID);
            return true;
        }


        void Generate()
        {
            int count = UIDSet.Count;

            for (Int64 i = 0; i < count; ++i)
            {
                UIDSet.TryTake(out Int64 result);
            }

            for (Int64 i = StartNumber; i < MaxCount; ++i)
            {
                UIDSet.Add(i);
            }
        }
        
    }

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
