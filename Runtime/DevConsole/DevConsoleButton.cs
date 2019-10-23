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
    public class DevConsoleButton : MonoBehaviour {
        public Text text;

        public DevConsoleAttribute MethodAttribute { get; set; }
        public MethodInfo Method { get; set; }

        public void Init( MethodInfo methodInfo, DevConsoleAttribute attribute ) {
            this.Method = methodInfo;
            this.MethodAttribute = attribute;

            text.text = attribute.displayName;
        }

        void OnEnable() {
            GetComponent<Button>().onClick.AddListener( OnButtonClicked );
        }

        void OnDisable() {
            GetComponent<Button>().onClick.RemoveListener( OnButtonClicked );
        }

        protected void OnButtonClicked() {
            Method.Invoke( null, null ); // all console methods are static
        }
    }
}
#endif
