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

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        static int orientation( Vector2 p, Vector2 q, Vector2 r ) {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            float val = ( q.y - p.y ) * ( r.x - q.x ) -
                    ( q.x - p.x ) * ( r.y - q.y );

            if ( val == 0 ) return 0; // colinear

            return ( val > 0 ) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool TestLineIntersection( Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2 ) {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation( p1, q1, p2 );
            int o2 = orientation( p1, q1, q2 );
            int o3 = orientation( p2, q2, p1 );
            int o4 = orientation( p2, q2, q1 );

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
        #endregion
    }
}
