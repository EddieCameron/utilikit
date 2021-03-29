/* GuidDrawer.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
#nullable enable
using System;
using UnityEngine;
using UnityEditor;

namespace Utilikit {
    [CustomPropertyDrawer( typeof( GuidAttribute ) )]
    public class GuidDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            if ( string.IsNullOrEmpty( property.stringValue ) ) {
                GenerateGuid( property );
            }

            int buttonWidth = 64;
            Rect propRect = position;
            propRect.width -= buttonWidth;

            Rect buttonRect = position;
            buttonRect.x = propRect.x + propRect.width;
            buttonRect.width = buttonWidth;
            if ( GUI.Button( buttonRect, "Regen" ) )
                GenerateGuid( property );

            bool guiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField( propRect, property );
            GUI.enabled = guiEnabled;
        }

        void GenerateGuid( SerializedProperty prop ) {
            prop.stringValue = Guid.NewGuid().ToString();
            EditorUtility.SetDirty( prop.serializedObject.targetObject );
        }
    }
}
