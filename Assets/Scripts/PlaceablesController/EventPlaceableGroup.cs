using System.Collections.Generic;
using ExtensionsPlus;
using UnityEngine;

namespace Placeables {
	
	public sealed class EventPlaceableGroup : PlaceableGroup {
		[SerializeField, Range(0, 4)] private int eventCount = 1;

		private Vector3 _recipeBookPosition;
		private List<Vector3> _availableEventSpawnPositions = new();
		
		public void ApplyProperties(Vector3 recipeBookPosition) {
			_recipeBookPosition = recipeBookPosition;
			_availableEventSpawnPositions = AllPossibleSpawnPositions;
		}

		protected override bool Validate() {
			if (!base.Validate() || _recipeBookPosition == Vector3.zero || _availableEventSpawnPositions.IsEmpty()) {
				Debug.LogError($"Failed to spawn Events. RecipeBook spawn position is Vector3.Zero or available event spawn positions are empty!");
				return false;
			}
			return true;
		}

		protected override void Spawn() {
			List<Vector3> farthestPoints = FindFarthestPoints(_recipeBookPosition, _availableEventSpawnPositions, eventCount);
			foreach (Vector3 point in farthestPoints) {
				Instantiate(Prefab, point, Quaternion.identity);
			}
		}
		
		// Because the event points will be ~3 per map and the available spawn points will be ~10 it's better to use Min-heap sorting,
		//  if in the future event count closely match the available spawn points then use Sort based sorting.
		private static List<Vector3> FindFarthestPoints(Vector3 referencePoint, List<Vector3> points, int count) {
			Comparer<(float Distance, Vector3 Point)> comparer = Comparer<(float Distance, Vector3 Point)>.Create((a, b) => a.Distance.CompareTo(b.Distance));
			SortedSet<(float Distance, Vector3 Point)> minHeap = new(comparer);
			foreach (Vector3 point in points) {
				float distance = Vector3.Distance(referencePoint, point);
				minHeap.Add((distance, point));
				if (minHeap.Count > count) {
					minHeap.Remove(minHeap.Min);
				}
			}

			List<Vector3> farthestPoints = new();
			foreach ((float _, Vector3 point) in minHeap) {
				farthestPoints.Add(point);
			}
			return farthestPoints;
		}
	}
}