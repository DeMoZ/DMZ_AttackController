using System;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

namespace Attack
{
    public interface IAttackController : IDisposable
    {
    }

    public class AttackController : IAttackController
    {
        private readonly InputBus _inputBus;
        private readonly AttackPlayerData _attackPlayerData;
        private readonly IAttackRepository _attackRepository;

        private CancellationTokenSource _attackTokenSource;
        private bool _isFailed;
        private AttackDirection _newDirection;

        public AttackController(InputBus inputBus,
            AttackPlayerData attackPlayerData,
            IAttackRepository attackRepository)
        {
            _inputBus = inputBus;
            _attackPlayerData = attackPlayerData;
            _attackRepository = attackRepository;
            _inputBus.AttackClicked += OnAttackClicked;
            _inputBus.BrakeClicked += OnBrake;
        }

        public void Dispose()
        {
            _inputBus.AttackClicked -= OnAttackClicked;
            _inputBus.BrakeClicked -= OnBrake;
            _attackTokenSource?.Cancel();
        }

        private void OnAttackClicked(AttackDirection attackDirection)
        {
            if (_isFailed)
                return;

            Debug.Log($"player fightSequenceState is {_attackPlayerData.AttackSequenceState.Value}");

            _newDirection = attackDirection;

            switch (_attackPlayerData.AttackSequenceState.Value)
            {
                case AttackState.None:
                case AttackState.SequenceFail:
                    Debug.Log("Attacking not available");
                    break;
                case AttackState.Idle:
                    EvaluateSequence();
                    break;
                case AttackState.Attack:
                    MarkFail();
                    break;
                case AttackState.SequenceReady:
                    _attackTokenSource.Cancel();
                    EvaluateSequence();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MarkFail()
        {
            _isFailed = true;
            Debug.Log($"_isFailed: {_isFailed}");
        }

        private void OnBrake()
        {
            throw new NotImplementedException();
        }

        private async Task AttackAsync()
        {
            var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceCode);
            try
            {
                await SequenceAsync(AttackState.Attack, time, _attackTokenSource.Token);

                if (_attackTokenSource.IsCancellationRequested)
                    return;

                if (_isFailed)
                {
                    time = _attackRepository.GetFailTime(_attackPlayerData.CurrentSequenceCode);
                    await SequenceAsync(AttackState.SequenceFail, time, _attackTokenSource.Token, onFinal: SetIdle);
                    _isFailed = false;
                }
                else
                {
                    time = _attackRepository.GetSequenceTime(_attackPlayerData.CurrentSequenceCode);
                    await SequenceAsync(AttackState.SequenceReady, time, _attackTokenSource.Token, onEnd: SetIdle);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task SequenceAsync(AttackState state, float time, CancellationToken token,
            Action onCancel = null, Action onEnd = null, Action onFinal = null)
        {
            _attackPlayerData.AttackSequenceState.Value = state;
            try
            {
                await TimerProcessAsync(time, token,
                    progress =>
                    {
                        var progress01 = Mathf.Clamp01(progress / time);
                        _attackPlayerData.AttackProgress.Value = new AttackProgressData(state, _newDirection, progress,
                            progress01);
                    }, state.ToString());

                if (token.IsCancellationRequested)
                    return;

                onEnd?.Invoke();
            }
            catch (TaskCanceledException)
            {
                if (onCancel != null)
                {
                    Debug.Log($"Canceled {state}");
                    onCancel?.Invoke();
                }
            }
            finally
            {
                onFinal?.Invoke();
            }
        }

        private void SetIdle()
        {
            _attackPlayerData.CurrentSequenceCode = default;
            _attackPlayerData.CurrentSequenceElement = default;
            _attackPlayerData.AttackSequenceState.Value = AttackState.Idle;
            _attackPlayerData.AttackProgress.Value = new AttackProgressData(AttackState.Idle, _newDirection, 0, 0);
        }

        private async void SetAttack()
        {
            _attackTokenSource = new CancellationTokenSource();
            await AttackAsync();
        }

        private void EvaluateSequence()
        {
            var currentCode = _attackPlayerData.CurrentSequenceCode;
            var newDirection = _newDirection.ToString();
            var newCode = currentCode + newDirection;

            if (_attackRepository.TryGetSequence(newCode, out var element))
            {
                Debug.Log($"EvaluateSequence success {_attackPlayerData.CurrentSequenceCode} > {newCode}");
                _attackPlayerData.CurrentSequenceCode = newCode;
                _attackPlayerData.CurrentSequenceElement = element;
                SetAttack();
            }
            else if (_attackRepository.TryGetSequence(newDirection, out element))
            {
                Debug.Log($"EvaluateSequence success as new {_attackPlayerData.CurrentSequenceCode} > {newDirection}"
                    .Yellow());
                _attackPlayerData.CurrentSequenceCode = newDirection;
                _attackPlayerData.CurrentSequenceElement = element;
                SetAttack();
            }
            else
            {
                Debug.Log($"EvaluateSequence fail  {_attackPlayerData.CurrentSequenceCode} > {newCode}");
                SetIdle();
            }
        }

        private async Task TimerProcessAsync(float time, CancellationToken cancellationToken, Action<float> progress,
            string description = null)
        {
            float elapsedTime = 0;

            try
            {
                Debug.Log($"Timer started. {time} {description}");

                while (elapsedTime < time)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        OnTimerCancel();
                        return;
                    }

                    progress?.Invoke(elapsedTime);
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                    elapsedTime += 0.1f;
                }

                progress?.Invoke(time);
                Debug.Log($"Timer elapsed. {time} {description}".Green());
            }
            catch (TaskCanceledException)
            {
                OnTimerCancel();
            }

            return;

            void OnTimerCancel()
            {
                Debug.Log($"Timer cancelled.  {elapsedTime}/{time} {description}".Yellow());
            }
        }
    }
}