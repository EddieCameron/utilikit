/* GameObjectPoolController.cs
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

namespace Utilikit
{
    /// <summary>
    /// Manages creation and destruction of pools, in optional groups
    /// /// </summary>
    public static class GameObjectPoolController {
        static Dictionary<string, GameObjectPoolGroup> _poolGroups = new Dictionary<string, GameObjectPoolGroup>();
        static Transform _despawnParent = null;

        [RuntimeInitializeOnLoadMethod]
        static void Init() {
            _poolGroups.Clear();
            if ( _despawnParent != null ) {
                GameObject.Destroy( _despawnParent.gameObject );
                _despawnParent = null;
            }
        }

        const string _DEFAULT_POOL_GROUP_KEY = "defaultPoolGroup";
        const int _DEFAULT_INITIAL_POOL_SIZE = 1;

        /// <summary>
        /// Returns whether a pool with the given key exists
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="poolGroupKey"></param>
        /// <returns></returns>
        public static bool HavePoolForObjectKey( string objectKey, string poolGroupKey = _DEFAULT_POOL_GROUP_KEY ) {
            return _poolGroups.TryGetValue( poolGroupKey, out GameObjectPoolGroup group ) && group.HavePoolForObjectKey( objectKey );
        }

        /// <summary>
        /// Returns the pool with the given key, or null if none exist
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="poolGroupKey"></param>
        /// <returns></returns>
        public static GameObjectPool GetPoolForObjectKey( string objectKey, string poolGroupKey = _DEFAULT_POOL_GROUP_KEY ) {
            if ( !_poolGroups.TryGetValue( poolGroupKey, out GameObjectPoolGroup group ) ) {
                Debug.LogWarning( "No pool group with key: " + poolGroupKey );
                return null;
            }


            if ( !group.TryGetPoolForObjectKey( objectKey, out GameObjectPool pool ) ) {
                Debug.LogWarning( "No pool found with key " + objectKey );
                return null;
            }

            return pool;
        }

        /// <summary>
        /// Return the pool for the given source object, or creates it (with an optional overridden key) if it doesn't exist
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="overrideObjectKey"></param>
        /// <param name="poolGroupKey"></param>
        /// <param name="initialPoolSize"></param>
        /// <returns></returns>
        public static GameObjectPool GetOrCreatePoolForObject( GameObject sourceObject, string overrideObjectKey = "", string poolGroupKey = _DEFAULT_POOL_GROUP_KEY, int initialPoolSize = _DEFAULT_INITIAL_POOL_SIZE ) {
            GameObjectPoolGroup group = GetOrCreateGroup( poolGroupKey );

            return group.GetOrCreatePoolForObject( sourceObject, overrideObjectKey, initialPoolSize );
        }

        public static void DespawnAllPoolsInGroup( string poolGroupKey = _DEFAULT_POOL_GROUP_KEY ) {
            if ( _poolGroups.TryGetValue( poolGroupKey, out GameObjectPoolGroup group ) )
                group.DespawnAllPools();
        }

        public static void DestroyAllPoolsInGroup( string poolGroupKey = _DEFAULT_POOL_GROUP_KEY ) {
            if ( _poolGroups.TryGetValue( poolGroupKey, out GameObjectPoolGroup group ) )
                group.DestroyAllPools();
        }

        static GameObjectPoolGroup GetOrCreateGroup( string groupName ) {
            GameObjectPoolGroup group;
            if ( !_poolGroups.TryGetValue( groupName, out group ) ) {
                // create group
                if ( _despawnParent == null ) {
                    // create base object to hold all despawned objects
                    _despawnParent = new GameObject( "Pooled Objects" ).transform;
                }

                Transform groupParent = new GameObject( groupName ).transform;
                groupParent.SetParent( _despawnParent );
                group = new GameObjectPoolGroup( groupName, groupParent );
                _poolGroups[groupName] = group;
            }
            return group;
        }

        private class GameObjectPoolGroup {
            public readonly string GroupKey;
            public readonly Transform GroupParent;

            Dictionary<string, GameObjectPool> _pools = new Dictionary<string, GameObjectPool>();
            Dictionary<GameObject, string> _gameObjectToKey = new Dictionary<GameObject, string>();

            public GameObjectPoolGroup( string groupKey, Transform groupParent ) {
                this.GroupKey = groupKey;
                this.GroupParent = groupParent;
            }

            public bool HavePoolForObjectKey( string objectKey ) => _pools.ContainsKey( objectKey );
            public GameObjectPool GetPoolForObjectKey( string objectKey ) => _pools[objectKey];
            public bool TryGetPoolForObjectKey( string objectKey, out GameObjectPool pool ) => _pools.TryGetValue( objectKey, out pool );


            public bool HavePoolForObject( GameObject gameObject ) {
                string key;
                if ( !_gameObjectToKey.TryGetValue( gameObject, out key ) ) {
                    return false;
                }

                return HavePoolForObjectKey( key );
            }

            public GameObjectPool GetOrCreatePoolForObject( GameObject sourceObject, string overrideObjectKey = "", int initialPoolSize = GameObjectPoolController._DEFAULT_INITIAL_POOL_SIZE ) {
                // get or create key
                string key;
                if ( _gameObjectToKey.TryGetValue( sourceObject, out key ) ) {
                    if ( !string.IsNullOrEmpty( overrideObjectKey ) && overrideObjectKey != key )
                        Debug.LogWarning( "Non-matching override key for pool that already exists will be ignored: " + overrideObjectKey );
                }
                else {
                    key = GetDefaultKeyForGameObject( sourceObject );
                    _gameObjectToKey[sourceObject] = key;
                }

                GameObjectPool pool;
                if ( !_pools.TryGetValue( key, out pool ) ) {
                    pool = new GameObjectPool( sourceObject, GroupParent, initialPoolSize );
                    _pools[key] = pool;
                }
                return pool;
            }

            public void DespawnAllPools() {
                foreach ( var pool in _pools.Values ) {
                    pool.DespawnAll();
                }
            }

            public void DestroyAllPools() {
                foreach ( var pool in _pools.Values ) {
                    pool.DestroyPool();
                }
                _pools.Clear();
                _gameObjectToKey.Clear();
            }

            string GetDefaultKeyForGameObject( GameObject sourceObject ) {
                return $"{sourceObject.name}_{sourceObject.GetInstanceID()}";
            }
        }
    }
}
