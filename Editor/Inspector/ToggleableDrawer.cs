/* ToggleableDrawer.cs
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
using System.Reflection;

namespace Utilikit {
    [CustomPropertyDrawer( typeof( ToggleableAttribute ) )]
    public class ToggleableDrawer : PropertyDrawer {
        public static object GetParentObject( SerializedProperty property ) {
            string[] path = property.propertyPath.Split( '.' );

            object propertyObject = property.serializedObject.targetObject;

            object propertyParent = null;
            for ( int i = 0; i < path.Length; ++i ) {
                if ( path[i] == "Array" ) {
                    int index = (int)( path[i + 1][path[i + 1].Length - 2] - '0' );
                    propertyObject = ( (IList)propertyObject )[index];
                    ++i;
                }
                else {
                    propertyParent = propertyObject;
                    propertyObject = propertyObject.GetType().GetField( path[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( propertyObject );
                }
            }

            return propertyParent;
        }



        bool GetToggleOnFieldValue( SerializedProperty prop ) {
            var parentObject = GetParentObject( prop );
            var toggleOnField = parentObject.GetType().GetField( ( (ToggleableAttribute)attribute ).toggleOnField );
            if ( toggleOnField == null ) {
                Debug.LogWarning( $"Toggle On Field: \"{( (ToggleableAttribute)attribute ).toggleOnField}\" not found" );
                return true;
            }

            var fieldValue = toggleOnField.GetValue( parentObject );
            if ( fieldValue is bool boolValue )
                return boolValue;

            Debug.LogWarning( $"Boolean Toggle On Field: \"{( (ToggleableAttribute)attribute ).toggleOnField}\" not a bool" );
            return true;
        }

        public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label ) {
            var toggleValue = GetToggleOnFieldValue( serializedProperty );

            bool showProp = toggleValue == ( (ToggleableAttribute)attribute ).showWhenToggleOnTrue;
            if ( showProp )
                EditorGUI.PropertyField( rect, serializedProperty, label, true );
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
            var toggleValue = GetToggleOnFieldValue( property );

            bool showProp = toggleValue == ( (ToggleableAttribute)attribute ).showWhenToggleOnTrue;
            if ( showProp )
                return base.GetPropertyHeight( property, label );
            else
                return 0;
        }
    }
}
