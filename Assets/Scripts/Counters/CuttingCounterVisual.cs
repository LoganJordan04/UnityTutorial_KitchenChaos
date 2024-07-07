using UnityEngine;

namespace Counters {
    public class CuttingCounterVisual : MonoBehaviour {

        private const string CUT = "Cut";

        [SerializeField] private CuttingCounter cuttingCounter;
    
        private Animator animator;
        private static readonly int Cut = Animator.StringToHash(CUT);

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            cuttingCounter.OnCut += CuttingCounter_OnCut;
        }

        private void CuttingCounter_OnCut(object sender, System.EventArgs e) {
            animator.SetTrigger(Cut);
        }
    }
}
