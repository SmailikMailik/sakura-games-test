using System;
using UnityEngine;

namespace Project.Runtime.Actors.Movement
{
    /// <summary>
    /// Определяет скорость и момент смены курса, обрабатывает необходимые поведению контакты.
    /// </summary>
    internal interface IMotionBehaviour
    {
        /// <summary>
        /// Обновляет состояние, при необходимости вызывает выбор курса и возвращает горизонтальную скорость.
        /// </summary>
        float Advance(float deltaTime, Action chooseDirection);

        /// <summary>
        /// Обрабатывает физический контакт тела.
        /// </summary>
        void ObserveContact(Collision collision);

        /// <summary>
        /// Завершает обработку контакта с поверхностью.
        /// </summary>
        void RemoveContact(Collider other);
    }
}