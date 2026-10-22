using Project.Runtime.Actors.Movement.Behaviours;
using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Profiles
{
    /// <summary>
    /// Скорость прямолинейного движения и время между сменами курса.
    /// </summary>
    [CreateAssetMenu(menuName = "Zoo/Motion/Linear")]
    internal sealed class LinearMotionProfile : MotionProfile
    {
        [SerializeField, Min(0.01f)] private float _speed = 2f;
        [SerializeField, Min(0.1f)] private float _turnIntervalSeconds = 2f;

        internal float Speed => _speed;

        /// <summary>
        /// Собирает линейное движение с собственным таймером смены курса.
        /// </summary>
        internal override ActorMovement CreateMovement(RigidbodyMotor motor, ActorNavigation navigation)
        {
            return new(motor, navigation, new LinearMotionBehaviour(motor.Body, _speed, _turnIntervalSeconds));
        }
    }
}