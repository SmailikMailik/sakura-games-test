using Project.Runtime.Randomness;
using UnityEngine;

namespace Project.Runtime.Space
{
    /// <summary>
    /// Выбирает курс актора с учётом стен и видимых границ площадки.
    /// </summary>
    internal sealed class ActorNavigation
    {
        private readonly CameraWorldArea _area;
        private readonly IRandomSource _random;

        internal ActorNavigation(CameraWorldArea area, IRandomSource random)
        {
            _area = area;
            _random = random;
        }

        /// <summary>
        /// Возвращает случайный единичный курс в плоскости XZ.
        /// </summary>
        internal Vector2 NextDirection()
        {
            var angle = _random.Range(0f, Mathf.PI * 2f);
            return new(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary>
        /// Отражает курс, направленный внутрь стены; движение вдоль стены или от неё сохраняет.
        /// </summary>
        internal Vector2 AvoidObstacle(Vector2 current, Vector2 surfaceNormal)
        {
            if (Vector2.Dot(current, surfaceNormal) >= 0f)
                return current;

            return Vector2.Reflect(current, surfaceNormal.normalized).normalized;
        }

        /// <summary>
        /// За видимыми границами возвращает курс к центру площадки; внутри сохраняет текущий курс.
        /// </summary>
        internal Vector2 ReturnDirection(Vector3 position, Vector2 current)
        {
            var point = new Vector2(position.x, position.z);
            var bounds = _area.Bounds;
            return bounds.Contains(point) ? current : (bounds.center - point).normalized;
        }
    }
}