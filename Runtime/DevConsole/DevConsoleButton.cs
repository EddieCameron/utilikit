#if DEVELOPMENT_BUILD
/* DevConsoleButton.cs
 * ----------------------------
 */
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utilikit {
    /// <summary>
    /// Devconsole button
    /// </summary>
    public class DevConsoleButton : DevConsoleUIElement<DevConsoleButtonAttribute> {
        public Text text;

        protected override void DoInit() {
            text.text = Attribute.displayName;
        }

        void OnEnable() {
            GetComponent<Button>().onClick.AddListener( OnButtonClicked );
        }

        void OnDisable() {
            GetComponent<Button>().onClick.RemoveListener( OnButtonClicked );
        }

        protected void OnButtonClicked() {
            TargetMethod.Invoke( null, null ); // all console methods are static
        }
    }
}
#endif
