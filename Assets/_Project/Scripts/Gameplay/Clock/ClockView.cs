using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Clock
{
    public class ClockView : MonoBehaviour
    {
        [field: SerializeField] public Transform HourHand { get; private set; }
        [field: SerializeField] public Transform MinuteHand { get; private set; }
        [field: SerializeField] public Transform SecondHand { get; private set; }
        [field: SerializeField] public TMP_Text TimeText { get; private set; }

        [field: SerializeField] private SpriteRenderer _backroundSpriteRenderer;
        [field: SerializeField] private Material _outlineMaterial;
        [field: SerializeField] private Material _fillMaterial;

        private Collider2D _hourHandCollider;
        private Collider2D _minuteHandCollider;
        private Collider2D _secondHandCollider;
        private Material _savedBackgroundMaterial;
        private Material _savedHourMaterial;
        private Material _savedMinuteMaterial;
        private Material _savedSecondMaterial;
        private SpriteRenderer _hourSpriteRenderer;
        private SpriteRenderer _minuteSpriteRenderer;
        private SpriteRenderer _secondSpriteRenderer;

        private void Awake()
        {
            _savedBackgroundMaterial = _backroundSpriteRenderer.material;
            _hourHandCollider = HourHand.GetComponent<Collider2D>();
            _minuteHandCollider = MinuteHand.GetComponent<Collider2D>();
            _secondHandCollider = SecondHand.GetComponent<Collider2D>();
            _hourSpriteRenderer = HourHand.GetComponent<SpriteRenderer>();
            _minuteSpriteRenderer = MinuteHand.GetComponent<SpriteRenderer>();
            _secondSpriteRenderer = SecondHand.GetComponent<SpriteRenderer>();
            _savedHourMaterial = _hourSpriteRenderer.material;
            _savedMinuteMaterial = _minuteSpriteRenderer.material;
            _savedSecondMaterial = _secondSpriteRenderer.material;
        }

        public Collider2D GetHandCollider(Transform hand)
        {
            if (hand == HourHand)
            {
                return _hourHandCollider;
            }

            if (hand == MinuteHand)
            {
                return _minuteHandCollider;
            }

            if (hand == SecondHand)
            {
                return _secondHandCollider;
            }
            
            return null;
        }

        public void SetIsEditMode(bool isEditMode)
        {
            _backroundSpriteRenderer.material = isEditMode == true ? _outlineMaterial : _savedBackgroundMaterial;
        }

        public void HighlightHourHand(bool highlight)
        {
            _hourSpriteRenderer.material = highlight ? _fillMaterial : _savedHourMaterial;
        }
        
        public void HighlightMinuteHand(bool highlight)
        {
            _minuteSpriteRenderer.material = highlight ? _fillMaterial : _savedMinuteMaterial;
        }
        
        public void HighlightSecondHand(bool highlight)
        {
            _secondSpriteRenderer.material = highlight ? _fillMaterial : _savedSecondMaterial;
        }

        public void ResetHightlighting()
        {
            HighlightHourHand(false);
            HighlightMinuteHand(false);
            HighlightSecondHand(false);
        }
    }
}