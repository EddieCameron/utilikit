/* SortingLayerDrawer.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

[CustomPropertyDrawer( typeof( SortingLayerAttribute ) )]
public class SortingLayerDrawer : PropertyDrawer {

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        if ( property.propertyType != SerializedPropertyType.Integer ) {
            // Integer is expected. Everything else is ignored.
            return;
        }

        using ( var scope = new EditorGUI.PropertyScope( position, label, property ) ) {
            GUIContent[] sortingLayerNames = GetSortingLayerNames();
            int[] sortingLayerIDs = GetSortingLayerIDs();

            int sortingLayerIndex = Mathf.Max( 0, System.Array.IndexOf<int>( sortingLayerIDs, property.intValue ) );
            sortingLayerIndex = EditorGUI.Popup( position, scope.content, sortingLayerIndex, sortingLayerNames );
            property.intValue = sortingLayerIDs[sortingLayerIndex];
        }
    }

    /**
     * Retrives list of sorting layer names.
     *
     * @return List of sorting layer names.
     */
    private GUIContent[] GetSortingLayerNames() {
        return SortingLayer.layers.Map( l => new GUIContent( l.name ) );
    }

    /**
     * Retrives list of sorting layer identifiers.
     *
     * @return List of sorting layer identifiers.
     */
    private int[] GetSortingLayerIDs() {
        return SortingLayer.layers.Map( l => l.id );
    }
}
