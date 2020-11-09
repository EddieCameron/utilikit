/* GroupSelected.cs
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
using UnityEditor;

namespace Utilikit {
    /// <summary>
    /// Groups all selected scene transforms under a single parent.
    /// Select GameObject/Group Selected Objects or use Ctrl/Cmd+Shift+G
    /// </summary>
    public static class GroupSelected {
        [MenuItem( "GameObject/Group Selected Objects %#G", isValidateFunction: true )]
        public static bool CreateGroupValidate() {
            return Selection.GetTransforms( SelectionMode.Editable | SelectionMode.ExcludePrefab | SelectionMode.OnlyUserModifiable | SelectionMode.TopLevel ).Length > 0;
        }

        [MenuItem( "GameObject/Group Selected Objects %#G")]
        public static void CreateGroup() {
            var transforms = Selection.GetTransforms( SelectionMode.Editable | SelectionMode.ExcludePrefab | SelectionMode.OnlyUserModifiable | SelectionMode.TopLevel );
            if ( transforms.Length == 0 )
                return;
            Vector3 avgPos = Vector3.zero;
            foreach ( var t in transforms ) {
                avgPos += t.position;
            }
            avgPos /= transforms.Length;

            Transform commonParent = FindLowestCommonAncestor( transforms );
            Transform newParent = new GameObject( "Group" ).transform;
            newParent.SetParent( commonParent );
            newParent.position = avgPos;
            Undo.RegisterCreatedObjectUndo( newParent.gameObject, "Create new parent" );
            foreach ( var t in transforms ) {
                Undo.SetTransformParent( t, newParent, "Set Parent" );
            }

            Selection.activeGameObject = newParent.gameObject;
        }

        public static Transform FindLowestCommonAncestor( IList<Transform> transforms ) {
            if ( transforms.Count == 0 )
                return null;
            if ( transforms.Count == 1 )
                return transforms[0];

            // get full heirarcy of first transform in list
            var fullHeirarchy = new List<Transform>();
            Transform parent = transforms[0].parent;
            while ( parent != null ) {
                fullHeirarchy.Add( parent );
                parent = parent.parent;
            }

            int highestCommonIdx = 0;
            for ( int i = 1; i < transforms.Count; i++ ) {
                parent = transforms[i].parent;
                bool foundCommonParent = false;
                while ( parent != null ) {
                    int commonIdx = fullHeirarchy.IndexOf( parent );
                    if ( commonIdx >= 0 ) {
                        // found a common parent
                        highestCommonIdx = Mathf.Max( commonIdx, highestCommonIdx );
                        foundCommonParent = true;
                        break;
                    }
                    parent = parent.parent;
                }

                if ( !foundCommonParent ) {
                    // no common parent
                    return null;
                }
            }

            return fullHeirarchy[highestCommonIdx];
        }
    }
}
