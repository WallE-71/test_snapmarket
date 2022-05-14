﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SnapMarket.Common.Extensions
{
    public static class LogLinqExtensions
    {
        public static IEnumerable<T> LogLINQ<T>(this IEnumerable<T> enumerable, string logName, Func<T, string> printMethod)
        {
#if DEBUG
            int count = 0;
            foreach (var item in enumerable)
            {
                if (printMethod != null)
                    Debug.WriteLine($"{logName}|item {count} = {printMethod(item)}");
                count++;
                yield return item;
            }
            Debug.WriteLine($"{logName}|count = {count}");
#else
    return enumerable;
#endif
        }
    }
}
