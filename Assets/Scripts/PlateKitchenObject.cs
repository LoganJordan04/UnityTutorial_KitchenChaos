using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO KitchenObjectSO;
    }
    
    [SerializeField] private List<KitchenObjectSO> validKitchenSOList;
    
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }
    
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenSOList.Contains(kitchenObjectSO)) {
            // Not a valid ingredient
            return false;
        }
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            // Already has this type
            return false;
        }

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            KitchenObjectSO = kitchenObjectSO
        });
        return true;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}
