using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Placeables {
	
	public sealed class PlayerPlaceableGroup : PlaceableGroup {
		[SerializeField, Min(1)] private int playerCount;

		public List<Vector3> PlayerSpawnPositions { get; private set; } = new();

		protected override bool Validate() {
			if (!base.Validate() || AllPossibleSpawnPositions.Count < playerCount) {
				Debug.LogError($"Failed to spawn players. Player count is more than available spawn points!");
				return false;
			}
			return true;
		}

		protected override void Spawn() {
			PlayerSpawnPositions = PickUniqueSpawnPositions();
			foreach (Vector3 playerPosition in PlayerSpawnPositions) {
				Instantiate(Prefab, playerPosition, Quaternion.identity);
			}
		}

		private List<Vector3> PickUniqueSpawnPositions() {
			List<Vector3> positions = new(AllPossibleSpawnPositions);
			Random random = new();
			for (int i = positions.Count - 1; i > 0; i--) {
				int j = random.Next(i + 1);
				(positions[i], positions[j]) = (positions[j], positions[i]);
			}

			List<Vector3> uniquePositions = new();
			for (int i = 0; i < playerCount && i < positions.Count; i++) {
				uniquePositions.Add(positions[i]);
			}
			return uniquePositions;
		}
	}
}