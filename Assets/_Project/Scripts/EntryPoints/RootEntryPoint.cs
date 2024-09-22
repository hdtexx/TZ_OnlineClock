using _Project.Scripts.Services.SceneLoader;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.EntryPoints
{
    public class RootEntryPoint
    {
        private static RootEntryPoint _instance;
        private readonly ISceneLoader _sceneLoader;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EntryPoint()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            _instance = ProjectContext.Instance.Container.Instantiate<RootEntryPoint>();
            _instance._sceneLoader.LoadMainMenu();
        }
        
        [Inject]
        public RootEntryPoint(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
    }
}