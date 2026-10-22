using System;
using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors.Movement
{
    /// <summary>
    /// Вызывает выбранное поведение, корректирует курс и передаёт скорость физическому мотору.
    /// </summary>
    internal sealed class ActorMovement
    {
        private readonly RigidbodyMotor _motor;
        private readonly ActorNavigation _navigation;
        private readonly IMotionBehaviour _behaviour;
        private readonly Action _chooseDirection;

        internal Vector2 Heading { get; private set; }

        internal ActorMovement(RigidbodyMotor motor, ActorNavigation navigation, IMotionBehaviour behaviour)
        {
            _motor = motor;
            _navigation = navigation;
            _behaviour = behaviour;
            _chooseDirection = ChooseDirection;
            ChooseDirection();
        }

        /// <summary>
        /// Обновляет поведение, возвращает курс внутрь площадки и задаёт скорость мотору.
        /// </summary>
        internal void Advance(float deltaTime)
        {
            var speed = _behaviour.Advance(deltaTime, _chooseDirection);
            Heading = _navigation.ReturnDirection(_motor.Position, Heading);
            _motor.Move(Heading * speed, deltaTime);
        }

        /// <summary>
        /// Передаёт контакт выбранному поведению движения.
        /// </summary>
        internal void ObserveContact(Collision collision)
        {
            _behaviour.ObserveContact(collision);
        }

        /// <summary>
        /// Уведомляет поведение о завершении физического контакта.
        /// </summary>
        internal void RemoveContact(Collider other)
        {
            _behaviour.RemoveContact(other);
        }

        /// <summary>
        /// Меняет курс, если он направлен в поверхность препятствия.
        /// </summary>
        internal void AvoidObstacle(Vector2 surfaceNormal)
        {
            Heading = _navigation.AvoidObstacle(Heading, surfaceNormal);
        }

        private void ChooseDirection()
        {
            Heading = _navigation.NextDirection();
        }
    }
}