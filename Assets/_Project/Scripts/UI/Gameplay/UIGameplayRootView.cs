using _Project.Scripts.Services.InputService;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.Gameplay.Clock;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Gameplay
{
    public class UIGameplayRootView : MonoBehaviour
    {
        [SerializeField] private Button _buttonMainMenu;
        [SerializeField] private Button _buttonEdit;
        [SerializeField] private Button _buttonSyncTime;
        [SerializeField] private GameObject _editPanel;
        [SerializeField] private TMP_Text _buttonEditTimeTMP;
        [SerializeField] private InputField _inputFieldHour;
        [SerializeField] private InputField _inputFieldMin;
        [SerializeField] private InputField _inputFieldSec;
        [SerializeField] private Button _buttonSet;

        private ISceneLoader _sceneLoader;
        private IInputService _inputService;
        private ClockView _clockView;
        private ClockController _clockController;
        private bool _editMode;

        public void Init(ISceneLoader sceneLoader,
            IInputService inputService,
            ClockView clockView,
            ClockController clockController)
        {
            _sceneLoader = sceneLoader;
            _inputService = inputService;
            _clockView = clockView;
            _clockController = clockController;
        }

        private void Awake()
        {
            _buttonMainMenu?.onClick.AddListener(OnButtonMenuClick);
            _buttonEdit?.onClick.AddListener(OnButtonEditClick);
            _buttonSyncTime?.onClick.AddListener(OnButtonSyncTimeClick);
            _buttonSet?.onClick.AddListener(OnButtonSaveClick);

            SetupInputField(_inputFieldHour, 0, 23);
            SetupInputField(_inputFieldMin, 0, 59);
            SetupInputField(_inputFieldSec, 0, 59);
        }

        private void Update()
        {
            if (_inputService.EscPressed())
            {
                _sceneLoader.LoadMainMenu();
            }
        }

        private void OnDestroy()
        {
            _buttonMainMenu?.onClick.RemoveListener(OnButtonMenuClick);
            _buttonEdit?.onClick.RemoveListener(OnButtonEditClick);
            _buttonSyncTime?.onClick.RemoveListener(OnButtonSyncTimeClick);
        }

        private void SetupInputField(InputField inputField, int minValue, int maxValue)
        {
            inputField.contentType = InputField.ContentType.IntegerNumber;
            inputField.onValueChanged.AddListener(value =>
            {
                ValidateInput(inputField, value, minValue, maxValue);
            });

            inputField.onEndEdit.AddListener(value =>
            {
                FormatInputOnEndEdit(inputField, value, minValue, maxValue);
            });
        }

        private void ValidateInput(InputField inputField, string value, int minValue, int maxValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            int caretPosition = inputField.caretPosition;

            if (value.Length > 2)
            {
                value = value.Substring(0, 2);
                inputField.text = value;
                inputField.caretPosition = Mathf.Clamp(caretPosition, 0, 2);
            }

            if (int.TryParse(value, out int intValue))
            {
                if (intValue > maxValue)
                {
                    inputField.text = maxValue.ToString("D2");
                }
                else if (intValue < minValue)
                {
                    inputField.text = minValue.ToString("D2");
                }
            }
            else
            {
                inputField.text = inputField.text.Substring(0, Mathf.Max(0, inputField.text.Length - 1));
            }

            inputField.caretPosition = Mathf.Clamp(caretPosition, 0, inputField.text.Length);
        }
        
        private void FormatInputOnEndEdit(InputField inputField, string value, int minValue, int maxValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                inputField.text = minValue.ToString("D2");
                return;
            }

            if (int.TryParse(value, out int intValue))
            {
                intValue = Mathf.Clamp(intValue, minValue, maxValue);
                inputField.text = intValue.ToString("D2");
            }
            else
            {
                inputField.text = minValue.ToString("D2");
            }
        }

        private void OnButtonMenuClick()
        {
            _sceneLoader.LoadMainMenu();
        }

        private void OnButtonEditClick()
        {
            _editMode = !_editMode;
            _buttonEditTimeTMP.text = _editMode ? "Exit edit" : "Edit";
            _editPanel.gameObject.SetActive(_editMode);
            _clockView.SetIsEditMode(_editMode);

            _clockController.ToggleEditMode();
            
            _inputFieldHour.text = "00";
            _inputFieldMin.text = "00";
            _inputFieldSec.text = "00";
        }

        private void OnButtonSyncTimeClick()
        {
            _clockController.SyncTimeWithServer().Forget();
        }

        private void OnButtonSaveClick()
        {
            int hour = int.Parse(_inputFieldHour.text);
            int minute = int.Parse(_inputFieldMin.text);
            int second = int.Parse(_inputFieldSec.text);

            _clockController.SetTimeFromInputs(hour, minute, second);
        }
    }
}
