using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) != 0;

        public static GameObject InstantiateAddressableSync(AssetReference reference, Vector3 position, Quaternion rotation)
        {
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(reference);

            var loadedAsset = loadHandle.WaitForCompletion();

            if (loadHandle.Status == AsyncOperationStatus.Succeeded && loadedAsset != null)
            {
                var instantiateHandle = Addressables.InstantiateAsync(reference, position, rotation);

                var instantiatedObject = instantiateHandle.WaitForCompletion();

                if (instantiateHandle.Status == AsyncOperationStatus.Succeeded && instantiatedObject != null)
                    return instantiatedObject;
                else
                {
                    Debug.LogError($"Failed to instantiate {instantiatedObject}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"Failed to load {loadedAsset}.");
                return null;
            }
        }
    }
}