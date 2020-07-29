/* InterpolatedCurve.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    public abstract class InterpolatedSpline {

        protected List<Vector2> controlPoints = new List<Vector2>();

        public virtual void SetControlPoints( ICollection<Vector2> controlPoints ) {
            this.controlPoints.Clear();
            this.controlPoints.AddRange( controlPoints );
        }

        /// <summary>
        /// Get the point at normalized distance along curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Vector2 Interpolate( float t );
    }
}
