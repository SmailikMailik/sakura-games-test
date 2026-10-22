using Project.Runtime.Actors.Movement.Body;
using Project.Runtime.Randomness;
using Project.Runtime.Timing;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.Composition
{
    /// <summary>
    /// Регистрирует общие зависимости проекта и управляет временем их жизни.
    /// </summary>
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private ActorPhysicsSettings _physicsSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_physicsSettings);
            builder.Register<SystemRandomSource>(Lifetime.Singleton).As<IRandomSource>();
            builder.Register<UnityGameTime>(Lifetime.Singleton).As<IGameTime>();
        }
    }
}