using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    [RequireComponent( typeof( Collider2D ))]
    public class Trigger2D : MonoBehaviour {
        [Tooltip( "Objects on these layers will activate the trigger" )]
        public LayerMask layerMask;

        HashSet<Collider2D> collidersWithin = new HashSet<Collider2D>();

        [Serializable]
        public class TriggerUnityEvent : UnityEngine.Events.UnityEvent<Collider2D> { }
        public TriggerUnityEvent TriggerEntered = new TriggerUnityEvent();
        public TriggerUnityEvent TriggerLeft = new TriggerUnityEvent();

        /// <summary>
        /// Is the given collider within the trigger
        /// </summary>
        /// <param name="toTest"></param>
        /// <returns></returns>
        public bool IsColliderWithin( Collider2D toTest ) => collidersWithin.Contains( toTest );

        public bool AnyCollidersWithin() => collidersWithin.Count > 0;

        public int NumCollidersWithin() => collidersWithin.Count;

        public IEnumerable<Collider2D> AllColliders {
            get {
                foreach ( var collider in collidersWithin ) {
                    yield return collider;
                }
            }
        }

        void OnTriggerEnter2D( Collider2D other ) {
            if ( !enabled )
                return;

            if ( ( ( 1 << other.gameObject.layer ) & layerMask.value ) != 0 ) {
                collidersWithin.TryAddUnique( other );

                TriggerEntered?.Invoke( other );
            }
        }

        void OnTriggerExit2D( Collider2D other ) {
            collidersWithin.Remove( other );

            if ( !enabled )
                return;

            if ( ( ( 1 << other.gameObject.layer ) & layerMask.value ) != 0 ) {
                TriggerLeft?.Invoke( other );
            }
        }
    }
}
