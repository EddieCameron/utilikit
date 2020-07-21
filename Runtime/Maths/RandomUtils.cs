using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit
{
    public static class RandomUtils {

        public static int PickWeightedRandom( int count, Func<int, float> getPickChance ) {
            // get total weight
            float total = 0;
            for ( int i = 0; i < count; i++ ) {
                total += getPickChance( i );
            }

            float random = UnityEngine.Random.value * total;
            float runningTotal = 0;
            for ( int i = 0; i < count - 1; i++ ) {
                float bucketWeight = getPickChance( i );
                runningTotal += bucketWeight;
                if ( runningTotal > random )
                    return i;
            }

            return count - 1;
        }

        public static Vector2 OnUnitCircle() {
            var angle = UnityEngine.Random.value * Mathf.PI * 2f;
            return new Vector2( Mathf.Cos( angle ), Mathf.Sin( angle ) );
        }
    }
}
