using System.Collections.Generic;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Body
{
    /// <summary>
    /// Запоминает поверхности под телом и проверяет, осталась ли действующая опора.
    /// </summary>
    internal sealed class GroundContactTracker
    {
        // Потеря одной поверхности не означает потерю остальных опор.
        private readonly HashSet<Collider> _groundColliders = new();

        /// <summary>
        /// Проверяет, осталась ли хотя бы одна включённая опора на активном объекте.
        /// </summary>
        internal bool HasActiveContact()
        {
            // Поверхность может исчезнуть или отключиться без уведомления о завершении контакта.
            foreach (var collider in _groundColliders)
            {
                if (collider == null)
                    continue;

                if (collider.enabled && collider.gameObject.activeInHierarchy)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Обновляет поверхность по нормалям текущего физического контакта.
        /// </summary>
        internal void ObserveContact(Collision collision)
        {
            if (MotionContacts.HasSupport(collision))
                _groundColliders.Add(collision.collider);
            else
                _groundColliders.Remove(collision.collider);
        }

        /// <summary>
        /// Забывает поверхность после завершения контакта с ней.
        /// </summary>
        internal void RemoveContact(Collider collider)
        {
            _groundColliders.Remove(collider);
        }

        /// <summary>
        /// Забывает все ранее полученные контакты.
        /// </summary>
        internal void Clear()
        {
            _groundColliders.Clear();
        }
    }
}