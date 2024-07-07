using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class GameStartCountdownUI : MonoBehaviour {

        private const string NUMBER_POPUP = "NumberPopup";

        [SerializeField] private TextMeshProUGUI countdownText;

        private Animator animator;
        private int previousCountdownNumber;
        private static readonly int NumberPopup = Animator.StringToHash(NUMBER_POPUP);

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

            Hide();
        }

        private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
            if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
                Show();
            }
            else {
                Hide();
            }
        }

        private void Update() {
            int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
            countdownText.text = countdownNumber.ToString();

            if (previousCountdownNumber != countdownNumber) {
                previousCountdownNumber = countdownNumber;
                animator.SetTrigger(NumberPopup);
                SoundManager.Instance.PlayCountdownSound();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}
