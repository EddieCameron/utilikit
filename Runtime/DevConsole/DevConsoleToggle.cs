#if DEVELOPMENT_BUILD
/* DevConsoleToggle.cs
 * ----------------------------
 */
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Utilikit {
    /// <summary>
    /// Devconsole toggle for a boolean prop
    /// </summary>
    public class DevConsoleToggle : DevConsoleUIElement<DevConsoleToggleAttribute> {
        public Text text;
        public Toggle toggle;

        protected Func<bool> GetMethod;
        protected Action<bool> SetMethod;

        protected override void DoInit() {
            GetMethod = () => (bool)TargetProperty.GetValue( null, null );
            SetMethod = newQuantity => TargetProperty.SetValue( null, newQuantity, null );

            text.text = Attribute.displayName;
            toggle.onValueChanged.AddListener( OnToggled );

            UpdateToggle();
        }

        void OnEnable() {
            UpdateToggle();
        }

        void UpdateToggle() {
            if ( GetMethod != null ) {
                toggle.isOn = GetMethod();
            }
        }

        protected void OnToggled( bool newValue ) {
            SetMethod?.Invoke( newValue );
            UpdateToggle();
        }
    }
}
#endif
