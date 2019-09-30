using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Utilikit
{
    public static class CollectionExtensions
    {
        public static TOut[] Map<TIn, TOut>( this IList<TIn> list, Func<TIn, TOut> mapFunction )
        {
            int listCount = list.Count;
            var outArray = new TOut[listCount];
            for (int i = 0; i < listCount; i++)
            {
                outArray[i] = mapFunction(list[i]);
            }
            return outArray;
        }
    }
}
