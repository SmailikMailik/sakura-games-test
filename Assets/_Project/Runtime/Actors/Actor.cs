using System;
using Project.Runtime.Actors.Contacts;
using Project.Runtime.Actors.Movement;
using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Actors
{
    /// <summary>
    /// Передаёт Unity-контакты и физические шаги движению, поворачивает Visual по выбранному курсу.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    internal sealed class Actor : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _collider;

        private Transform _transform;
        private ActorMovement _movement;
        private IActorContactHandler _contacts;
        private bool _initialized;
        private float _radius;

        internal Rigidbody Rigidbody => _rigidbody;
        internal ActorState State { get; private set; }
        internal Vector3 Position => _transform.position;
        internal bool IsAlive => _initialized && State.IsAlive;
        internal float Radius => _initialized ? _radius : ColliderRadius();

        internal Vector3 ColliderCenterOffset => Vector3.Scale(_collider.center, transform.lossyScale);

        private void Awake()
        {
            // Если родитель был неактивен, Awake может вызваться уже после Initialize.
            enabled = _initialized;
        }

        private void FixedUpdate()
        {
            if (State.IsAlive is false)
                return;

            _movement.Advance(Time.fixedDeltaTime);
            var heading = _movement.Heading;
            _visual.rotation = Quaternion.LookRotation(new(heading.x, 0f, heading.y));
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Unity сообщает о столкновениях даже отключённым MonoBehaviour.
            if (IsAlive is false)
                return;

            ObserveContact(collision);
            var otherBody = collision.rigidbody;
            if (otherBody == null)
                return;

            if (otherBody.TryGetComponent<Actor>(out var other) is false)
                return;

            if (other._initialized is false)
                return;

            _contacts.HandleContact(this, other);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (IsAlive)
                ObserveContact(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_initialized)
                _movement.RemoveContact(collision.collider);
        }

        /// <summary>
        /// Подключает состояние, движение и обработчик контактов, затем включает обновление актора.
        /// </summary>
        internal void Initialize(ActorState state, ActorMovement movement,
            IActorContactHandler contacts)
        {
            if (_initialized)
                throw new InvalidOperationException("[Actor::Initialize] Actor is already initialized.");

            _transform = transform;
            State = state;
            _movement = movement;
            _contacts = contacts;
            _radius = ColliderRadius();
            _initialized = true;
            enabled = true;
        }

        private void ObserveContact(Collision collision)
        {
            _movement.ObserveContact(collision);
            if (collision.collider.GetComponentInParent<WorldObstacle>() == null)
                return;

            _movement.AvoidObstacle(MotionContacts.GetSideNormal(collision));
        }

        private float ColliderRadius()
        {
            var scale = transform.lossyScale;
            return _collider.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        }
    }
}