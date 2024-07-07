using Counters;
using UnityEngine;

namespace UI {
    public class StoveBurnFlashingBarUI : MonoBehaviour {

        private const string IS_FLASHING = "IsFlashing";
        
        [SerializeField] private StoveCounter stoveCounter;

        private Animator animator;
        private static readonly int IsFlashing = Animator.StringToHash(IS_FLASHING);

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

            animator.SetBool(IsFlashing, false);
        }

        private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
            float burnShowProgressAmount = .5f;
            bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

            animator.SetBool(IsFlashing, show);
        }
    }
}
