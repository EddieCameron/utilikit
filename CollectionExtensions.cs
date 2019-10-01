using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Utilikit
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Transform a collection into an array of the same length, running a function on each element
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mapFunction"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
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


        /// <summary>
        /// Only add the given item if it doesn't already exist.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if element was added, false if not</returns>
        public static bool TryAddUnique<T>( this ICollection<T> collection, T element ) {
            if ( collection.Contains( element ) )
                return false;

            collection.Add( element );
            return true;
        }
    }
}
