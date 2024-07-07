using System;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class StoveCounter : BaseCounter, IHasProgress {

        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public class OnStateChangedEventArgs : EventArgs {
            public State state;
        }

        public enum State {
            Idle,
            Frying,
            Fried,
            Burned
        }

        [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

        private State state;
        private float fryingTimer;
        private float burningTimer;
        private FryingRecipeSO fryingRecipeSO;
        private BurningRecipeSO burningRecipeSO;

        private void Start() {
            state = State.Idle;
        }

        private void Update() {
            if (HasKitchenObject()) {
                switch (state) {
                    case State.Idle:
                        break;
                
                    case State.Frying:
                        fryingTimer += Time.deltaTime;
                    
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                        });
                    
                        if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
                            // Fried
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        
                            state = State.Fried;
                            burningTimer = 0f;
                            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());
                        
                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                                state = state
                            });
                        }
                        break;
                
                    case State.Fried:
                        burningTimer += Time.deltaTime;
                    
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                        });
                    
                        if (burningTimer > burningRecipeSO.burningTimerMax) {
                            // Burned
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        
                            state = State.Burned;
                        
                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                                state = state
                            });
                        
                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                                progressNormalized = 0f
                            });
                        }
                        break;
                
                    case State.Burned:
                        break;
                }
            }
        }

        public override void Interact(Player player) {
            if (!HasKitchenObject()) {
                // There is no kitchenObject here
                if (player.HasKitchenObject()) {
                    // Player is carrying something
                    if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo())) {
                        // Player carrying something that can be fried
                        player.GetKitchenObject().SetKitchenObjectParent(this);
                    
                        fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

                        state = State.Frying;
                        fryingTimer = 0f;
                    
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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
                            
                            state = State.Idle;
                
                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                                state = state
                            });
                
                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                                progressNormalized = 0f
                            });
                        }
                    }
                }
                else {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);

                    state = State.Idle;
                
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });
                
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
            }
        }
    
        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo) {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
            return fryingRecipeSO != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo) {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
            if (fryingRecipeSO != null) {
                return fryingRecipeSO.output;
            }

            return null;
        }

        private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo) {
            foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
                if (fryingRecipeSO.input == inputKitchenObjectSo) {
                    return fryingRecipeSO;
                }
            }
        
            return null;
        }
    
        private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo) {
            foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
                if (burningRecipeSO.input == inputKitchenObjectSo) {
                    return burningRecipeSO;
                }
            }
        
            return null;
        }
        
        public bool IsFried() {
            return state == State.Fried;
        }
    }
}
