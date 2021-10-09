using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KAP.Pool
{
    public static class Pool
    {
        /// <summary>When you spawn a prefab from this class, the association between the spawned clone and the pool that spawned it will be stored here so it can quickly be despawned.</summary>
		public static Dictionary<GameObject, GameObjectPool> links = new Dictionary<GameObject, GameObjectPool>();

        #region Spawn
        /// <summary>This allows you to spawn a prefab with GameObject.</summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position) => Spawn(prefab, position, Quaternion.identity);

        /// <summary>This allows you to spawn a prefab with GameObject.</summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("Attempting to spawn a null prefab.");
                return null;
            }

            if(parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(rotation);
            }

            return Spawn(prefab, position, rotation, parent, false);

        }
        
        /// <summary>This allows you to spawn a prefab with GameObject.</summary>
        public static GameObject Spawn(GameObject prefab, Vector3 localPosition, Quaternion localRotation,
            Transform parent, bool worldPositionStays)
        {
            if(prefab == null)
            {
                Debug.LogError("Attempting to spawn a null prefab.");
                return null;
            }

            // Create a new Pool for this Prefab?
            if(GameObjectPool.TryFindPoolByPrefab(prefab, out GameObjectPool pool) == false)
            {
                pool = new GameObject("Pool (" + prefab.name + ")").AddComponent<GameObjectPool>();

                pool.Prefab = prefab;
            }

            // Try and spawn a clone from this pool
            GameObject clone = default(GameObject);

            if (pool.TrySpawn(ref clone, localPosition, localRotation, parent, worldPositionStays))
            {
                // Clone already registered?
                if (links.Remove(clone) == true)
                {
                    Debug.LogWarning("You're attempting to spawn a clone that hasn't been despawned. Make sure all your Spawn and Despawn calls match, you shouldn't be manually destroying them!", clone);
                }

                // Associate this clone with this pool
                links.Add(clone, pool);

                return clone;
            }

            return null;
        }
        #endregion

        #region Despawn
        public static void Despawn(GameObject clone)
        {
            if(clone == null)
            {
                Debug.LogWarning("You're attempting to despawn a null gameObject.", clone);
                return;
            }

            // Try and find the pool associated with this clone
            if(links.TryGetValue(clone, out GameObjectPool pool))
            {
                // Remove the association
                links.Remove(clone);

                pool.Despawn(clone);
            }
            else
            {
                Debug.LogWarning("You can't despawned an object because it wasn't spawned", clone);
            }
        }

        public static void DespawnAll()
        {
            int count = links.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject clone = links.Keys.ElementAt(links.Count - 1);
                GameObjectPool pool = links.Values.ElementAt(links.Count - 1);

                links.Remove(clone);
                pool.Despawn(clone);
            }
        }
        #endregion

        #region Detach
        /// <summary>This allows you to detach a clone via GameObject.
		/// A detached clone will act as a normal GameObject, requiring you to manually destroy or otherwise manage it.</summary>
        public static void Detach(GameObject clone)
        {
            if (clone != null) links.Remove(clone);
            else Debug.LogWarning("You're attempting to detach a null GameObject.", clone);
        }
        #endregion
    }
}