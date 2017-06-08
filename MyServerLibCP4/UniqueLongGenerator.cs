using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace MyServerLibCP4
{
    /// <summary>
    /// 유니크 ID(번호) 할당기
    /// </summary>
    public class UniqueNumberAllocator
    {
        ConcurrentBag<Int64> UIDSet = new ConcurrentBag<Int64>();
        Int64 StartNumber = 1;
        Int64 MaxCount = 1;

        
        public void Reset(Int64 startNumber, Int64 maxCount)
        {
            StartNumber = startNumber;
            MaxCount = maxCount;

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

}
