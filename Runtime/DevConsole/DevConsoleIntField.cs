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
    public class DevConsoleIntField : MonoBehaviour {
        public Text titleText;
        public InputField inputField;

        protected Func<int> GetQuantityMethod;
        protected Action<int> SetQuantityMethod;

        bool _allowNegative;
        private int _lastValue;

        public void Init( PropertyInfo onProperty, DevConsoleIntFieldAttribute attr ) {
            GetQuantityMethod = () => (int)onProperty.GetValue( null, null );
            SetQuantityMethod = newQuantity => onProperty.SetValue( null, newQuantity, null );

            titleText.text = attr.displayName;

            inputField.characterValidation = InputField.CharacterValidation.Integer;
            inputField.onEndEdit.AddListener( OnInputEntered );

            _allowNegative = attr.allowNegative;

            UpdateQuantity();
        }

        protected void OnEnable() {
            UpdateQuantity();
        }

        public void UpdateQuantity() {
            if ( GetQuantityMethod != null ) {
                _lastValue = GetQuantityMethod();
                inputField.text = _lastValue.ToString();
            }
        }

        public void OnInputEntered( string input ) {
            if ( int.TryParse( input, out int enteredInt ) &&
                    ( _allowNegative || enteredInt >= 0 ) ) {
                SetQuantityMethod?.Invoke( enteredInt );
                UpdateQuantity();
            }
            else {
                inputField.text = _lastValue.ToString();
            }
        }
    }

    [AttributeUsage( AttributeTargets.Property )]
    public class DevConsoleIntFieldAttribute : DevConsoleAttribute {
        public bool allowNegative;
        public DevConsoleIntFieldAttribute( string displayName, bool allowNegative = true, string screen = "" ) : base( displayName, screen ) {
            this.allowNegative = allowNegative;
        }
    }
}
#endif
