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
    public class DevConsoleSlider : DevConsoleUIElement<DevConsoleSliderAttribute> {
        public Text titleText;
        public Slider slider;
        public Text quantityText;

        protected Func<float> GetQuantityMethod;
        protected Action<float> SetQuantityMethod;

        protected override void DoInit() {
            GetQuantityMethod = () => (float)TargetProperty.GetValue( null, null );
            SetQuantityMethod = newQuantity => TargetProperty.SetValue( null, newQuantity, null );

            titleText.text = Attribute.displayName;

            slider.minValue = Attribute.min;
            slider.maxValue = Attribute.max;
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
