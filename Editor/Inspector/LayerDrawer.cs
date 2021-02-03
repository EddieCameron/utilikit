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

[CustomPropertyDrawer( typeof( LayerAttribute ) )]
public class LayerDrawer : PropertyDrawer {

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        if ( property.propertyType != SerializedPropertyType.Integer ) {
            // Integer is expected. Everything else is ignored.
            return;
        }

        using ( var scope = new EditorGUI.PropertyScope( position, label, property ) ) {
            property.intValue = EditorGUI.LayerField( position, scope.content, property.intValue, EditorStyles.popup );
        }
    }
}
