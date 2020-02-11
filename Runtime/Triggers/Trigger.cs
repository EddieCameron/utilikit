using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    [RequireComponent( typeof( Collider ))]
    public class Trigger : MonoBehaviour {
        [Tooltip( "Objects on these layers will activate the trigger" )]
        public LayerMask layerMask;

        HashSet<Collider> collidersWithin = new HashSet<Collider>();

        public class TriggerUnityEvent : UnityEngine.Events.UnityEvent<Collider> { }
        public TriggerUnityEvent TriggerEntered, TriggerStay, TriggerLeft;

        /// <summary>
        /// Is the given collider within the trigger
        /// </summary>
        /// <param name="toTest"></param>
        /// <returns></returns>
        public bool IsColliderWithin( Collider toTest ) => collidersWithin.Contains( toTest );

        public bool AnyCollidersWithin() => collidersWithin.Count > 0;

        public int NumCollidersWithin() => collidersWithin.Count;

        public IEnumerable<Collider> AllColliders {
            get {
                foreach ( var collider in collidersWithin ) {
                    yield return collider;
                }
            }
        }

        void OnTriggerEnter( Collider other ) {
            if ( !enabled )
                return;

            if ( ( ( 1 << other.gameObject.layer ) & layerMask.value ) != 0 ) {
                collidersWithin.TryAddUnique( other );

                TriggerEntered?.Invoke( other );
            }
        }

        void OnTriggerStay( Collider other ) {
            if ( !enabled )
                return;

            if ( ( ( 1 << other.gameObject.layer ) & layerMask.value ) != 0 ) {
                collidersWithin.TryAddUnique( other );

                TriggerStay?.Invoke( other );
            }
        }

        void OnTriggerExit( Collider other ) {
            collidersWithin.Remove( other );

            if ( !enabled )
                return;

            if ( ( ( 1 << other.gameObject.layer ) & layerMask.value ) != 0 ) {
                TriggerLeft?.Invoke( other );
            }
        }
    }
}
