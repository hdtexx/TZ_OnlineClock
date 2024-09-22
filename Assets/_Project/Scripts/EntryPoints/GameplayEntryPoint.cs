using _Project.Scripts.Gameplay.Clock;
using _Project.Scripts.Services.InputService;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Gameplay;
using _Project.Scripts.UI.Root;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.EntryPoints
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootView _uIGameplayRootViewPrefab;
        
        private ClockView _clockView;
        private ClockController _clockController;
        private ISceneLoader _sceneLoader;
        private IInputService _inputService;

        [Inject]
        private void Construct(ISceneLoader sceneLoader, 
            IInputService inputService,
            ClockView clockView, 
            ClockController clockController)
        {
            _sceneLoader = sceneLoader;
            _inputService = inputService;
            _clockView = clockView;
            _clockController = clockController;
        }
        
        public void Init(UIRootView uiRootView)
        {
            UIGameplayRootView sceneUI = ProjectContext.Instance.Container
                .InstantiatePrefabForComponent<UIGameplayRootView>(_uIGameplayRootViewPrefab);
            
            sceneUI.Init(_sceneLoader, _inputService, _clockView, _clockController);
            
            uiRootView.AttachSceneUI(sceneUI.gameObject);
        }
    }
}