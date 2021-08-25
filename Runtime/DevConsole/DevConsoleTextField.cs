#if DEVELOPMENT_BUILD
/* DevConsoleSlider.cs
 * ----------------------------
 */
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace Utilikit {
    /// <summary>
    /// An integer quantity that can be changed from the devconsole
    /// </summary>
    public class DevConsoleTextField : DevConsoleUIElement<DevConsoleTextFieldAttribute> {
        public Text titleText;
        public InputField inputField;

        protected Func<string> GetQuantityMethod;
        protected Action<string> SetQuantityMethod;

        private string _lastValue;

        protected override void DoInit() {
            GetQuantityMethod = () => (string)TargetProperty.GetValue( null, null );
            SetQuantityMethod = newQuantity => TargetProperty.SetValue( null, newQuantity, null );

            titleText.text = Attribute.displayName;

            inputField.onEndEdit.AddListener( OnInputEntered );

            UpdateQuantity();
        }

        protected void OnEnable() {
            UpdateQuantity();
        }

        public void UpdateQuantity() {
            if ( GetQuantityMethod != null ) {
                _lastValue = GetQuantityMethod();
                inputField.text = _lastValue;
            }
        }

        public void OnInputEntered( string input ) {
            SetQuantityMethod?.Invoke( input );
            UpdateQuantity();
        }
    }

    [AttributeUsage( AttributeTargets.Property )]
    public class DevConsoleTextFieldAttribute : DevConsoleAttribute {
        public DevConsoleTextFieldAttribute( string displayName, string screen = "" ) : base( displayName, screen ) {
        }
    }
}
#endif
