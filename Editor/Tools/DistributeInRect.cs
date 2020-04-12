using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif

namespace Utilikit {
    [EditorTool( "Distribute In Rect" )]
    public class DistributeInRect : EditorTool {
        private Rect rect;
        private float minDist;
        private float maxDist;
        private GameObject currentSelection;

        public override bool IsAvailable() {
            return Selection.gameObjects != null && Selection.gameObjects.Length == 1;
        }

        public override void OnToolGUI( EditorWindow window ) {
            if ( Selection.activeGameObject == null )
                return;

            if ( currentSelection != Selection.activeGameObject ) {
                // reset
                currentSelection = Selection.activeGameObject;
                if ( rect.size.magnitude < .1f ) {
                    rect.size = new Vector2( 10, 10 );
                }
                rect.center = Selection.activeGameObject.transform.position;
            }

            Color rectColor = new Color( .52f, .76f, .86f );
            rect = RectHandleUtilities.DrawRectHandles( rect, currentSelection.transform.position.z, Handles.DotHandleCap, rectColor, Color.clear, 0.1F * HandleUtility.GetHandleSize( currentSelection.transform.position ), 0f );

            Handles.BeginGUI();
            using ( var area = new GUILayout.AreaScope( new Rect( 20, 20, 300f, 200f ) ) ) {
                using ( var vert = new GUILayout.VerticalScope() ) {
                    minDist = EditorGUILayout.FloatField( "Min Dist", minDist );
                    maxDist = EditorGUILayout.FloatField( "Max Dist", maxDist );

                    if ( GUILayout.Button( "Distribute!" ) ) {
                        var dist = new PoissonDiscSampler( rect.width, rect.height, minDist, maxDist );
                        int undoId = Undo.GetCurrentGroup();
                        foreach (var point in dist.Samples() )
                        {
                            Vector3 pos = rect.min + point;
                            pos.z = currentSelection.transform.position.z;

                            GameObject newObj;
                            if ( PrefabUtility.IsAnyPrefabInstanceRoot( currentSelection ) ) {
                                var prefab = PrefabUtility.GetCorrespondingObjectFromSource( currentSelection );
                                newObj = (GameObject)PrefabUtility.InstantiatePrefab( prefab, currentSelection.transform.parent );
                                newObj.transform.position = pos;
                            }
                            else {
                                newObj = Instantiate( currentSelection, pos, currentSelection.transform.rotation, currentSelection.transform.parent );
                            }
                            Undo.RegisterCreatedObjectUndo( newObj, "Ditribute Object" );
                            Undo.CollapseUndoOperations( undoId );
                        }
                    }
                }
            }
            Handles.EndGUI();
        }
    }
}
