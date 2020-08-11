/* FloatRange.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Utilikit {
    [Serializable]
    public struct FloatRange {
        public float min, max;

        public float GetInterpolatedPoint( float t ) {
            return Mathf.Lerp( min, max, t );
        }

        public float GetRandomPoint() {
            return Mathf.Lerp( min, max, Random.value );
        }

        public float Clamp( float value ) {
            return Mathf.Clamp( value, min, max );
        }
    }
}
