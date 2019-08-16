using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public static class TransformExtensions {
        /// <summary>
        /// Sets the X position.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="space">Space, (world or self) Default is self</param>
        public static void SetXPos( this Transform transform, float x, Space space = Space.Self ) {
            if ( space == Space.Self ) {
                Vector3 newPos = transform.localPosition;
                newPos.x = x;
                transform.localPosition = newPos;
            }
            else {
                Vector3 newPos = transform.position;
                newPos.x = x;
                transform.position = newPos;
            }
        }

        /// <summary>
        /// Sets the Y position.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="space">Space, (world or self) Default is self</param>
        public static void SetYPos( this Transform transform, float y, Space space = Space.Self ) {
            if ( space == Space.Self ) {
                Vector3 newPos = transform.localPosition;
                newPos.y = y;
                transform.localPosition = newPos;
            }
            else {
                Vector3 newPos = transform.position;
                newPos.y = y;
                transform.position = newPos;
            }
        }

        /// <summary>
        /// Sets the Z position.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="space">Space, (world or self) Default is self</param>
        public static void SetZPos( this Transform transform, float z, Space space = Space.Self ) {
            if ( space == Space.Self ) {
                Vector3 newPos = transform.localPosition;
                newPos.z = z;
                transform.localPosition = newPos;
            }
            else {
                Vector3 newPos = transform.position;
                newPos.z = z;
                transform.position = newPos;
            }
        }

        /// <summary>
        /// Sets the X scale.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="x">The x coordinate.</param>
        public static void SetXScale( this Transform transform, float x ) {
            Vector3 newScale = transform.localScale;
            newScale.x = x;
            transform.localScale = newScale;
        }

        /// <summary>
        /// Sets the Y scale.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="y">The y coordinate.</param>
        public static void SetYScale( this Transform transform, float y ) {
            Vector3 newScale = transform.localScale;
            newScale.y = y;
            transform.localScale = newScale;
        }

        /// <summary>
        /// Sets the Z scale.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="z">The z coordinate.</param>
        public static void SetZScale( this Transform transform, float z ) {
            Vector3 newScale = transform.localScale;
            newScale.z = z;
            transform.localScale = newScale;
        }

        /// <summary>
        /// Renames the unity InverseTransformPoint method
        /// </summary>
        /// <returns>The local position.</returns>
        /// <param name="transform"></param>
        /// <param name="worldPos">world position.</param>
        /// <param name="position">If set to <c>true</c>, transforms as position, not direction.</param>
        public static Vector3 WorldToLocalPosition( this Transform transform, Vector3 worldPos, bool position = true ) {
            if ( position )
                return transform.InverseTransformPoint( worldPos );
            else
                return transform.InverseTransformDirection( worldPos );
        }

        /// <summary>
        /// Renames the Unity InverseTransformDirection method
        /// </summary>
        /// <returns>The local direction.</returns>
        /// <param name="transform"></param>
        /// <param name="worldDirection">World direction.</param>
        public static Vector3 WorldToLocalDirection( this Transform transform, Vector3 worldDirection ) {
            return transform.InverseTransformDirection( worldDirection );
        }

        /// <summary>
        /// Renames the unity TransformPoint method
        /// </summary>
        /// <returns>The world position.</returns>
        /// <param name="transform"></param>
        /// <param name="localPos">Local position.</param>
        public static Vector3 LocalToWorldPosition( this Transform transform, Vector3 localPos ) {
            return transform.TransformPoint( localPos );
        }

        /// <summary>
        /// Renames the Unity TransformDirection method
        /// </summary>
        /// <returns>The world direction.</returns>
        /// <param name="transform"></param>
        /// <param name="localDirection">Local direction.</param>
        public static Vector3 LocalToWorldDirection( this Transform transform, Vector3 localDirection ) {
            return transform.TransformDirection( localDirection );
        }

        /// <summary>
        /// Get the ray in world space starting at this position and going along transform.forward
        /// </summary>
        /// <returns>The ray.</returns>
        /// <param name="transform">Transform.</param>
        public static Ray GetRay( this Transform transform ) {
            return new Ray( transform.position, transform.forward );
        }
    }
}
