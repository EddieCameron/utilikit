/* BezierCurve.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    public class BezierCurve : InterpolatedSpline {

        bool isDirty = true;

        float _approxLength;
        public float ApproxLength {
            get {
                if ( isDirty ) {
                    // TODO better/cheaper estimate
                    _approxLength = 0;
                    Vector2 lastPoint = controlPoints[0];
                    for ( int i = 1; i <= 20; i++ ) {
                        float t = (float)i / 20;
                        Vector2 nextPoint = Interpolate( t );
                        _approxLength += ( nextPoint - lastPoint ).magnitude;
                        lastPoint = nextPoint;
                    }

                    isDirty = false;
                }
                return _approxLength;
            }
        }

        public BezierCurve( params Vector2[] controlPoints ) {
            if ( controlPoints.Length < 3 )
                throw new ArgumentException( "Need at least three control points to make a curve" );
            base.SetControlPoints( controlPoints );
        }

        public override void SetControlPoints(ICollection<Vector2> controlPoints) {
            base.SetControlPoints( controlPoints );
            isDirty = true;
        }

        /// <summary>
        /// Get the point at normalized distance along curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override Vector2 Interpolate( float t ) {
            t = Mathf.Clamp01( t );

            return controlPoints[1] + ( 1 - t ) * ( 1 - t ) * ( controlPoints[0] - controlPoints[1] ) + t * t * ( controlPoints[2] - controlPoints[1] );
        }
    }
}
