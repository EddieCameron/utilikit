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
        #endregion
    }
}
