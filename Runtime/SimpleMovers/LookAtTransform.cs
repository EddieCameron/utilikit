/* LookAtTransform.cs
 * Copyright Eddie Cameron & Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilikit;

namespace SimpleMovers
{
    [ExecuteInEditMode]
    public class LookAtTransform : MonoBehaviour
    {
        public Transform target;
        public bool reverse;
        public Vector3 worldUp = Vector3.zero;
        public bool doOnce = false;

        public bool useMainCam;
        public RotationAxes constrainToLocalAxis;

        public bool alignWithPlaneOnly;

        protected void Start() {
            if (useMainCam && target == null)
                // Setting the main camera in edit mode when it's in another
                // scene results in build errors, so here we check to make sure
                // we're in play mode, or that it's in our scene.
                if (Camera.main != null && (Application.isPlaying || Camera.main.gameObject.scene == gameObject.scene))
                    target = Camera.main.transform;

            LookAt();
            if ( doOnce )
                enabled = false;
        }

        protected void Update()
        {
            LookAt();
        }

        void LookAt() {
            if ( useMainCam && target == null ) {
                // Setting the main camera in edit mode when it's in another
                // scene results in build errors, so here we check to make sure
                // we're in play mode, or that it's in our scene.
                if ( Camera.main != null && ( Application.isPlaying || Camera.main.gameObject.scene == gameObject.scene ) )
                    target = Camera.main.transform;
            }

            if ( target ) {
                if ( !alignWithPlaneOnly && constrainToLocalAxis == RotationAxes.None ) {
                    Vector3 lookPos = target.position;
                    if ( reverse )
                        lookPos = 2 * transform.position - target.position;
                    if ( worldUp == Vector3.zero )
                        transform.LookAt( lookPos );
                    else
                        transform.LookAt( lookPos, worldUp );
                } else {
                    transform.localRotation = Quaternion.identity; // reset rotation to fix constraints in case they change

                    Vector3 lookDir;
                    if ( alignWithPlaneOnly )
                        lookDir = target.forward;
                    else
                        lookDir = target.position - transform.position;

                    if ( reverse )
                        lookDir = -lookDir;

                    Vector3 toTargetLocal = transform.WorldToLocalDirection( lookDir );
                    if ( constrainToLocalAxis != RotationAxes.None )
                        toTargetLocal[(int)constrainToLocalAxis - 1] = 0;    // no movement on constrained axis

                    transform.localRotation = Quaternion.LookRotation( toTargetLocal );
                }
            }

        }

        public enum RotationAxes
        {
            None,
            XAxis,
            YAxis,
        }
    }
}
