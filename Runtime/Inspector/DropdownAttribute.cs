/* Dropdown.cs
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

/// <summary>
/// Use on a string field to show a dropdown list of possible options
/// Options are found by calling the static method in the attribute constructor, returning a collection of strings.
/// If no type is given, static method will be called on the field's containing class
/// </summary>
public class DropdownAttribute : PropertyAttribute {

    public string staticMethodType;
    public string staticMethodName;

    public DropdownAttribute( string getStringListStaticMethodType, string getStringListStaticMethodName ) {
        this.staticMethodType = getStringListStaticMethodType;
        this.staticMethodName = getStringListStaticMethodName;
    }

    public DropdownAttribute( string getStringListStaticMethodName ) {
        this.staticMethodType = "";
        this.staticMethodName = getStringListStaticMethodName;
    }
}
