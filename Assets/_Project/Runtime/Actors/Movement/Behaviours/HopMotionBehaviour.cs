using System;
using Project.Runtime.Actors.Movement.Body;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Behaviours
{
    /// <summary>
    /// Следит за интервалом прыжков и отталкивается от опоры; полёт рассчитывает физика Unity.
    /// </summary>
    internal sealed class HopMotionBehaviour : IMotionBehaviour
    {
        private readonly GroundContactTracker _groundContacts = new();
        private readonly Rigidbody _body;
        private readonly float _horizontalSpeed;
        private readonly float _height;
        private readonly float _jumpIntervalSeconds;
        private float _jumpCooldownSeconds;
        private bool _isJumping;
        private bool _ignoreGroundContactsUntilDescending;

        internal HopMotionBehaviour(Rigidbody body, float distance, float height, float intervalSeconds)
        {
            _body = body;
            var flightTime = 2f * Mathf.Sqrt(-2f * height / Physics.gravity.y);
            _horizontalSpeed = distance / flightTime;
            _height = height;
            _jumpIntervalSeconds = intervalSeconds;
        }

        /// <summary>
        /// Прыгает с опоры, когда истёк интервал, и задаёт скорость вперёд до приземления.
        /// </summary>
        public float Advance(float deltaTime, Action chooseDirection)
        {
            if (_ignoreGroundContactsUntilDescending && _body.linearVelocity.y <= 0f)
                _ignoreGroundContactsUntilDescending = false;

            _jumpCooldownSeconds -= deltaTime;
            if (_groundContacts.HasActiveContact())
                UpdateGroundedMotion(chooseDirection);

            return _isJumping ? _horizontalSpeed : 0f;
        }

        /// <summary>
        /// Передаёт контакты учёту опор. После отталкивания игнорирует их до начала снижения.
        /// </summary>
        public void ObserveContact(Collision collision)
        {
            if (_ignoreGroundContactsUntilDescending)
                _groundContacts.RemoveContact(collision.collider);
            else
                _groundContacts.ObserveContact(collision);
        }

        /// <summary>
        /// Забывает поверхность, с которой тело больше не соприкасается.
        /// </summary>
        public void RemoveContact(Collider other)
        {
            _groundContacts.RemoveContact(other);
        }

        private void UpdateGroundedMotion(Action chooseDirection)
        {
            // На опоре гасим отскок; в полёте вертикальную скорость меняет физика Unity.
            VerticalMotion.StopRising(_body);

            if (_jumpCooldownSeconds > 0f)
            {
                _isJumping = false;
                return;
            }

            chooseDirection();
            StartJump();
        }

        private void StartJump()
        {
            _isJumping = true;
            VerticalMotion.SetSpeed(_body, Mathf.Sqrt(-2f * Physics.gravity.y * _height));
            _groundContacts.Clear();
            _ignoreGroundContactsUntilDescending = true;
            _jumpCooldownSeconds = _jumpIntervalSeconds;
        }
    }
}