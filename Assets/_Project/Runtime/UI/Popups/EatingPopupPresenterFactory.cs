using UnityEngine;

namespace Project.Runtime.UI.Popups
{
    /// <summary>
    /// Собирает presenter надписей для HUD с камерой текущей сцены.
    /// </summary>
    internal sealed class EatingPopupPresenterFactory
    {
        private readonly Camera _camera;

        internal EatingPopupPresenterFactory(Camera camera)
        {
            _camera = camera;
        }

        /// <summary>
        /// Создаёт владельца пула надписей; вызывающий HUD освобождает его при отвязке.
        /// </summary>
        internal EatingPopupPresenter Create(EatingPopup prefab, RectTransform root)
        {
            return new(prefab, root, _camera);
        }
    }
}