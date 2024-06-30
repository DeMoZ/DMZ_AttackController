using System;
using UnityEngine;
using Zenject;

namespace Attack
{
    public class SequencesVisualiser : MonoBehaviour
    {
        [SerializeField] private HitBar anyBar;
        [SerializeField] private HitBar lBar;
        [SerializeField] private HitBar rBar;
        [SerializeField] private HitBar uBar;
        [SerializeField] private HitBar dBar;
        [SerializeField] private HitBar sequenceBar;
        [SerializeField] private HitBar failBar;

        private AttackPlayerData _playerData;

        [Inject]
        public void Construct(AttackPlayerData playerData)
        {
            _playerData = playerData;
            _playerData.AttackProgress.Subscribe(OnProgress);
        }

        private void OnDestroy()
        {
            _playerData.AttackProgress.Subscribe(OnProgress);
        }

        private void OnProgress(AttackProgressData progress)
        {
            anyBar.SetProgress(progress);
            
            switch (progress.Direction)
            {
                case AttackDirection.L:
                    lBar.SetProgress(progress);
                    break;
                case AttackDirection.R:
                    rBar.SetProgress(progress);
                    break;
                case AttackDirection.D:
                    dBar.SetProgress(progress);
                    break;
                case AttackDirection.U:
                    uBar.SetProgress(progress);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (progress.State)
            {
                case AttackPhase.SequenceReady:
                    sequenceBar.SetProgress(progress);
                    break;
                case AttackPhase.SequenceFail:
                    failBar.SetProgress(progress);
                    break;
            }
            // progressImage.fillAmount = progress.Time01;
            // progressValue.text = $"{progress.Time}";
            // progressState.text = progress.State.ToString();
        }
    }
}