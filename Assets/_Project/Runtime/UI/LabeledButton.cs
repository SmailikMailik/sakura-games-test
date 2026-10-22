using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.UI
{
    /// <summary>
    /// Объединяет кнопку с подписью и заменяемым действием нажатия.
    /// </summary>
    [RequireComponent(typeof(Button))]
    internal sealed class LabeledButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        private IDisposable _actionSubscription;

        private void OnDestroy()
        {
            _actionSubscription?.Dispose();
        }

        /// <summary>
        /// Задаёт текст подписи кнопки.
        /// </summary>
        internal void SetText(string text)
        {
            _label.text = text;
        }

        /// <summary>
        /// Заменяет действие нажатия; освобождение возвращённой подписки отключает это действие.
        /// </summary>
        /// <returns>Подписка на нажатие, которую может освободить владелец привязки.</returns>
        internal IDisposable SetAction(Action action)
        {
            _actionSubscription?.Dispose();
            _actionSubscription = _button.OnClickAsObservable().Subscribe(_ => action());
            return _actionSubscription;
        }
    }
}