/* DebugExtensions.cs
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
    public static class DebugExtensions {


        /// <summary>
        /// Draw a rect in the scene
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="zPos"></param>
        public static void DrawRect( Rect rect, float zPos = 0 ) {
            DrawRect( rect, zPos, Color.white, 0 );
        }

        /// <summary>
        /// Draw a rect in the scene
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="zPos"></param>
        public static void DrawRect( Rect rect, float zPos, Color c, float t = 0 ) {
            Vector3 min = rect.min;
            min.z = zPos;
            Vector3 max = rect.max;
            max.z = zPos;

            Vector3 upperLeft = new Vector3( min.x, max.y, zPos );
            Vector3 lowerRight = new Vector3( max.x, min.y, zPos );
            Debug.DrawLine( min, upperLeft, c, t );
            Debug.DrawLine( upperLeft, max, c, t );
            Debug.DrawLine( max, lowerRight, c, t );
            Debug.DrawLine( lowerRight, min, c, t );
        }
    }
}
