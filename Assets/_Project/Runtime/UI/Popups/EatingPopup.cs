using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Project.Runtime.UI.Popups
{
    /// <summary>
    /// Надпись поедания с одной переиспользуемой анимацией; оформление берётся из prefab.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    internal sealed class EatingPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Vector2 _offset = new(0f, -38f);
        [SerializeField, Min(0f)] private float _initialScale = 0.65f;
        [SerializeField, Min(0f)] private float _growDurationSeconds = 0.2f;
        [SerializeField, Min(0f)] private float _punchDurationSeconds = 0.35f;
        [SerializeField] private Vector3 _punchRotation = new(0f, 0f, 10f);
        [SerializeField, Min(1)] private int _punchVibrato = 5;
        [SerializeField, Min(0f)] private float _holdDurationSeconds = 0.85f;
        [SerializeField, Min(0f)] private float _fadeDurationSeconds = 0.3f;

        private RectTransform _rectTransform;
        private Sequence _animation;

        internal bool IsPlaying => _animation.IsPlaying();

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            var labelRect = _label.rectTransform;
            labelRect.localScale = Vector3.one * _initialScale;
            labelRect.localRotation = Quaternion.identity;
            _label.alpha = 1f;
            _animation = DOTween.Sequence().SetAutoKill(false).SetLink(gameObject).Pause();
            _animation.Append(labelRect.DOScale(1f, _growDurationSeconds).SetEase(Ease.OutBack));
            _animation.Join(labelRect.DOPunchRotation(_punchRotation, _punchDurationSeconds, _punchVibrato));
            _animation.AppendInterval(_holdDurationSeconds);
            _animation.Append(_label.DOFade(0f, _fadeDurationSeconds));
        }

        private void OnDestroy()
        {
            _animation.Kill();
        }

        /// <summary>
        /// Повторно запускает анимацию с начала; её время зависит от Time.timeScale.
        /// </summary>
        internal void Play()
        {
            _animation.Restart();
        }

        /// <summary>
        /// Размещает надпись относительно точки в контейнере UI с учётом настроенного смещения.
        /// </summary>
        internal void Position(Vector2 anchor)
        {
            _rectTransform.anchoredPosition = anchor + _offset;
        }

        /// <summary>
        /// Останавливает и перематывает анимацию, сохраняя её для следующего показа.
        /// </summary>
        internal void Stop()
        {
            _animation.Rewind();
        }
    }
}