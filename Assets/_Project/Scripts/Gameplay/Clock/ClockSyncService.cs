using System;
using System.Threading;
using _Project.Scripts.Configs;
using _Project.Scripts.Services.TimeService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Clock
{
    public class ClockSyncService : ITickable, IDisposable
    {
        private DateTime _currentTime;
        private bool _isDisposed = false;
        private float _timeSinceLastSync;
        private readonly ITimeService _timeService;
        private readonly MainConfig _mainConfig;
        private readonly CancellationTokenSource _cts;
        private const float CorrectionThreshold = 1f;
        private bool _isEditModeActive = false;

        public event Action<DateTime> OnTimeUpdated;

        public DateTime CurrentTime => _currentTime;

        public ClockSyncService(ITimeService timeService, MainConfig mainConfig)
        {
            _timeService = timeService;
            _mainConfig = mainConfig;
            _cts = new CancellationTokenSource();
            InitializeTime().Forget();
        }

        private async UniTaskVoid InitializeTime()
        {
            if (!_timeService.IsTimePreloaded)
            {
                await _timeService.PreloadTimeAsync();
            }

            _currentTime = _timeService.GetPreloadedTime();
            OnTimeUpdated?.Invoke(_currentTime);
        }

        public void Tick()
        {
            if (_isDisposed || _isEditModeActive)
            {
                return;
            }

            _currentTime = _currentTime.AddSeconds(Time.deltaTime);
            OnTimeUpdated?.Invoke(_currentTime);

            _timeSinceLastSync += Time.deltaTime;
            
            if (_timeSinceLastSync >= _mainConfig.AutoSyncInterval)
            {
                SyncTimeWithServer().Forget();
                _timeSinceLastSync = 0f;
            }
        }

        public async UniTask<DateTime> SyncTimeWithServer()
        {
            if (_isDisposed) return _currentTime;

            try
            {
                DateTime onlineTime = await _timeService.GetOnlineTimeAsync().AttachExternalCancellation(_cts.Token);
                double difference = Math.Abs((_currentTime - onlineTime).TotalSeconds);

                if (difference > CorrectionThreshold)
                {
                    _currentTime = onlineTime;
                    OnTimeUpdated?.Invoke(_currentTime);
                }

                return _currentTime;
            }
            catch (OperationCanceledException)
            {
                return _currentTime;
            }
        }

        public void SetTime(DateTime newTime)
        {
            _currentTime = newTime;
            OnTimeUpdated?.Invoke(_currentTime);
        }

        public void SetEditMode(bool isActive)
        {
            _isEditModeActive = isActive;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}