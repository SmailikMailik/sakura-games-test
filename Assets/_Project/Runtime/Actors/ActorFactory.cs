using Project.Runtime.Actors.Contacts;
using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors
{
    /// <summary>
    /// Создаёт участника симуляции и связывает его состояние, физическое тело и движение.
    /// </summary>
    internal sealed class ActorFactory
    {
        private readonly Transform _root;
        private readonly ActorNavigation _navigation;
        private readonly ActorPhysicsSettings _physicsSettings;

        internal ActorFactory(Transform root, ActorNavigation navigation, ActorPhysicsSettings physicsSettings)
        {
            _root = root;
            _navigation = navigation;
            _physicsSettings = physicsSettings;
        }

        /// <summary>
        /// Создаёт актора в заданной точке и подключает обработчик его столкновений.
        /// </summary>
        internal Actor Create(ActorDefinition definition, Vector3 position, long order,
            IActorContactHandler contacts)
        {
            var actor = Object.Instantiate(definition.Prefab, position, Quaternion.identity, _root);
            actor.name = $"{definition.name} {order}";
            var motor = new RigidbodyMotor(actor.Rigidbody, _physicsSettings.RecoveryAcceleration);
            var state = new ActorState(definition.Role, order);
            var movement = definition.Motion.CreateMovement(motor, _navigation);
            actor.Initialize(state, movement, contacts);
            return actor;
        }
    }
}