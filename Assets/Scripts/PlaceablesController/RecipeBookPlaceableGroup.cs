using System.Collections.Generic;
using ExtensionsPlus;
using UnityEngine;

namespace Placeables {
	
	public sealed class RecipeBookPlaceableGroup : PlaceableGroup {
		private List<Vector3> _playerSpawnPositions = new();
		private List<Vector3> _availableRecipeBookSpawnPositions = new();

		public Vector3 RecipeBookPosition { get; private set; } = Vector3.zero;
		
		public void ApplyProperties(List<Vector3> playerSpawnPoints) {
			_playerSpawnPositions = playerSpawnPoints;
			_availableRecipeBookSpawnPositions = AllPossibleSpawnPositions;
		}

		protected override bool Validate() {
			if (!base.Validate() || _playerSpawnPositions.IsEmpty() || _availableRecipeBookSpawnPositions.IsEmpty()) {
				Debug.LogError($"Failed to spawn RecipeBook. Player spawn positions or available recipe book spawn positions are empty!");
				return false;
			}
			return true;
		}

		protected override void Spawn() {
			float smallestMeasure = float.MaxValue;

			foreach (Vector3 recipeBookSpawnPoint in _availableRecipeBookSpawnPositions) {
				float sum = 0.0f, sumOfSquares = 0.0f;
				int playerCount = _playerSpawnPositions.Count;

				foreach (Vector3 playerPoint in _playerSpawnPositions) {
					float distance = Vector3.Distance(recipeBookSpawnPoint, playerPoint);
					sum += distance;
					sumOfSquares += distance * distance;
				}

				float measure;
				if (playerCount == 1) {
					measure = sum;
				} else {
					float mean = sum / playerCount;
					float variance = (sumOfSquares / playerCount) - (mean * mean);
					measure = variance;
				}

				if (measure < smallestMeasure) {
					smallestMeasure = measure;
					RecipeBookPosition = recipeBookSpawnPoint;
				}
			}

			Instantiate(Prefab, RecipeBookPosition, Quaternion.identity);
		}
	}
}