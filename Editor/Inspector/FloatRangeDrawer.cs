/* FloatRangeDrawer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utilikit {
    [CustomPropertyDrawer( typeof( FloatRange ), useForChildren: true )]
    public class FloatRangeDrawer : PropertyDrawer {
        static GUIContent[] fieldLabels = new[] { new GUIContent( "Min" ), new GUIContent( "Max" ) };
        float[] values = new float[2];

        public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label ) {
            SerializedProperty minProp = serializedProperty.FindPropertyRelative( "min" );
            SerializedProperty maxProp = serializedProperty.FindPropertyRelative( "max" );

            using ( var prop = new EditorGUI.PropertyScope( rect, label, serializedProperty ) ) {
                var valuesRect = EditorGUI.PrefixLabel( rect, label );
                float fieldWidth = valuesRect.width * .5f;

                // var labelWidth = EditorGUIUtility.labelWidth;
                // EditorGUIUtility.labelWidth = 24f;
                // var minRect = valuesRect;
                // minRect.width = fieldWidth - 8f;

                values[0] = minProp.floatValue;
                values[1] = maxProp.floatValue;
                EditorGUI.BeginChangeCheck();
                EditorGUI.MultiFloatField( valuesRect, fieldLabels, values );
                if ( EditorGUI.EndChangeCheck() ) {
                    values[1] = Mathf.Max( values[1], values[0] );
                    minProp.floatValue = values[0];
                    maxProp.floatValue = values[1];
                }

                // var maxRect = minRect;
                // maxRect.x += fieldWidth;
                // EditorGUI.PropertyField( maxRect, maxProp );
                // EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        public override float GetPropertyHeight( SerializedProperty serializedProperty, GUIContent label ) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
