using _Project.Scripts.Services.SceneLoader;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Menu
{
    public class UIMainMenuRootView : MonoBehaviour
    {
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonExit;
        
        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        private void Awake()
        {
            _buttonStart.onClick.AddListener(OnButtonStartClick);
            _buttonExit.onClick.AddListener(OnButtonExitClick);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        private void OnButtonStartClick()
        {
            _sceneLoader.LoadGameplay();
        }
    
        private void OnButtonExitClick()
        {
            Application.Quit();
        
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}