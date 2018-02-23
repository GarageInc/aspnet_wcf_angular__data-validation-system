using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.Extensions
{
    public static class StringExtensions
    {
        public static List<List<T>> Split<T>(this List<T> items, int sliceSize = 50)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += sliceSize)
                list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
            return list;
        }

        public static T? TryParse<T>(this string input) where T : struct
        {
            T i = default(T);
            object[] args = new object[] { input, i };
            var tryParse = typeof(T).GetMethod("TryParse",
                                      new[] { typeof(string), typeof(T).MakeByRefType() });
            if (tryParse != null)
            {
                var r = (bool)tryParse.Invoke(null, args);
                return r ? (T)args[1] : (T?)null;
            }
            return (T?)null;
        }
    }
}
