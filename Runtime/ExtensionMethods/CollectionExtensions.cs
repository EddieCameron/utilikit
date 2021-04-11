using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Utilikit
{
    public static class CollectionExtensions {
        /// <summary>
        /// Transform a collection into an array of the same length, running a function on each element
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mapFunction"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        public static TOut[] Map<TIn, TOut>( this IList<TIn> list, Func<TIn, TOut> mapFunction ) {
            int listCount = list.Count;
            var outArray = new TOut[listCount];
            for ( int i = 0; i < listCount; i++ ) {
                outArray[i] = mapFunction( list[i] );
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

        /// <summary>
        /// Only add the given item if it doesn't already exist.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if all elements were added, false if at least one was not</returns>
        public static bool AddRangeUnique<T>( this ICollection<T> collection, IEnumerable<T> rangeToAdd ) {
            bool allAdded = true;
            foreach ( var element in rangeToAdd ) {
                if ( collection.Contains( element ) )
                    allAdded = false;
                else
                    collection.Add( element );
            }
            return allAdded;
        }

        /// <summary>
        /// Get a random element from a non-empty list/array
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomElement<T>( this IReadOnlyList<T> collection ) {
            if ( collection.Count == 0 )
                throw new ArgumentException( "Can't get random element from empty collection" );

            return collection[UnityEngine.Random.Range( 0, collection.Count )];
        }

        /// <summary>
        /// Get a random element from a non-empty list/array
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomIListElement<T>( this IList<T> collection ) {
            if ( collection.Count == 0 )
                throw new ArgumentException( "Can't get random element from empty collection" );

            return collection[UnityEngine.Random.Range( 0, collection.Count )];
        }

        /// <summary>
        /// Find the element in the collection that returns the highest value when evaluated with the evaluateFunction
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="evaluateFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindBestMatch<T>( this IEnumerable<T> collection, Func<T, float> evaluateFunction ) {
            T currentHighest = default( T );
            float currentHighestValue = float.MinValue;
            foreach ( var item in collection ) {
                float itemEvaluatedValue = evaluateFunction( item );
                if ( itemEvaluatedValue > currentHighestValue ) {
                    currentHighest = item;
                    currentHighestValue = itemEvaluatedValue;
                }
            }
            return currentHighest;
        }

        /// <summary>
        /// Find the element in the collection that returns the highest value when evaluated with the evaluateFunction
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="evaluateFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int FindBestMatchIndex<T>( this IList<T> collection, Func<T, float> evaluateFunction ) {
            int currentHighestIdx = -1;
            float currentHighestValue = float.MinValue;
            for ( int i = 0; i < collection.Count; i++ ) {
                T item = collection[i];
                float itemEvaluatedValue = evaluateFunction( item );
                if ( itemEvaluatedValue > currentHighestValue ) {
                    currentHighestIdx = i;
                    currentHighestValue = itemEvaluatedValue;
                }
            }
            return currentHighestIdx;
        }

        /// <summary>
        /// Find the element in the collection that returns the highest value when evaluated with the evaluateFunction
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="evaluateFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsAny<T>( this IEnumerable<T> collection, Predicate<T> matchPredicate ) {
            foreach ( var item in collection ) {
                if ( matchPredicate( item ) )
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Inplace fisher-yates shuffle of a list
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>( this IList<T> list ) {
            var rng = new System.Random();
            int n = list.Count;
            while ( n > 1 ) {
                n--;
                int k = rng.Next( n + 1 );
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
