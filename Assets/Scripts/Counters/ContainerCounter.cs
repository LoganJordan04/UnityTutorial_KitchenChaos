using System;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class ContainerCounter : BaseCounter {

        public event EventHandler OnPlayerGrabbedObject;
    
        [SerializeField] private KitchenObjectSO kitchenObjectSo;
    
        public override void Interact(Player player) {
            // Player is not carrying anything
            if (!player.HasKitchenObject()) {
                KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
        
                OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
