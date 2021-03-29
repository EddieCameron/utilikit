/* GuidAttribute.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
#nullable enable
using System;
using UnityEngine;

namespace Utilikit {
    [AttributeUsage( AttributeTargets.Field )]
    public class GuidAttribute : PropertyAttribute {
    }
}
