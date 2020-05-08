/* ReorderableListDrawer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

[CustomPropertyDrawer( typeof( ReorderableList<> ), useForChildren: true )]
public class ReorderableListDrawer : PropertyDrawer {

    Dictionary<string, ReorderableList> _listsPerProp = new Dictionary<string, ReorderableList>();

    ReorderableList GetReorderableList( SerializedProperty prop ) {
        SerializedProperty listProperty = prop.FindPropertyRelative( "_list" );

        ReorderableList list;
        if ( _listsPerProp.TryGetValue( listProperty.propertyPath, out list ) ) {
            return list;
        }

        list = new ReorderableList( listProperty.serializedObject, listProperty, draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true );
        _listsPerProp[listProperty.propertyPath] = list;

        float height = 0f;
        for(var i = 0; i < listProperty.arraySize; i++)
        {
            height = Mathf.Max( height, EditorGUI.GetPropertyHeight( listProperty.GetArrayElementAtIndex( i ), true ) );
        }
        list.elementHeight = height;

        list.drawHeaderCallback += rect => {
            EditorGUI.LabelField( rect, prop.displayName );
        };

        list.drawElementCallback += ( rect, index, isActive, isFocused ) => {
            SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex( index );

            SerializedProperty end = elementProp.GetEndProperty();
            elementProp.Next( enterChildren: true );
            do {
                if ( SerializedProperty.EqualContents( elementProp, end ) )
                    break;

                rect.height = EditorGUI.GetPropertyHeight( elementProp );
                EditorGUI.PropertyField( rect, elementProp, includeChildren: true );
                rect.y += rect.height;
            } while ( elementProp.Next( enterChildren: false ) );
        };

        list.elementHeightCallback += idx => {
            SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex( idx );
            return EditorGUI.GetPropertyHeight( elementProp, includeChildren: true ) - EditorGUIUtility.singleLineHeight;
        };

        return list;
    }

    public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label )
    {
        ReorderableList list = GetReorderableList( serializedProperty );

        list.DoList( rect );
    }

    public override float GetPropertyHeight( SerializedProperty serializedProperty, GUIContent label )
    {
        return GetReorderableList( serializedProperty ).GetHeight();
    }
}
