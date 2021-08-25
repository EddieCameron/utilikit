using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilikit {
    public class OscillateRotation : MonoBehaviour {
        [Tooltip( "How long to complete a full oscillation (up to max, down to centre, return to start" )]
        public float period = 1;

        public float min, max;

        [Tooltip( "At what point in the cycle do we start at (0=centre moving up, .25 = max, .5 = centre moving down, .75=min" )]
        public float startOffset = 0;


        float t;


        public enum Axis {
            X = 0,
            Y = 1,
            Z = 2
        }
        public Axis axis;

        public float Value { get; private set; }


        void Start() {
            t = Mathf.Clamp01( startOffset );
            UpdateValue();
        }

        void Update() {
            if ( period > 0 )
                t += Time.deltaTime / period;
            UpdateValue();
        }

        void UpdateValue() {
            float angle = t * Mathf.PI * 2;

            float amp = Mathf.DeltaAngle( min, max ) * .5f;
            float centre = min + amp;

            Value = centre + Mathf.Sin( angle ) * amp;

            Vector3 eulers = transform.localEulerAngles;
            eulers[(int)axis] = Value;
            transform.localEulerAngles = eulers;
        }
    }
}
