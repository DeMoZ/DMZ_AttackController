using UnityEngine;
using Zenject;

namespace Attack
{
    public class AttackInstaller : MonoInstaller
    {
        [SerializeField] private AttackConfig attackConfig;
            
        public override void InstallBindings()
        {
            var playerData = new AttackPlayerData();
            playerData.AttackSequenceState.Value = AttackState.Idle;

            var attackRepository = new AttackRepository();
            attackRepository.Init(attackConfig);
            Container.Bind<IAttackRepository>().FromInstance(attackRepository).AsSingle();
            
            Container.Bind<AttackPlayerData>().FromInstance(playerData).AsSingle();
            Container.Bind<InputBus>().FromInstance(new InputBus()).AsSingle();
            Container.BindInterfacesTo<AttackController>().AsSingle();
        }
    }
}