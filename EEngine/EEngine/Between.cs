﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEngine.EEngine
{
    public static class Between
    {
        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                && Comparer<T>.Default.Compare(item, end) <= 0;
        }
    }
}
