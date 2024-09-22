using _Project.Scripts.Configs;
using _Project.Scripts.Gameplay.Clock;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _clockPrefab;

        public override void InstallBindings()
        {
            ClockView clockView = Container.InstantiatePrefabForComponent<ClockView>(_clockPrefab);
            MainConfig mainConfig = Container.Resolve<MainConfig>();
            clockView.transform.position = mainConfig.ClockPosition;

            Container.Bind<ClockView>().FromInstance(clockView).AsSingle();
            Container.Bind<ClockAnimationService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClockSyncService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClockController>().AsSingle();
        }
    }
}