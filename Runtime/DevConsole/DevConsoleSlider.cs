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
    /// A slider value that can be changed from the dev console
    /// </summary>
    public class DevConsoleSlider : MonoBehaviour {
        public Text titleText;
        public Slider slider;
        public Text quantityText;

        protected Func<float> GetQuantityMethod;
        protected Action<float> SetQuantityMethod;

        public void Init( string title ) {
            titleText.text = title;
            UpdateQuantity();
        }

        public void Init( PropertyInfo onProperty, DevConsoleSliderAttribute attr ) {
            GetQuantityMethod = () => (float)onProperty.GetValue( null, null );
            SetQuantityMethod = newQuantity => onProperty.SetValue( null, newQuantity, null );

            titleText.text = attr.displayName;

            slider.minValue = attr.min;
            slider.maxValue = attr.max;
            slider.onValueChanged.AddListener( delegate { OnQuantityFieldUpdated( slider.value ); } );
            UpdateQuantity();
        }

        protected void OnEnable() {
            UpdateQuantity();
        }

        public void UpdateQuantity() {
            if ( GetQuantityMethod != null ) {
                slider.value = GetQuantityMethod();
                quantityText.text = GetQuantityMethod().ToString();
            }
        }

        public void OnQuantityFieldUpdated( float newAmount ) {
            if ( SetQuantityMethod != null ) {
                SetQuantityMethod( newAmount );
            }
            UpdateQuantity();
        }
    }
}
#endif
