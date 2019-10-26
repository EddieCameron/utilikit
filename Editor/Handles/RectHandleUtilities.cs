using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Utilikit {
    public static class RectHandleUtilities {
        public static Rect DrawRectHandles( Rect inRect, float worldZ, Handles.CapFunction capFunc, Color capCol, Color fillCol, float capSize, float snap ) {
            
            Vector3[] corners = { new Vector2( inRect.xMin, inRect.yMax ),    // Top Left
                                   inRect.max,     // Top Right
                                   new Vector2( inRect.xMax, inRect.yMin ),    // Bottom Right
                                   inRect.min }; // Bottom Left
            for ( int i = 0; i < 4; i++ ) {
                corners[i].z = worldZ;
            }

            Handles.DrawSolidRectangleWithOutline( corners, new Color( fillCol.r, fillCol.g, fillCol.b, 0.25f ), capCol );


            // Calculate the 4 control points. Should be the center of each edge.            
            Vector3[] handlePoints =  { new Vector2( inRect.xMin, inRect.center.y ),   // Left
                                         new Vector2( inRect.xMax, inRect.center.y ),    // Right
                                         new Vector2( inRect.center.x, inRect.yMax ),    // Top
                                         new Vector2( inRect.center.x, inRect.yMin ) }; // Bottom 

            Handles.color = capCol;
            handlePoints[0] = Handles.Slider( handlePoints[0], -Vector3.right, capSize, capFunc, snap ); // Left Handle
            handlePoints[1] = Handles.Slider( handlePoints[1], Vector3.right, capSize, capFunc, snap );  // Right Handle
            handlePoints[2] = Handles.Slider( handlePoints[2], Vector3.up, capSize, capFunc, snap );     // Top Handle
            handlePoints[3] = Handles.Slider( handlePoints[3], -Vector3.up, capSize, capFunc, snap );    // Bottom Handle

            Vector2 min = new Vector2( handlePoints[0].x, handlePoints[3].y );
            Vector2 size = new Vector2( handlePoints[1].x - handlePoints[0].x,
                handlePoints[2].y - handlePoints[3].y );

            return new Rect( min, size );
        }
    }
}
