using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public static class CameraExtensions {

        /// <summary>
        /// Returns true if the given point is within the camera frustrum
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsWorldPointInViewport( this Camera cam, Vector3 point ) {
            Vector3 viewportPoint = cam.WorldToViewportPoint( point );
            return viewportPoint.z > 0 && viewportPoint.x >= 0 && viewportPoint.x < 1 && viewportPoint.y >= 0 && viewportPoint.y < 1;
        }
    }
}
