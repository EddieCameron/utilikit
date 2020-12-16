/* PooledObjectList.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

[Serializable]
public class PooledObjectList<T> where T : Component {
    public Transform spawnRoot;
    public T prefabToSpawn;

    GameObjectPool _spawnPool;
    GameObjectPool SpawnPool {
        get {
            _spawnPool = _spawnPool ?? GameObjectPoolController.GetOrCreatePoolForObject( prefabToSpawn.gameObject );
            return _spawnPool;
        }
    }

    List<T> spawnedObjects = new List<T>();
    public IReadOnlyList<T> SpawnedObjects => spawnedObjects;

    public T Spawn() {
        CheckSpawn();

        var newObj = SpawnPool.Spawn<T>( spawnRoot.position, spawnRoot.rotation, parent: spawnRoot );
        newObj.transform.localScale = Vector3.one;

        spawnedObjects.Add( newObj );
        return newObj;
    }
    public void Clear() {
        foreach ( var obj in spawnedObjects ) {
            if ( obj != null ) {
                SpawnPool.Despawn( obj.gameObject );
            }
        }
        spawnedObjects.Clear();
    }

    public bool Despawn(T toDespawn) {
        if ( spawnedObjects.Remove( toDespawn ) ) {
            SpawnPool.Despawn( toDespawn.gameObject );
            return true;
        }

        return false;
    }

    void CheckSpawn() {
        for ( int i = spawnedObjects.Count - 1; i >= 0; i-- ) {
            if ( spawnedObjects[i] == null )
                spawnedObjects.RemoveAt( i );
        }
    }

}
