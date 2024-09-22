using System;
using _Project.Scripts.Services.InputService;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Gameplay.Clock
{
    public class ClockController : ITickable, IDisposable
    {
        private readonly ClockView _clockView;
        private readonly IInputService _inputService;
        private readonly ClockAnimationService _clockAnimationService;
        private readonly ClockSyncService _clockSyncService;
        private bool _isEditModeActive = false;
        private Transform _selectedHand;
        private DateTime _currentEditTime;
        private Camera _camera;
        private float _previousHourAngle;
        private int _fullRotations = 0;
        private int _initialHours;
        private bool _handsHighlightReseted;

        public ClockController(
            ClockView clockView,
            IInputService inputService,
            ClockAnimationService clockAnimationService,
            ClockSyncService clockSyncService)
        {
            _clockView = clockView;
            _inputService = inputService;
            _clockAnimationService = clockAnimationService;
            _clockSyncService = clockSyncService;
            _camera = Camera.main;

            _clockSyncService.OnTimeUpdated += OnTimeUpdated;
        }

        private void OnTimeUpdated(DateTime newTime)
        {
            if (!_isEditModeActive)
            {
                UpdateClockDisplay(newTime);
            }
        }

        private void UpdateClockDisplay(DateTime time)
        {
            _clockAnimationService.UpdateClockHandsPositions(time);
            UpdateTimeText(time);
        }

        public void Tick()
        {
            if (_isEditModeActive)
            {
                HandleEditModeInput();
            }
        }

        private void HandleEditModeInput()
        {
            if (_inputService.IsLeftMouseButtonPressed())
            {
                SelectHand();
            }

            if (_selectedHand != null && _inputService.IsLeftMouseButtonHeld())
            {
                RotateSelectedHand();
            }
            else if (!_inputService.IsLeftMouseButtonHeld())
            {
                _selectedHand = null;
                
                if (_handsHighlightReseted == false)
                {
                    _handsHighlightReseted = true;
                    _clockView.ResetHightlighting();
                }
            }
        }

        public void ToggleEditMode()
        {
            _isEditModeActive = !_isEditModeActive;

            if (_isEditModeActive)
            {
                StartEditing();
            }
            else
            {
                FinishEditing();
            }
        }

        private void StartEditing()
        {
            _clockAnimationService.PauseAnimations();
            _currentEditTime = _clockSyncService.CurrentTime;
            _clockView.SetIsEditMode(true);
            _clockSyncService.SetEditMode(true);
            _fullRotations = 0;
            _previousHourAngle = NormalizeAngle(_clockView.HourHand.localEulerAngles.z);
            _initialHours = _currentEditTime.Hour;
        }

        private void FinishEditing()
        {
            _selectedHand = null;
            UpdateTimeFromHandPositions();
            _clockSyncService.SetTime(_currentEditTime);
            _clockAnimationService.UpdateClockHandsPositions(_currentEditTime);
            _clockAnimationService.StartAnimations();
            _clockView.SetIsEditMode(false);
            _clockSyncService.SetEditMode(false);
        }

        private void SelectHand()
        {
            Ray ray = _camera.ScreenPointToRay(_inputService.GetMousePosition());
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                if (hit.collider == _clockView.GetHandCollider(_clockView.HourHand))
                {
                    _selectedHand = _clockView.HourHand;
                    _clockView.HighlightHourHand(true);
                }
                else if (hit.collider == _clockView.GetHandCollider(_clockView.MinuteHand))
                {
                    _selectedHand = _clockView.MinuteHand;
                    _clockView.HighlightMinuteHand(true);
                }
                else if (hit.collider == _clockView.GetHandCollider(_clockView.SecondHand))
                {
                    _selectedHand = _clockView.SecondHand;
                    _clockView.HighlightSecondHand(true);
                }

                _handsHighlightReseted = false;
            }
        }

        private void RotateSelectedHand()
        {
            if (_selectedHand == null) return;

            Vector3 clockCenter = _camera.WorldToScreenPoint(_clockView.transform.position);
            Vector3 mousePosition = _inputService.GetMousePosition();
            Vector2 direction = mousePosition - clockCenter;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle -= 90;

            if (angle < 0)
            {
                angle += 360;
            }

            _selectedHand.localRotation = Quaternion.Euler(0, 0, angle);

            if (_selectedHand == _clockView.HourHand)
            {
                float currentAngle = NormalizeAngle(angle);
                bool isClockwise = (currentAngle - _previousHourAngle + 360) % 360 < 180;

                if (isClockwise && _previousHourAngle > 270 && currentAngle < 90)
                {
                    _fullRotations++;
                }
                else if (!isClockwise && _previousHourAngle < 90 && currentAngle > 270)
                {
                    _fullRotations--;
                }

                _previousHourAngle = currentAngle;
            }

            UpdateTimeFromHandPositions();
        }

        private void UpdateTimeFromHandPositions()
        {
            float hourAngle = NormalizeAngle(_clockView.HourHand.localEulerAngles.z);
            float minuteAngle = NormalizeAngle(_clockView.MinuteHand.localEulerAngles.z);
            float secondAngle = NormalizeAngle(_clockView.SecondHand.localEulerAngles.z);

            int hoursDifference = Mathf.FloorToInt(hourAngle / 30f) - (_initialHours % 12);
            int totalHours = _initialHours + hoursDifference + (_fullRotations * 12);
            int minutes = Mathf.FloorToInt(minuteAngle / 6f);
            int seconds = Mathf.FloorToInt(secondAngle / 6f);

            totalHours = (totalHours + 24) % 24;

            try
            {
                _currentEditTime = new DateTime(
                    _currentEditTime.Year,
                    _currentEditTime.Month,
                    _currentEditTime.Day,
                    totalHours,
                    minutes,
                    seconds
                );
                UpdateTimeText(_currentEditTime);
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogWarning($"Неверные значения: {totalHours}:{minutes}:{seconds}. Пропуск обновления");
            }
        }

        private float NormalizeAngle(float angle)
        {
            return (360 - angle) % 360;
        }

        private void UpdateTimeText(DateTime time)
        {
            if (_clockView.TimeText != null)
            {
                string timeString = $"{time:HH:mm:ss}";
                _clockView.TimeText.text = timeString;
            }
        }

        public void SetTimeFromInputs(int hour, int minute, int second)
        {
            _currentEditTime = new DateTime(
                _currentEditTime.Year,
                _currentEditTime.Month,
                _currentEditTime.Day,
                hour,
                minute,
                second
            );

            _clockAnimationService.UpdateClockHandsPositions(_currentEditTime);
            UpdateTimeText(_currentEditTime);
        }

        public async UniTaskVoid SyncTimeWithServer()
        {
            DateTime serverTime = await _clockSyncService.SyncTimeWithServer();

            if (_isEditModeActive)
            {
                _currentEditTime = serverTime;
                _clockAnimationService.PauseAnimations();
                UpdateClockDisplay(_currentEditTime);
            }
            else
            {
                UpdateClockDisplay(serverTime);
                _clockAnimationService.StartAnimations();
            }
        }

        public void Dispose()
        {
            _clockSyncService.OnTimeUpdated -= OnTimeUpdated;
            _clockAnimationService.Dispose();
        }
    }
}