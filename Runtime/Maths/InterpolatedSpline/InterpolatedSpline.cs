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

        protected Vector2[] controlPoints;

        public virtual void SetControlPoints( ICollection<Vector2> controlPoints ) {
            this.controlPoints = new Vector2[controlPoints.Count];
            controlPoints.CopyTo( this.controlPoints, 0 );
        }

        /// <summary>
        /// Get the point at normalized distance along curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Vector2 Interpolate( float t );
    }
}
