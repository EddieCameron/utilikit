/* Oscillator.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    public class Oscillator : MonoBehaviour {
        [Tooltip( "How long to complete a full oscillation (up to max, down to centre, return to start" )]
        public float period;

        [Tooltip( "At what point in the cycle do we start at (0=centre moving up, .25 = max, .5 = centre moving down, .75=min")]
        public float offset;

        public float min, max;

        public SetFloatEvent setProperty;
        public class SetFloatEvent : UnityEvent<float> {}

        float t;

        public float Value { get; private set; }


        void Start() {
            t = offset;
            UpdateValue();
        }

        void Update() {
            if ( period > 0 )
                t += Time.deltaTime / period;
            UpdateValue();
        }

        void UpdateValue() {
            float angle = t * Mathf.PI * 2;

            float amp = ( max - min ) * .5f;
            float centre = min + amp;

            Value = centre + Mathf.Sin( angle ) * amp;
            setProperty.Invoke( Value );
        }
    }
}
