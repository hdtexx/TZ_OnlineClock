using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Root
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _uiSceneContainer;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _progressView;
        [SerializeField] private Image _progressBar;
        
        private void Awake()
        {
            HideLoadingScreen();
        }
        
        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();
            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }
        
        public void ClearSceneUI()
        {
            int childCount = _uiSceneContainer.childCount;
            
            for (int i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }
        
        public void ShowLoadingScreen()
        {
            _loadingScreen.gameObject.SetActive(true);
        }

        public void HideLoadingScreen()
        {
            _loadingScreen.gameObject.SetActive(false);
        }
        
        public void SetProgress(int currentIndex, int maxAmount)
        {
            float progress = maxAmount > 0f ? currentIndex / (float)maxAmount : 0f;
            
            if (_progressBar)
            {
                _progressBar.fillAmount = Mathf.Clamp01(progress);
            }
        }

        public void ResetProgress()
        {
            if (_progressBar)
            {
                _progressBar.fillAmount = 0f;
            }
        }
    }
}