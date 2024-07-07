using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class ClearCounter : BaseCounter {

        [SerializeField] private KitchenObjectSO kitchenObjectSo;
    
        public override void Interact(Player player) {
            if (!HasKitchenObject()) {
                // There is no kitchenObject here
                if (player.HasKitchenObject()) {
                    // Player is carrying something
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
            else {
                // There is a kitchenObject here
                if (player.HasKitchenObject()) {
                    // Player is carrying something
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                        // Player is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo())) {
                            GetKitchenObject().DestroySelf();
                        }
                    }
                    else {
                        // Player is not carrying Plate but something else
                        if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                            // Counter is holding a Plate
                            if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSo())) {
                                player.GetKitchenObject().DestroySelf();
                            }
                        }
                    }
                }
                else {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);
                }
            }
        }
    }
}
