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
    public PooledObject Spawn( Transform parent = null, Vector3 worldPosition = default( Vector3 ), Quaternion worldRotation = default( Quaternion ), bool leaveInactive = false ) {
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
    public TComponent Spawn<TComponent>( Transform parent = null, Vector3 worldPosition = default( Vector3 ), Quaternion worldRotation = default( Quaternion ), bool leaveInactive = false ) where TComponent : UnityEngine.Component {
        return Spawn( parent, worldPosition, worldRotation, leaveInactive ).GetComponent<TComponent>();
    }

    /// <summary>
    /// Deactivate a pooled object and return it to the pool
    /// </summary>
    /// <param name="pooledObject"></param>
    public void Despawn( PooledObject pooledObject ) {
        bool wasSpawned = _pooledObjects.Remove( pooledObject );
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

    /// <summary>
    /// Take a gameobject out of the pool
    /// </summary>
    /// <param name="pooledObject"></param>
    public void RemoveFromPool( PooledObject pooledObject ) {
        _spawnedObjects.Remove( pooledObject );
        _pooledObjects.Remove( pooledObject );
    }

    void FillPool( int numObjectsToSpawn, Transform parent ) {
        for ( int i = 0; i < numObjectsToSpawn; i++ ) {
            PooledObject newObject = InstantiateObject( parent, parent.position, parent.rotation );

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
