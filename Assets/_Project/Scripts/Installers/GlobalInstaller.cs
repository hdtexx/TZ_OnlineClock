using _Project.Scripts.Configs;
using _Project.Scripts.Services.InputService;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.Services.TimeService;
using _Project.Scripts.UI.Root;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private MainConfig mainConfig;
        
        public override void InstallBindings()
        {
            if (mainConfig)
            {
                Container.Bind<MainConfig>().FromInstance(mainConfig).AsSingle();
            }
            else
            {
                MainConfig defaultConfig = ScriptableObject.CreateInstance<MainConfig>();
                Container.Bind<MainConfig>().FromInstance(defaultConfig).AsSingle();
            }
            
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<IInputService>().To<InputService>().AsSingle();
            Container.Bind<ITimeService>().To<TimeService>().AsSingle();
            
            UIRootView uiRootPrefab = Resources.Load<UIRootView>("UIRoot");
            UIRootView uiRootInstance = Instantiate(uiRootPrefab);
            DontDestroyOnLoad(uiRootInstance);
            Container.Bind<UIRootView>().FromInstance(uiRootInstance).AsSingle();
        }
    }
}