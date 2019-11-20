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
    public class DevConsoleToggle : MonoBehaviour {
        public Text text;
        public Toggle toggle;

        protected Func<bool> GetMethod;
        protected Action<bool> SetMethod;

        public void Init( PropertyInfo propertyInfo, DevConsoleAttribute attribute ) {
            GetMethod = () => (bool)propertyInfo.GetValue( null, null );
            SetMethod = newQuantity => propertyInfo.SetValue( null, newQuantity, null );

            text.text = attribute.displayName;
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
