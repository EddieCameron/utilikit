using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utilikit {
    public class AddressablePrefabPool {
        public GameObjectPool Pool { get; private set; }
        AsyncOperationHandle<GameObject> loadedAssetHandle;

        public AddressablePrefabPool( AssetReferenceGameObject prefabRef, string poolGroupKey = "" ) {
            loadedAssetHandle = Addressables.LoadAssetAsync<GameObject>( prefabRef );
            loadedAssetHandle.WaitForCompletion();
            Pool = GameObjectPoolController.GetOrCreatePoolForObject( loadedAssetHandle.Result, poolGroupKey: poolGroupKey );
        }

        public void Release() {
            Pool.DestroyPool();
            Pool = null;
            Addressables.Release( loadedAssetHandle );
        }
    }
}
