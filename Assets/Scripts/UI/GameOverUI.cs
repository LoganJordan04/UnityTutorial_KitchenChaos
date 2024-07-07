using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GameOverUI : MonoBehaviour {
        
        private const string PLAYER_PREFS_BEST = "Best";

        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
        [SerializeField] private TextMeshProUGUI bestText;
        [SerializeField] private Button restartButton;

        private int best;

        private void Awake() {
            restartButton.onClick.AddListener(() => {
                KitchenGameManager.Instance.Restart();
            });
            
            best = PlayerPrefs.GetInt(PLAYER_PREFS_BEST, 0);
        }

        private void Start() {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
            
            Hide();
        }

        private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
            if (KitchenGameManager.Instance.IsGameOver()) {
                Show();
                
                recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();

                if (DeliveryManager.Instance.GetSuccessfulRecipesAmount() > best) {
                    best = DeliveryManager.Instance.GetSuccessfulRecipesAmount();
                    PlayerPrefs.SetInt(PLAYER_PREFS_BEST, best);
                    PlayerPrefs.Save();
                }

                bestText.text = best.ToString();
            }
            else {
                Hide();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
            
            restartButton.Select();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}
