/* ReorderableListDrawer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utilikit {
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

            list.drawHeaderCallback += rect => {
                EditorGUI.LabelField( rect, prop.displayName );
            };

            list.drawElementCallback += ( rect, index, isActive, isFocused ) => {
                SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex( index );
                if ( elementProp.hasVisibleChildren ) {
                    EditorGUI.PropertyField( rect, elementProp, includeChildren: true );
                }
                else {
                    EditorGUI.PropertyField( rect, elementProp, includeChildren: true, label: GUIContent.none );   // dont draw label if its a single line
                }
            };

            list.elementHeightCallback += idx => {
                SerializedProperty elementProp = listProperty.GetArrayElementAtIndex( idx );
                return EditorGUI.GetPropertyHeight( elementProp );
            };

            return list;
        }

        public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label ) {
            ReorderableList list = GetReorderableList( serializedProperty );

            list.DoList( rect );
        }

        public override float GetPropertyHeight( SerializedProperty serializedProperty, GUIContent label ) {
            return GetReorderableList( serializedProperty ).GetHeight();
        }
    }
}
