using System.Collections.Generic;
using ExtensionsPlus;
using UnityEngine;

namespace Placeables {

	public abstract class PlaceableGroup : MonoBehaviour {
		[SerializeField] private GameObject prefab;
		[SerializeField] private Transform[] spawnPointTransforms;

		private readonly List<Vector3> _cachedAllPossibleSpawnPositions = new();
		
		protected GameObject Prefab => prefab;

		protected List<Vector3> AllPossibleSpawnPositions {
			get {
				if (_cachedAllPossibleSpawnPositions.IsEmpty()) {
					foreach (Transform spawnPointTransform in spawnPointTransforms) {
						_cachedAllPossibleSpawnPositions.Add(spawnPointTransform.position);
					}
				}

				return _cachedAllPossibleSpawnPositions;
			}
		}

		public void TrySpawn() {
			if (Validate()) {
				Spawn();
				return;
			}
			
			Debug.LogError("Failed to validate Spawn.");
		}

		protected virtual bool Validate() {
			return prefab != null && !spawnPointTransforms.IsNullOrEmpty();
		}

		protected abstract void Spawn();
	}
}