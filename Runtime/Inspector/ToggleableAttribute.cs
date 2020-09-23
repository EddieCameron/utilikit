/* ToggleableAttribute.cs
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

namespace Utilikit {
    public class ToggleableAttribute : PropertyAttribute {

        public string toggleOnField;
        public bool showWhenToggleOnTrue;

        public ToggleableAttribute( string toggleOnField, bool showWhenToggleOnTrue = true ) {
            this.toggleOnField = toggleOnField;
            this.showWhenToggleOnTrue = showWhenToggleOnTrue;
        }
    }
}
