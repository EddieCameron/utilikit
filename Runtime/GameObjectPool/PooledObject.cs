using System;
using UnityEngine;

namespace Utilikit {
    /// <summary>
    /// On each instance of a pooled game object
    /// </summary>
    public class PooledObject : MonoBehaviour {
        public GameObjectPool SourcePool { get; private set; }

        public bool WasSpawnedFromPool => SourcePool != null;

        public event Action<PooledObject> DidSpawn;
        public event Action<PooledObject> DidDespawn;

        bool isDespawning;

        void OnDestroy() {
            if ( WasSpawnedFromPool ) {
                SourcePool.RemoveFromPool( this );
                SourcePool = null;
            }
        }

        /// <summary>
        /// Deactivate this object and return to its pool to be recycled
        /// </summary>
        public void DespawnToPool() {
            if ( isDespawning )
                return; // already despawned/ing

            if ( !WasSpawnedFromPool )
                throw new InvalidOperationException( "Tried to return a gameobject to a pool, but it didn't come from one" );

            SourcePool.Despawn( this );
            SourcePool = null;
        }

        /// <summary>
        /// Called when this object is spawned from a pool, should not be called any other time
        /// </summary>
        /// <param name="pool"></param>
        internal void OnSpawnedFromPool( GameObjectPool pool ) {
            if ( WasSpawnedFromPool )
                throw new InvalidOperationException( "OnSpawnedFromPool called but object has already been spawned" );

            SourcePool = pool;
            isDespawning = false;

            DidSpawn?.Invoke( this );
        }

        /// <summary>
        /// Called when this object is despawned from a pool, should not be called any other time
        /// </summary>
        /// <param name="pool"></param>
        internal void OnDespawnedFromPool() {
            if ( !WasSpawnedFromPool )
                throw new InvalidOperationException( "OnDespawnedFromPool called but object wasn't spawned" );

            SourcePool = null;
            isDespawning = true;

            DidDespawn?.Invoke( this );
        }
    }
}
