using UnityEngine;

namespace Project.Runtime.Actors.Movement.Body
{
    /// <summary>
    /// Меняет скорость тела по Y, не трогая горизонтальное движение.
    /// </summary>
    internal static class VerticalMotion
    {
        /// <summary>
        /// Задаёт вертикальную скорость, не меняя скорость в плоскости XZ.
        /// </summary>
        internal static void SetSpeed(Rigidbody body, float speed)
        {
            var velocity = body.linearVelocity;
            velocity.y = speed;
            body.linearVelocity = velocity;
        }

        /// <summary>
        /// Убирает движение вверх, сохраняя падение и горизонтальное расталкивание.
        /// </summary>
        internal static void StopRising(Rigidbody body)
        {
            SetSpeed(body, Mathf.Min(body.linearVelocity.y, 0f));
        }
    }
}