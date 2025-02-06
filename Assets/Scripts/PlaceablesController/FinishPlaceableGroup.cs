using UnityEngine;

namespace Placeables {
	
	public sealed class FinishPlaceableGroup : PlaceableGroup {
		
		protected override void Spawn() {
			Instantiate(Prefab, AllPossibleSpawnPositions[0], Quaternion.identity);
		}
	}
}