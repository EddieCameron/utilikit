using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilikit {
    public class OscillatePosition : MonoBehaviour {
        [Tooltip( "How long to complete a full oscillation (up to max, down to centre, return to start" )]
        public float period;

        [Tooltip( "At what point in the cycle do we start at (0=centre moving up, .25 = max, .5 = centre moving down, .75=min" )]
        public float offset;

        public float min, max;

        float t;


        public enum Axis {
            X,
            Y,
            Z
        }
        public Axis axis;

        public float Value { get; private set; }

        public bool addLocalPositionOffsetFromStart = false;
        Vector3 startPos;


        void Start() {
            t = offset;
            startPos = transform.localPosition;
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

            if ( addLocalPositionOffsetFromStart )
                Value += startPos[(int)axis];

            Vector3 pos = transform.localPosition;
            pos[(int)axis] = Value;
            transform.localPosition = pos;
        }
    }
}
