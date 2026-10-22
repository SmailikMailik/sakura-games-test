using UnityEngine;

namespace Project.Runtime.Actors.Movement.Body
{
    /// <summary>
    /// Отличает опору от бокового контакта по нормалям столкновения.
    /// </summary>
    internal static class MotionContacts
    {
        private const float MinSupportNormalY = 0.5f;
        private const float MaxSideNormalY = 0.5f;

        /// <summary>
        /// Проверяет, есть ли в столкновении опора под телом.
        /// </summary>
        internal static bool HasSupport(Collision collision)
        {
            for (var i = 0; i < collision.contactCount; i++)
                if (collision.GetContact(i).normal.y > MinSupportNormalY)
                    return true;

            return false;
        }

        /// <summary>
        /// Суммирует горизонтальные нормали боковых контактов, исключая пол и потолок.
        /// </summary>
        internal static Vector2 GetSideNormal(Collision collision)
        {
            var result = Vector2.zero;
            for (var i = 0; i < collision.contactCount; i++)
            {
                var normal = collision.GetContact(i).normal;
                if (Mathf.Abs(normal.y) < MaxSideNormalY)
                    result += new Vector2(normal.x, normal.z);
            }

            return result;
        }
    }
}