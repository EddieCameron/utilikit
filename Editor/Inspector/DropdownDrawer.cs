/* DropdownDrawer.cs
 * © Eddie Cameron 2021
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
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer( typeof( DropdownAttribute ))]
public class DropdownDrawer : PropertyDrawer {

    GUIContent[] dropdownOptions;

    void GetDropdownOptions() {
        DropdownAttribute dropdownAttr = (DropdownAttribute)attribute;
        Type staticType;
        if ( string.IsNullOrEmpty( dropdownAttr.staticMethodType ) ) {
            staticType = fieldInfo.DeclaringType;
        }
        else {
            staticType = fieldInfo.DeclaringType.Assembly.GetType( dropdownAttr.staticMethodType );
            if ( staticType == null ) {
                Debug.LogError( "Get dropdown options type not found: " + dropdownAttr.staticMethodType );
                return;
            }
        }

        var getOptionsMethod = staticType.GetMethod( dropdownAttr.staticMethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
        if ( getOptionsMethod == null ) {
            // try getting property
            getOptionsMethod = staticType.GetProperty( dropdownAttr.staticMethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public )?.GetMethod;
        }
        if ( getOptionsMethod == null ) {
            Debug.LogError( "Get dropdown options method not found: " + dropdownAttr.staticMethodName );
            return;
        }

        var optionsEnumerable = getOptionsMethod.Invoke( null, null ) as IEnumerable<string>;
        if ( optionsEnumerable == null ) {
            Debug.LogError( "Get dropdown options method doesn't return an enumerable string collection" );
            return;
        }
        dropdownOptions = optionsEnumerable.Select( s => new GUIContent( s ) ).ToArray();
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        if ( dropdownOptions == null ) {
            GetDropdownOptions();
        }

        if ( dropdownOptions == null ) {
            EditorGUI.PropertyField( position, property, label );
            return;
        }

        int selectedOption = -1;
        for ( int i = 0; i < dropdownOptions.Length; i++ ) {
            if ( property.stringValue == dropdownOptions[i].text ) {
                selectedOption = i;
                break;
            }
        }

        var startColour = GUI.contentColor;
        if ( selectedOption == -1 ) {
            // invalid
            GUI.contentColor = Color.red;
        }
        selectedOption = EditorGUI.Popup( position, label, selectedOption, dropdownOptions );
        if ( selectedOption >= 0 ) {
            property.stringValue = dropdownOptions[selectedOption].text;
        }
        GUI.contentColor = startColour;
    }
}
