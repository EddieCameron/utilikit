using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

/// <summary>
/// A pool to manage spawning and despawning of gameobjects
/// </summary>
public class GameObjectPool {
    public readonly GameObject SourceObject;

    HashSet<PooledObject> _spawnedObjects;
    public IReadOnlyCollection<PooledObject> SpawnedObjects => _spawnedObjects;

    Transform _despawnedObjectContainer;
    LinkedList<PooledObject> _pooledObjects;
    int _spawnCounter = 0;

    /// <summary>
    /// Create a new GameObject pool for the given sourceObject
    /// </summary>
    /// <param name="sourceObject"></param>
    /// <param name="despawnedObjectContainer"></param>
    /// <param name="startSize"></param>
    public GameObjectPool( GameObject sourceObject, Transform despawnedObjectContainer, int startSize ) {
        this.SourceObject = sourceObject;
        this._despawnedObjectContainer = despawnedObjectContainer;

        _spawnedObjects = new HashSet<PooledObject>();
        _pooledObjects = new LinkedList<PooledObject>();

        FillPool( startSize, _despawnedObjectContainer );
    }

    /// <summary>
    /// Get an object from the pool, if it's empty, instantiate a new one
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public PooledObject Spawn( Vector3 worldPosition = default( Vector3 ), Quaternion worldRotation = default( Quaternion ), Transform parent = null, bool leaveInactive = false ) {
        if ( _pooledObjects.Count > 0 ) {
            // pull from pool
            PooledObject toSpawn = _pooledObjects.First.Value;
            _pooledObjects.RemoveFirst();

            toSpawn.transform.position = worldPosition;
            toSpawn.transform.rotation = worldRotation;
            toSpawn.transform.SetParent( parent, worldPositionStays: true );

            _spawnedObjects.Add( toSpawn );

            toSpawn.OnSpawnedFromPool( this );

            if ( !leaveInactive )
                toSpawn.gameObject.SetActive( true );

            return toSpawn;
        }
        else {
            // pool empty, instantiate
            PooledObject toSpawn = InstantiateObject( parent, worldPosition, worldRotation );
            if ( leaveInactive )
                toSpawn.gameObject.SetActive( false );

            _spawnedObjects.Add( toSpawn );
            toSpawn.OnSpawnedFromPool( this );
            return toSpawn;
        }
    }

    /// <summary>
    /// Get an object from the pool, if it's empty, instantiate a new one
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public TComponent Spawn<TComponent>( Vector3 worldPosition = default( Vector3 ), Quaternion worldRotation = default( Quaternion ), Transform parent = null, bool leaveInactive = false ) where TComponent : UnityEngine.Component {
        return Spawn( worldPosition, worldRotation, parent, leaveInactive ).GetComponent<TComponent>();
    }

    /// <summary>
    /// Deactivate a pooled object and return it to the pool
    /// </summary>
    /// <param name="pooledObject"></param>
    public void Despawn( PooledObject pooledObject ) {
        bool wasSpawned = _spawnedObjects.Remove( pooledObject );
        if ( !wasSpawned ) {
            throw new ArgumentException( "Tried to despawn an object that didn't come from this pool" );
        }

        pooledObject.gameObject.SetActive( false );
        pooledObject.transform.SetParent( _despawnedObjectContainer );
        _pooledObjects.AddLast( pooledObject );

        pooledObject.OnDespawnedFromPool();
    }

    /// <summary>
    /// Deactivate a pooled object and return it to the pool
    /// </summary>
    /// <param name="pooledObject"></param>
    public void Despawn( GameObject gameObject ) {
        PooledObject pooledObject = gameObject.GetComponent<PooledObject>();
        if ( pooledObject == null ) {
            throw new ArgumentException( "Tried to despawn a gameobject that has no PooledObject component" );
        }

        Despawn( pooledObject );
    }

    List<PooledObject> _spawnedObjectTempCache;

    /// <summary>
    /// Return all spawned game objects to the pool
    /// </summary>
    public void DespawnAll() {
        // make temp cache in case spawned objects are modified during despawn
        if ( _spawnedObjectTempCache == null )
            _spawnedObjectTempCache = new List<PooledObject>( _spawnedObjects.Count );
        else
            _spawnedObjectTempCache.Clear();

        foreach ( var obj in _spawnedObjects )
            _spawnedObjectTempCache.Add( obj );

        foreach ( var obj in _spawnedObjectTempCache )
            obj.DespawnToPool();
    }

    /// <summary>
    /// Take a gameobject out of the pool
    /// </summary>
    /// <param name="pooledObject"></param>
    public void RemoveFromPool( PooledObject pooledObject ) {
        _spawnedObjects.Remove( pooledObject );
        _pooledObjects.Remove( pooledObject );
    }

    List<PooledObject> _objectsToDestroyTempCache;
    /// <summary>
    /// Destroy all spawned and despawned objects in the pool
    /// </summary>
    public void DestroyPool() {
        DespawnAll();

        if ( _objectsToDestroyTempCache == null )
            _objectsToDestroyTempCache = new List<PooledObject>();
        else
            _objectsToDestroyTempCache.Clear();
        _objectsToDestroyTempCache.AddRange( _pooledObjects );

        foreach ( var item in _objectsToDestroyTempCache ) {
            RemoveFromPool( item );
            GameObject.Destroy( item.gameObject );
        }

        if ( _spawnedObjects.Count > 0 || _pooledObjects.Count > 0 ) {
            Debug.LogError( "Items were created while pool was despawned." );
        }
    }

    void FillPool( int numObjectsToSpawn, Transform parent ) {
        for ( int i = 0; i < numObjectsToSpawn; i++ ) {
            PooledObject newObject = InstantiateObject( parent, parent ? parent.position : Vector3.zero, parent ? parent.rotation : Quaternion.identity );

            newObject.gameObject.SetActive( false );
            _pooledObjects.AddLast( newObject );
        }
    }

    PooledObject InstantiateObject( Transform parent, Vector3 worldPosition, Quaternion worldRotation ) {
        GameObject newObject = UnityEngine.Object.Instantiate( SourceObject, worldPosition, worldRotation, parent );
        newObject.name = $"{SourceObject.name} [Pooled {_spawnCounter++}]";

        PooledObject pooledObject = newObject.GetComponent<PooledObject>();
        if ( pooledObject == null )
            pooledObject = newObject.AddComponent<PooledObject>();

        return pooledObject;
    }
}
