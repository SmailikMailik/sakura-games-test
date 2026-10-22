using Project.Runtime.Actors;
using Project.Runtime.Population;
using Project.Runtime.Simulation;
using Project.Runtime.Space;
using Project.Runtime.UI;
using Project.Runtime.UI.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.Composition
{
    /// <summary>
    /// Регистрирует зависимости сцены и управляет временем их жизни.
    /// </summary>
    internal sealed class MainLifetimeScope : LifetimeScope
    {
        [SerializeField] private PopulationSettings _settings;
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private Hud _hud;
        [SerializeField] private Transform _actorRoot;

        internal PopulationSettings Settings => _settings;
        internal Transform ActorRoot => _actorRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            // Спавнер ищет препятствия в физическом мире текущей сцены.
            var physicsScene = gameObject.scene.GetPhysicsScene();

            builder.RegisterInstance(Settings);
            builder.RegisterInstance(_worldCamera);
            builder.RegisterInstance(_hud);
            builder.Register<EatingPopupPresenterFactory>(Lifetime.Scoped);
            builder.Register<ActorSpawnPlacement>(Lifetime.Scoped).WithParameter(physicsScene);
            builder.Register<CameraWorldArea>(Lifetime.Scoped);
            builder.Register<ActorNavigation>(Lifetime.Scoped);
            builder.Register<ActorFactory>(Lifetime.Scoped).WithParameter(ActorRoot);
            builder.Register<ActorSpawner>(Lifetime.Scoped);
            builder.Register<ActorPopulation>(Lifetime.Scoped);
            builder.Register<SimulationClock>(Lifetime.Scoped);
            builder.RegisterEntryPoint<SimulationController>(Lifetime.Scoped);
        }
    }
}