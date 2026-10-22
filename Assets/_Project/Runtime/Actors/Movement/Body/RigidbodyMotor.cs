using UnityEngine;

namespace Project.Runtime.Actors.Movement.Body
{
    /// <summary>
    /// Задаёт горизонтальное движение Rigidbody и постепенно гасит внешние толчки.
    /// </summary>
    internal sealed class RigidbodyMotor
    {
        private readonly Rigidbody _body;
        private readonly float _recoveryAcceleration;
        private Vector2 _previousDrive;

        internal Rigidbody Body => _body;
        internal Vector3 Position => _body.position;

        internal RigidbodyMotor(Rigidbody body, float recoveryAcceleration)
        {
            _body = body;
            _recoveryAcceleration = recoveryAcceleration;
        }

        /// <summary>
        /// Задаёт скорость в плоскости XZ, сохраняя вертикальную скорость и остаток внешнего толчка.
        /// </summary>
        internal void Move(Vector2 velocity, float deltaTime)
        {
            var current = _body.linearVelocity;
            var horizontal = RecoverHorizontalVelocity(new(current.x, current.z), velocity, deltaTime);
            _body.linearVelocity = new(horizontal.x, current.y, horizontal.y);
            _previousDrive = velocity;
        }

        private Vector2 RecoverHorizontalVelocity(Vector2 current, Vector2 drive, float deltaTime)
        {
            var externalVelocity = current - _previousDrive;
            // Замедление от трения не должно превращаться в обратное движение после остановки.
            if (Vector2.Dot(current, externalVelocity) < 0f)
                externalVelocity = Vector2.zero;

            externalVelocity = Vector2.MoveTowards(externalVelocity, Vector2.zero, _recoveryAcceleration * deltaTime);
            return drive + externalVelocity;
        }
    }
}