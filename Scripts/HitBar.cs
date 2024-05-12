using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Attack
{
    public class HitBar : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private TMP_Text progressValue;
        [SerializeField] private TMP_Text progressState;

        public void SetProgress(AttackProgressData progress)
        {
            progressImage.fillAmount = progress.Time01;
            progressValue.text = $"{progress.Time}";
            progressState.text = progress.State.ToString();
        }
    }
}