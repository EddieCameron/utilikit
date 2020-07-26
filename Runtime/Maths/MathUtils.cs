using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public static class MathUtils {
        #region  Rect
        /// <summary>
        /// Create a new Rect that expands this rect to include the given point
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Rect Encapsulate( this Rect rect, Vector2 point ) {
            if ( point.x < rect.xMin )
                rect.xMin = point.x;
            else if ( point.x > rect.xMax )
                rect.xMax = point.x;

            if ( point.y < rect.yMin )
                rect.yMin = point.y;
            else if ( point.y > rect.yMax )
                rect.yMax = point.y;

            return rect;
        }

        /// <summary>
        /// Create a new Rect that expands this rect to include the given rect
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect Encapsulate( this Rect rect, Rect otherRect ) {
            if ( otherRect.xMin < rect.xMin )
                rect.xMin = otherRect.xMin;
            else if ( otherRect.xMax > rect.xMax )
                rect.xMax = otherRect.xMax;

            if ( otherRect.yMin < rect.yMin )
                rect.yMin = otherRect.yMin;
            else if ( otherRect.yMax > rect.yMax )
                rect.yMax = otherRect.yMax;

            return rect;
        }

        public static Vector2 RandomPointWithin( this Rect rect ) {
            return new Vector2( Random.Range( rect.xMin, rect.xMax ), Random.Range( rect.yMin, rect.yMax ) );
        }

        /// <summary>
        /// Whether the given rect is entirely contained within the first rect
        /// </summary>
        /// <param name="rectA"></param>
        /// <param name="rectB"></param>
        /// <returns></returns>
        public static bool Contains( this Rect rectA, Rect rectB ) {
            return rectA.xMin <= rectB.xMin &&
                rectA.xMax >= rectB.xMax &&
                rectA.yMin <= rectB.yMin &&
                rectA.yMax >= rectB.yMax;
        }
        #endregion

        #region Vectors
        /// <summary>
        /// Get the angle required to rotate anti-clockwise from (1, 0) to this vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ToDegrees( this Vector2 v ) {
            return Mathf.Atan2( v.y, v.x ) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Create a unit vector that is rotated the given degrees anticlockwise from (1,0)
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <returns></returns>
        public static Vector2 GetAngleVector( float angleDegrees ) {
            float rad = angleDegrees * Mathf.Deg2Rad;
            return new Vector2( Mathf.Cos( rad ), Mathf.Sin( rad ) );
        }
        #endregion

        #region  Geometry
        /// <summary>
        /// Return the length of a vector as projected along a given direction
        /// </summary>
        /// <param name="a"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float ProjectedLength( this Vector2 a, Vector2 dir ) {
            return Vector2.Dot( a, dir.normalized );
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        static bool onSegment( Vector2 p, Vector2 q, Vector2 r ) {
            if ( q.x <= Mathf.Max( p.x, r.x ) && q.x >= Mathf.Min( p.x, r.x ) &&
                q.y <= Mathf.Max( p.y, r.y ) && q.y >= Mathf.Min( p.y, r.y ) )
                return true;

            return false;
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool TestLineIntersection( Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2 ) {
            // Find the four orientations needed for general and
            // special cases
            int o1 = GetPointToLineOrientation( p1, q1, p2 );
            int o2 = GetPointToLineOrientation( p1, q1, q2 );
            int o3 = GetPointToLineOrientation( p2, q2, p1 );
            int o4 = GetPointToLineOrientation( p2, q2, q1 );

            // General case
            if ( o1 != o2 && o3 != o4 )
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if ( o1 == 0 && onSegment( p1, p2, q1 ) ) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1
            if ( o2 == 0 && onSegment( p1, q2, q1 ) ) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if ( o3 == 0 && onSegment( p2, p1, q2 ) ) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if ( o4 == 0 && onSegment( p2, q1, q2 ) ) return true;

            return false; // Doesn't fall in any of the above cases
        }


        /// <summary>
        /// What is the shortest distance from a point on the given line to pTest
        /// </summary>
        /// <returns></returns>
        public static float DistanceFromPointToLine( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            // https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
            // Return minimum distance between line segment vw and point p
            float l2 = ( p1 - p0 ).sqrMagnitude;  // i.e. |w-v|^2 -  avoid a sqrt
            if ( l2 == 0.0 ) return ( pTest - p0 ).magnitude;   // v == w case

            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line.
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            float t = Mathf.Max( 0, Mathf.Min( 1, Vector2.Dot( pTest - p0, p1 - p0 ) / l2 ) );
            Vector2 projection = p0 + t * ( p1 - p0 );  // Projection falls on the segment
            return ( pTest - projection ).magnitude;
        }


        /// <summary>
        /// Get the closest point on line p0-p1 to pTest
        /// </summary>
        /// <returns></returns>
        public static Vector2 NearestPointOnLine( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            Vector2 startToPoint = pTest - p0;
            Vector2 startToEnd = ( p1 - p0 ).normalized;
            float dot = Vector2.Dot( startToEnd, startToPoint );

            if ( dot <= 0 )
                return p0;

            if ( dot >= Vector2.Distance( p0, p1 ) )
                return p1;

            Vector2 offsetToPoint = startToEnd * dot;
            return p0 + offsetToPoint;
        }

        /// <summary>
        /// Returns 0 if pTest is on line
        /// +1 if pTest is to the right of line ( three points are clockwise)
        /// -1 if pTest is to the left (three points are anti-clockwise)
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="pTest"></param>
        /// <returns></returns>
        public static int GetPointToLineOrientation( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            float orientation = ( ( p1.y - p0.y ) * ( pTest.x - p1.x )
                    - ( p1.x - p0.x ) * ( pTest.y - p1.y ) );
            if ( orientation == 0 )
                return 0;

            return orientation > 0 ? 1 : -1;
        }

        /// <summary>
        /// Is a point to the left, right, or on, an infinite line passing between two points
        /// > 0 : left of line
        /// < 0 : right of line
        /// == 0 : on line
        /// </summary>
        /// <returns></returns>
        public static float IsLeft( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            return ( ( p1.x - p0.x ) * ( pTest.y - p0.y )
                    - ( pTest.x - p0.x ) * ( p1.y - p0.y ) );
        }

        #endregion

        #region Maths
        /// <summary>
        /// Ping pongs t between 0 and length(inclusive)
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int PingPongInt( int t, int length ) {
            int r = t % ( 2 * length );
            if ( r < length )
                return r;
            else
                return length * 2 - r;
        }

        /// <summary>
        /// Smoothly interpolate towards the given target.
        /// A more predictable Mathf.Lerp(current, target, x * Time.deltaTime )
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static float SmoothLerpTowards( this float current, float target, float rate ) {
            return SmoothLerpTowards( current, target, rate, Time.deltaTime );
        }

        public static float SmoothLerpTowards( this float current, float target, float rate, float deltaTime ) {
            rate = Mathf.Pow( 2, rate );    // use log scale
            return Mathf.Lerp( target, current, Mathf.Pow( 2, -rate * deltaTime ) );
        }
        #endregion
    }
}
