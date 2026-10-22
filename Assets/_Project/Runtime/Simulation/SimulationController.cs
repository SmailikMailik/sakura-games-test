using System;
using Project.Runtime.Population;
using Project.Runtime.Timing;
using Project.Runtime.UI;
using Project.Runtime.UI.Popups;
using R3;
using VContainer.Unity;

namespace Project.Runtime.Simulation
{
    /// <summary>
    /// Связывает популяцию, часы и HUD; управляет паузой и перезапуском всей симуляции.
    /// </summary>
    internal sealed class SimulationController : IStartable, ITickable, IDisposable
    {
        private readonly ActorPopulation _population;
        private readonly SimulationClock _clock;
        private readonly Hud _hud;
        private readonly EatingPopupPresenterFactory _popupFactory;
        private readonly IGameTime _time;
        private readonly float _originalTimeScale;
        private bool _started;

        internal SimulationController(ActorPopulation population, SimulationClock clock, Hud hud,
            EatingPopupPresenterFactory popupFactory, IGameTime time)
        {
            _population = population;
            _clock = clock;
            _hud = hud;
            _popupFactory = popupFactory;
            _time = time;
            _originalTimeScale = _time.TimeScale;
        }

        /// <summary>
        /// Подключает HUD, сбрасывает часы и сразу делает первую попытку спавна.
        /// </summary>
        public void Start()
        {
            _clock.Reset();
            _population.Advance(0f);
            var state = _population.State.CombineLatest(_clock.State,
                (population, clock) => new HudSnapshot(population, clock.Elapsed, clock.IsPaused));
            _hud.Bind(state, _population.FoodConsumed, TogglePause, Restart, _popupFactory);
            _started = true;
        }

        /// <summary>
        /// Обновляет спавн вне паузы; изменения состояния доставляются интерфейсу через R3.
        /// </summary>
        public void Tick()
        {
            if (_clock.IsPaused is false)
                _population.Advance(_time.DeltaTime);
        }

        /// <summary>
        /// Отключает подписки и HUD, восстанавливает масштаб времени, сохранённый при создании контроллера.
        /// </summary>
        public void Dispose()
        {
            if (_started is false)
                return;

            _time.TimeScale = _originalTimeScale;
            // При выгрузке сцены HUD мог быть уже уничтожен.
            if (_hud != null)
                _hud.Unbind();

            _started = false;
        }

        private void TogglePause()
        {
            _clock.SetPaused(_clock.IsPaused is false);
            _time.TimeScale = _clock.IsPaused ? 0f : _originalTimeScale;
        }

        private void Restart()
        {
            _population.Reset();
            _clock.Reset();
            _time.TimeScale = _originalTimeScale;
            _hud.ClearPopups();
            _population.Advance(0f);
        }
    }
}