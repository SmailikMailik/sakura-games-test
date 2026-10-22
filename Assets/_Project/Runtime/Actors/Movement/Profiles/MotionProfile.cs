using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Profiles
{
    /// <summary>
    /// Хранит общие настройки и собирает отдельное движение для каждого актора.
    /// </summary>
    internal abstract class MotionProfile : ScriptableObject
    {
        /// <summary>
        /// Собирает движение с заданным мотором и навигацией; таймеры принадлежат новому экземпляру.
        /// </summary>
        internal abstract ActorMovement CreateMovement(RigidbodyMotor motor, ActorNavigation navigation);
    }
}