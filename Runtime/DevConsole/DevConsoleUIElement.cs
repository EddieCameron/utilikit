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
    public abstract class DevConsoleUIElement<TAttribute> : MonoBehaviour where TAttribute: DevConsoleAttribute {
        public TAttribute Attribute { get; private set; }

        public MethodInfo TargetMethod;
        public PropertyInfo TargetProperty;

        public void Init( TAttribute attribute, MethodInfo targetMethod = null, PropertyInfo targetProperty = null ) {
            this.Attribute = attribute;
            this.TargetMethod = targetMethod;
            this.TargetProperty = targetProperty;

            DoInit();
        }

        protected abstract void DoInit();
    }
}
#endif
