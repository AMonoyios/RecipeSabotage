using UnityEngine;

namespace Placeables {
    
    public sealed class PlaceableManager : MonoBehaviour {
        [SerializeField] private PlayerPlaceableGroup playerPlaceableGroup;
        [SerializeField] private RecipeBookPlaceableGroup recipeBookPlaceableGroup;
        [SerializeField] private EventPlaceableGroup eventPlaceableGroup;
        [SerializeField] private FinishPlaceableGroup finishPlaceableGroup;

        private void Start() {
            // Order matters
            SpawnPlayerPlaceables();
            SpawnRecipeBookPlaceable();
            SpawnEventPlaceables();
            SpawnFinishPlaceable();
        }

        private void SpawnPlayerPlaceables() {
            playerPlaceableGroup.TrySpawn();
        }

        private void SpawnRecipeBookPlaceable() {
            recipeBookPlaceableGroup.ApplyProperties(playerPlaceableGroup.PlayerSpawnPositions);
            recipeBookPlaceableGroup.TrySpawn();
        }

        private void SpawnEventPlaceables() {
            eventPlaceableGroup.ApplyProperties(recipeBookPlaceableGroup.RecipeBookPosition);
            eventPlaceableGroup.TrySpawn();
        }

        private void SpawnFinishPlaceable() {
            finishPlaceableGroup.TrySpawn();
        }
    }
}