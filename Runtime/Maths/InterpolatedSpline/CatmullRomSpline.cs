/* InterpolatedCurve.cs
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
    public class CatmullRomSpline : InterpolatedSpline {

        public float Alpha { get; private set; } = 0.5f;
        public float Tension { get; private set; } = 0;

        Segment[] segments;

        struct Segment {
            public Vector2 a, b, c, d;

            public Segment( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha, float tension ) {
                // https://qroph.github.io/2018/07/30/smooth-paths-using-catmull-rom-splines.html
                float t01 = Mathf.Pow( ( p1 - p0 ).magnitude, alpha );
                float t12 = Mathf.Pow( ( p1 - p2 ).magnitude, alpha );
                float t23 = Mathf.Pow( ( p2 - p3 ).magnitude, alpha );

                Vector2 m1 = ( 1.0f - tension ) *
    ( p2 - p1 + t12 * ( ( p1 - p0 ) / t01 - ( p2 - p0 ) / ( t01 + t12 ) ) );
                Vector2 m2 = ( 1.0f - tension ) *
    ( p2 - p1 + t12 * ( ( p3 - p2 ) / t23 - ( p3 - p1 ) / ( t12 + t23 ) ) );

                a = 2.0f * ( p1 - p2 ) + m1 + m2;
                b = -3.0f * ( p1 - p2 ) - m1 - m1 - m2;
                c = m1;
                d = p1;
            }
        }


        public CatmullRomSpline( float alpha = 0.5f ) {
            this.Alpha = alpha;
        }

        /// <summary>
        /// Create control points based on the corners plus interpolated start and end control points
        /// </summary>
        /// <param name="corners"></param>
        /// <returns></returns>
        public static CatmullRomSpline CreateCurveThroughCorners( ICollection<Vector2> corners ) {
            if ( corners == null || corners.Count < 2 ) {
                throw new ArgumentException( "Need at least two corners to create a curve" );
            }

            int numControlPoints = corners.Count + 2;
            Vector2[] controlPoints = new Vector2[numControlPoints];
            corners.CopyTo( controlPoints, 1 );
            // start control point is same as the second corner in the opposite direction
            controlPoints[0] = controlPoints[1] + controlPoints[1] - controlPoints[2];
            // end is the same as the second to last in the opposite direction
            controlPoints[numControlPoints - 1] = controlPoints[numControlPoints - 2] + controlPoints[numControlPoints - 2] - controlPoints[numControlPoints - 3];

            CatmullRomSpline spline = new CatmullRomSpline();
            spline.SetControlPoints( controlPoints );
            return spline;
        }

        public override void SetControlPoints(ICollection<Vector2> controlPoints) {
            base.SetControlPoints( controlPoints );

            // precalculated the ts for each segment
            int numSegments = controlPoints.Count - 3;
            segments = new Segment[numSegments];
            for ( int i = 0; i < numSegments; i++ ) {
                segments[i] = new Segment( this.controlPoints[i], this.controlPoints[i + 1], this.controlPoints[i + 2], this.controlPoints[i + 3], Alpha, Tension );
            }
        }

        /// <summary>
        /// Get the point at normalized distance along curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override Vector2 Interpolate( float t ) {
            int numSegments = controlPoints.Length - 3;
            float segmentAmount = 1f / numSegments;
            int segmentIdx = Mathf.Min( (int)( t * numSegments ), numSegments - 1 );

            float segmentStart = (float)segmentIdx / numSegments;
            float u = ( t - segmentStart ) / segmentAmount;

            Segment segment = segments[segmentIdx];
            return segment.a * u * u * u +
             segment.b * u * u +
             segment.c * u +
             segment.d;
        }
    }
}
