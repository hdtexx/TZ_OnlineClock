using System;
using _Project.Scripts.EntryPoints;
using _Project.Scripts.UI.Root;
using _Project.Scripts.Services.TimeService;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        private readonly UIRootView _uiRoot;
        private readonly ITimeService _timeService;
        private int _taskAmount;

        [Inject]
        public SceneLoader(UIRootView uiRoot, ITimeService timeService)
        {
            _uiRoot = uiRoot;
            _timeService = timeService;
        }

        public void LoadMainMenu()
        {
            LoadAndStartMenuAsync().Forget();
        }

        public void LoadGameplay()
        {
            LoadAndStartGameplayAsync().Forget();
        }

        private async UniTaskVoid LoadAndStartMenuAsync()
        {
            _uiRoot.ResetProgress();
            _uiRoot.ShowLoadingScreen();
            _taskAmount = 2;

            await LoadTask(() => SceneManager.LoadSceneAsync(Scenes.ROOT).ToUniTask(), 1);
            await LoadTask(() => SceneManager.LoadSceneAsync(Scenes.MAIN_MENU).ToUniTask(), 2);

            MainMenuEntryPoint mainMenuEntryPoint = Object.FindObjectOfType<MainMenuEntryPoint>();
            mainMenuEntryPoint.Init(_uiRoot);
            
            await UniTask.Delay(300);
            _uiRoot.HideLoadingScreen();
        }

        private async UniTaskVoid LoadAndStartGameplayAsync()
        {
            _uiRoot.ResetProgress();
            _uiRoot.ShowLoadingScreen();
            _taskAmount = 3;

            await LoadTask(() => SceneManager.LoadSceneAsync(Scenes.ROOT).ToUniTask(), 1);
            await LoadTask(_timeService.PreloadTimeAsync, 2);
            await LoadTask(() => SceneManager.LoadSceneAsync(Scenes.GAMEPLAY).ToUniTask(), 3);
            await UniTask.Yield();
            
            GameplayEntryPoint gameplayEntryPoint = Object.FindObjectOfType<GameplayEntryPoint>();
            gameplayEntryPoint.Init(_uiRoot);
            
            await UniTask.Delay(300);
            _uiRoot.HideLoadingScreen();
        }

        private async UniTask LoadTask(Func<UniTask> task, int taskIndex)
        {
            await task();
            _uiRoot.SetProgress(taskIndex, _taskAmount);
        }
    }
}