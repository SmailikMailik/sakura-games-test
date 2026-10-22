using System;
using System.Collections.Generic;
using Project.Runtime.Actors;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Project.Runtime.UI.Popups
{
    /// <summary>
    /// Владеет пулом надписей поедания; у каждого актора может быть не более одной активной надписи.
    /// </summary>
    internal sealed class EatingPopupPresenter : IDisposable
    {
        private const int InitialCapacity = 8;
        private const int MaxInactivePopups = 32;

        private readonly EatingPopup _prefab;
        private readonly RectTransform _root;
        private readonly Camera _camera;
        private readonly List<Popup> _popups = new(InitialCapacity);
        private readonly ObjectPool<EatingPopup> _pool;

        internal EatingPopupPresenter(EatingPopup prefab, RectTransform root, Camera camera)
        {
            _prefab = prefab;
            _root = root;
            _camera = camera;
            _pool = new(CreatePopup,
                ActivatePopup,
                ReleasePopup,
                DestroyPopup,
                defaultCapacity: InitialCapacity,
                maxSize: MaxInactivePopups);
        }

        /// <summary>
        /// Показывает надпись у живого актора; повторное поедание перезапускает ту же надпись.
        /// </summary>
        internal void Show(Actor eater)
        {
            if (eater.IsAlive is false)
                return;

            var popup = GetOrCreatePopup(eater);
            popup.View.Play();
            PositionPopup(popup);
        }

        /// <summary>
        /// Обновляет позиции надписей; убирает завершённые и оставшиеся без живого актора.
        /// </summary>
        internal void Update()
        {
            for (var i = _popups.Count - 1; i >= 0; i--)
            {
                var popup = _popups[i];
                if (popup.ShouldRemove)
                    RemovePopup(i);
                else
                    PositionPopup(popup);
            }
        }

        /// <summary>
        /// Возвращает все активные надписи в пул без его уничтожения.
        /// </summary>
        internal void Clear()
        {
            for (var i = _popups.Count - 1; i >= 0; i--)
                RemovePopup(i);
        }

        /// <summary>
        /// Уничтожает активные и свободные надписи, освобождая пул.
        /// </summary>
        public void Dispose()
        {
            Clear();
            _pool.Dispose();
        }

        private Popup GetOrCreatePopup(Actor owner)
        {
            for (var i = _popups.Count - 1; i >= 0; i--)
            {
                var existing = _popups[i];
                if (existing.Owner != owner)
                    continue;

                if (existing.View != null)
                    return existing;

                RemovePopup(i);
                break;
            }

            var popup = new Popup(owner, _pool.Get());
            _popups.Add(popup);
            return popup;
        }

        private EatingPopup CreatePopup()
        {
            return Object.Instantiate(_prefab, _root, false);
        }

        private static void ActivatePopup(EatingPopup popup)
        {
            popup.gameObject.SetActive(true);
        }

        private static void ReleasePopup(EatingPopup popup)
        {
            popup.Stop();
            popup.gameObject.SetActive(false);
        }

        private static void DestroyPopup(EatingPopup popup)
        {
            // При выгрузке сцены дочерние надписи могли быть уже уничтожены.
            if (popup != null)
                Object.Destroy(popup.gameObject);
        }

        private void PositionPopup(Popup popup)
        {
            var screen = _camera.WorldToScreenPoint(popup.Owner.Position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_root, screen, null, out var local);
            popup.View.Position(local);
        }

        private void RemovePopup(int index)
        {
            var popup = _popups[index];
            if (popup.View != null)
                _pool.Release(popup.View);

            _popups.RemoveAt(index);
        }

        /// <summary>
        /// Пара из актора и показанной рядом с ним надписи.
        /// </summary>
        private readonly struct Popup
        {
            internal readonly Actor Owner;
            internal readonly EatingPopup View;

            internal bool ShouldRemove => Owner == null || Owner.IsAlive is false ||
                                          View == null || View.IsPlaying is false;

            internal Popup(Actor owner, EatingPopup view)
            {
                Owner = owner;
                View = view;
            }
        }
    }
}