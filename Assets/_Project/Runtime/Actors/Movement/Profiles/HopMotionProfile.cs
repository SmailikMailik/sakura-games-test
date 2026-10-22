using Project.Runtime.Actors.Movement.Behaviours;
using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors.Movement.Profiles
{
    /// <summary>
    /// Настройки прыжка: высота, расстояние и интервал между началами прыжков.
    /// </summary>
    [CreateAssetMenu(menuName = "Zoo/Motion/Hop")]
    internal sealed class HopMotionProfile : MotionProfile
    {
        [SerializeField, Min(0.01f)] private float _distance = 1.8f;
        [SerializeField, Min(0.01f)] private float _height = 0.7f;
        [SerializeField, Min(0.02f)] private float _intervalSeconds = 1.2f;

        internal float Distance => _distance;
        internal float Height => _height;

        /// <summary>
        /// Собирает прыжковое движение с собственным таймером и учётом опор.
        /// </summary>
        internal override ActorMovement CreateMovement(RigidbodyMotor motor, ActorNavigation navigation)
        {
            return new(motor, navigation, new HopMotionBehaviour(motor.Body, _distance, _height, _intervalSeconds));
        }
    }
}