/* DevConsoleAttribute.cs
 * ----------------------------
 */
using System;

namespace Utilikit {
    /// <summary>
    /// Base attribute to mark up dev console items
    /// </summary>
    public abstract class DevConsoleAttribute : Attribute {
        public string displayName;
        public string screen;

        public DevConsoleAttribute( string displayName, string screen = "" ) {
            this.displayName = displayName;
            this.screen = screen;
        }
    }

    [AttributeUsage( AttributeTargets.Method )]
    public class DevConsoleButtonAttribute : DevConsoleAttribute {
        public DevConsoleButtonAttribute( string displayName, string screen = "" ) : base( displayName, screen ) { }
    }

    [AttributeUsage( AttributeTargets.Property )]
    public class DevConsoleToggleAttribute : DevConsoleAttribute {
        public DevConsoleToggleAttribute( string displayName, string screen = "" ) : base( displayName, screen ) { }
    }

    [AttributeUsage( AttributeTargets.Property )]
    public class DevConsoleSliderAttribute : DevConsoleAttribute {
        public float min, max;
        public DevConsoleSliderAttribute( string displayName, float min, float max, string screen = "" ) : base( displayName, screen ) {
            this.min = min;
            this.max = max;
        }
    }
}
