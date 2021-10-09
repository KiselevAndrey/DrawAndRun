using System.Collections.Generic;
using UnityEngine;

namespace KAP.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        [Header("Lists")]
        /// <summary>This stores all spawned clones in a list. This is used when Recycle is enabled, because knowing the spawn order must be known. This list is also used during serialization.</summary>
		[SerializeField] private List<GameObject> spawnedClones = new List<GameObject>();
        /// <summary>All the currently despawned prefab instances.</summary>
		[SerializeField] private List<GameObject> despawnedClones = new List<GameObject>();

        #region Properties
        /// <summary>The prefab this pool controls.</summary>
        public GameObject Prefab
        {
            get
            { 
                return _prefab; 
            }
            set 
            { 
                if (value != _prefab) 
                { 
                    UnregisterPrefab(); 
                    _prefab = value; 
                    RegisterPrefab(); 
                } 
            } 
        }

        public int Total { get => spawnedClones.Count + despawnedClones.Count; }
        #endregion

        private GameObject _prefab;

        #region Static
        private static Dictionary<GameObject, GameObjectPool> _prefabMap = new Dictionary<GameObject, GameObjectPool>();

        /// <summary>Find the pool responsible for handling the specified prefab.</summary>
        public static bool TryFindPoolByPrefab(GameObject prefab, out GameObjectPool foundPool)
        {
            return _prefabMap.TryGetValue(prefab, out foundPool);
        }
        #endregion

        private void OnDestroy()
        {
            // If OnDestroy is called then the scene is likely changing, so we detach the spawned prefabs from the global links dictionary to prevent issues.
            foreach (var clone in spawnedClones)
                Pool.Detach(clone);

            _prefabMap.Remove(Prefab);
        }

        #region TrySpawn
        /// <summary>This will either spawn a previously despawned/preloaded clone, recycle one, create a new one, or return null.</summary>
        public bool TrySpawn(ref GameObject clone, Vector3 localPosition, Quaternion localRotation, 
            Transform parent, bool worldPositionStays)
        {
            if (_prefab == null)
                return false;

            // Spawn a previously despawned/preloaded clone?
            for (int i = despawnedClones.Count - 1; i >= 0; i--)
            {
                clone = despawnedClones[i];

                despawnedClones.RemoveAt(i);

                if(clone != null)
                {
                    SpawnClone(clone, localPosition, localRotation, parent, worldPositionStays);

                    return true;
                }    
            }

            // Make a new clone
            clone = CreateClone(localPosition, localRotation, parent, worldPositionStays);

            spawnedClones.Add(clone);
            clone.SetActive(true);

            return true;
        }

        #region Spawn
        private void SpawnClone(GameObject clone, Vector3 localPosition, Quaternion localRotation,
            Transform parent, bool worldPositionStays)
        {
            spawnedClones.Add(clone);

            Transform cloneTransform = clone.transform;
            cloneTransform.SetParent(null, false);
            cloneTransform.localPosition = localPosition;
            cloneTransform.localRotation = localRotation;
            cloneTransform.SetParent(parent, worldPositionStays);

            if (parent == null)
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(clone, UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            clone.SetActive(true);
        }
        #endregion

        #region Create
        private GameObject CreateClone(Vector3 localPosition, Quaternion localRotation, Transform parent, bool worldPositionStays)
        {
            if (worldPositionStays)
                return Instantiate(Prefab, parent, true);
            else
                return Instantiate(Prefab, localPosition, localRotation, parent);
        }
        #endregion
        #endregion

        #region Despawn
        public void Despawn(GameObject clone)
        {
            if(clone == null)
            {
                Debug.LogWarning("You're attempting to despawn a null gameObject", this);
                return;
            }

            TryDespawn(clone);
        }

        private void TryDespawn(GameObject clone)
        {
            if(spawnedClones.Remove(clone) == false)
            {
                Debug.LogWarning("You're attempting to despawn a GameObject that wasn't spawned from this pool, make sure your Spawn and Despawn calls match.", clone);
                return;
            }

            despawnedClones.Add(clone);
            clone.SetActive(false);
            clone.transform.SetParent(transform);
        }
        #endregion

        #region Un/Register Prefab
        private void UnregisterPrefab()
        {
            // Skip actually null prefabs, but allow destroyed prefabs
            if (Equals(_prefab, null)) 
                return;

            if (_prefabMap.TryGetValue(_prefab, out GameObjectPool existingPool) && existingPool == this)
                _prefabMap.Remove(_prefab);
        }

        private void RegisterPrefab()
        {
            if(_prefab != null)
            {
                if (_prefabMap.TryGetValue(_prefab, out GameObjectPool existingPool))
                    Debug.LogWarning("You have multiple pools managing the same prefab (" + _prefab.name + ").", existingPool);
                else
                    _prefabMap.Add(_prefab, this);
            }
        }
        #endregion
    }
}
