using System;
using Project.Runtime.Actors.Movement.Body;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Behaviours
{
    /// <summary>
    /// Держит заданную скорость и выбирает новый курс по таймеру.
    /// </summary>
    internal sealed class LinearMotionBehaviour : IMotionBehaviour
    {
        private readonly Rigidbody _body;
        private readonly float _speed;
        private readonly float _turnIntervalSeconds;
        private float _turnRemainingSeconds;

        internal LinearMotionBehaviour(Rigidbody body, float speed, float turnIntervalSeconds)
        {
            _body = body;
            _speed = speed;
            _turnIntervalSeconds = turnIntervalSeconds;
            _turnRemainingSeconds = turnIntervalSeconds;
        }

        /// <summary>
        /// Возвращает постоянную скорость и запрашивает новый курс после истечения интервала.
        /// </summary>
        public float Advance(float deltaTime, Action chooseDirection)
        {
            if (_turnRemainingSeconds <= 0f)
            {
                chooseDirection();
                // Сохраняем остаток времени, чтобы не сдвигать следующие повороты.
                _turnRemainingSeconds = _turnIntervalSeconds + _turnRemainingSeconds % _turnIntervalSeconds;
            }

            _turnRemainingSeconds -= deltaTime;
            return _speed;
        }

        /// <summary>
        /// Гасит вертикальный отскок от опоры, сохраняя боковое расталкивание.
        /// </summary>
        public void ObserveContact(Collision collision)
        {
            if (MotionContacts.HasSupport(collision) is false)
                return;

            VerticalMotion.StopRising(_body);
        }

        /// <summary>
        /// Завершение контакта не меняет состояние прямолинейного движения.
        /// </summary>
        public void RemoveContact(Collider other) { }
    }
}