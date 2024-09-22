using UnityEngine;
using DG.Tweening;
using System;

namespace _Project.Scripts.Gameplay.Clock
{
    public class ClockAnimationService : IDisposable
    {
        private readonly ClockView _clockView;
        private Tween _secondHandTween;
        private Tween _minuteHandTween;
        private Tween _hourHandTween;
        private bool _isDisposed = false;
        private bool _isAnimationPaused = true;

        public ClockAnimationService(ClockView clockView)
        {
            _clockView = clockView;
            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            _secondHandTween = _clockView.SecondHand
                .DOLocalRotate(new Vector3(0, 0, -360), 60f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            _minuteHandTween = _clockView.MinuteHand
                .DOLocalRotate(new Vector3(0, 0, -360), 3600f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            _hourHandTween = _clockView.HourHand
                .DOLocalRotate(new Vector3(0, 0, -360), 43200f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            PauseAnimations();
        }

        public void UpdateClockHandsPositions(DateTime currentTime)
        {
            float secondsElapsed = currentTime.Second + currentTime.Millisecond / 1000f;
            float minutesElapsed = currentTime.Minute * 60 + secondsElapsed;
            float hoursElapsed = (currentTime.Hour % 12) * 3600 + minutesElapsed;

            _secondHandTween.Goto(secondsElapsed, false);
            _minuteHandTween.Goto(minutesElapsed, false);
            _hourHandTween.Goto(hoursElapsed, false);
        }

        public void StartAnimations()
        {
            if (_isAnimationPaused)
            {
                _secondHandTween.Play();
                _minuteHandTween.Play();
                _hourHandTween.Play();
                _isAnimationPaused = false;
            }
        }

        public void PauseAnimations()
        {
            if (!_isAnimationPaused)
            {
                _secondHandTween.Pause();
                _minuteHandTween.Pause();
                _hourHandTween.Pause();
                _isAnimationPaused = true;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _secondHandTween?.Kill();
            _minuteHandTween?.Kill();
            _hourHandTween?.Kill();
        }
    }
}