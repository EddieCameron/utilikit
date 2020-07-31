/* ReadOnlyDrawer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utilikit {
    [CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label ) {
            var enabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField( rect, serializedProperty, label );
            GUI.enabled = enabled;
        }
    }
}
