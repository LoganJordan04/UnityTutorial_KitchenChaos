using System;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class CuttingCounter : BaseCounter, IHasProgress {

        public static event EventHandler OnAnyCut;

        public new static void ResetStaticData() {
            OnAnyCut = null;
        }
        
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler OnCut;

        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

        private int cuttingProgress;

        public override void Interact(Player player) {
            if (!HasKitchenObject()) {
                // There is no kitchenObject here
                if (player.HasKitchenObject()) {
                    // Player is carrying something
                    if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo())) {
                        // Player carrying something that can be cut
                        player.GetKitchenObject().SetKitchenObjectParent(this);
                        cuttingProgress = 0;

                        CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = (float)cuttingProgress / cuttingRecipeSo.cuttingProgressMax
                        });
                    }
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
                }
                else {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);
                
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0
                    });
                }
                
            }
        }

        public override void InteractAlternate(Player player) {
            // There is a kitchenObject here and it can be cut
            if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSo())) {
                cuttingProgress++;

                OnCut?.Invoke(this, EventArgs.Empty);
                OnAnyCut?.Invoke(this, EventArgs.Empty);
            
                CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());
            
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSo.cuttingProgressMax
                });

                if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax) {
                    KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSo());
        
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                }
            }
        }

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo) {
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo) {
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
            if (cuttingRecipeSo != null) {
                return cuttingRecipeSo.output;
            }

            return null;
        }

        private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo) {
            foreach (CuttingRecipeSO cuttingRecipeSo in cuttingRecipeSOArray) {
                if (cuttingRecipeSo.input == inputKitchenObjectSo) {
                    return cuttingRecipeSo;
                }
            }
        
            return null;
        }
    }
}
