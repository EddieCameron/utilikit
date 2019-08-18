using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit
{
    public static class MathUtils
    {
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
#endregion
    }
}
