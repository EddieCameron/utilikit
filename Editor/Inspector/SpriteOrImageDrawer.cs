/* SpriteOrImageDrawer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    [CustomPropertyDrawer( typeof( SpriteOrImage ))]
    public class SpriteOrImageDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            SerializedProperty spriteProp = property.FindPropertyRelative( "_spriteRenderer" );
            SerializedProperty imageProp = property.FindPropertyRelative( "_image" );

            bool isImage = imageProp.objectReferenceValue != null;
            UnityEngine.Object referencedObject = isImage ? imageProp.objectReferenceValue : spriteProp.objectReferenceValue;

            var draggedObject = EditorGUI.ObjectField( position, label, referencedObject, typeof( Component ), allowSceneObjects: true );
            if ( draggedObject != referencedObject ) {
                Component draggedComponent = GetImageOrSpriteFromDraggedObject( draggedObject );
                
                if ( draggedComponent is Image ) {
                    spriteProp.objectReferenceValue = null;
                    imageProp.objectReferenceValue = draggedComponent;
                }
                else if ( draggedComponent is SpriteRenderer ) {
                    spriteProp.objectReferenceValue = draggedComponent;
                    imageProp.objectReferenceValue = null;
                }
                else {
                    spriteProp.objectReferenceValue = null;
                    imageProp.objectReferenceValue = null;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        Component GetImageOrSpriteFromDraggedObject( UnityEngine.Object draggedObject ) {
            if ( draggedObject == null )
                return null;

            if ( draggedObject is Image i )
                return i;
            if ( draggedObject is SpriteRenderer s )
                return s;

            if ( draggedObject is Component c ) {
                // find a valid image/sprite on same gameobject
                SpriteRenderer sprite = c.GetComponent<SpriteRenderer>();
                if ( sprite != null )
                    return sprite;

                Image image = c.GetComponent<Image>();
                if ( image != null )
                    return image;
            }

            return null;    // no valid object
        }
    }
}
