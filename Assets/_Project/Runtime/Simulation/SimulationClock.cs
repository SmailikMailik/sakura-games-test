using System;
using Project.Runtime.Timing;
using R3;

namespace Project.Runtime.Simulation
{
    /// <summary>
    /// Измеряет игровое время от стартовой отметки и публикует секунды и изменения паузы.
    /// </summary>
    internal sealed class SimulationClock : IDisposable
    {
        private readonly IGameTime _time;
        private readonly ReactiveProperty<(double StartedAt, bool IsPaused)> _run = new((0d, false));
        private readonly ReadOnlyReactiveProperty<SimulationClockSnapshot> _state;

        internal TimeSpan Elapsed => TimeSpan.FromSeconds(_time.TimeAsDouble - _run.Value.StartedAt);
        internal bool IsPaused => _run.Value.IsPaused;
        internal SimulationClockSnapshot Snapshot => new(Elapsed, IsPaused);
        internal ReadOnlyReactiveProperty<SimulationClockSnapshot> State => _state;

        internal SimulationClock(IGameTime time)
        {
            _time = time;
            var frames = Observable.EveryUpdate().Prepend(Unit.Default);

            _state = _run
                .CombineLatest(frames, (_, _) => Snapshot)
                .DistinctUntilChangedBy(snapshot => (Seconds: Math.Floor(snapshot.Elapsed.TotalSeconds), snapshot.IsPaused))
                .ToReadOnlyReactiveProperty();
        }

        /// <summary>
        /// Публикует признак паузы; остановкой времени Unity управляет контроллер.
        /// </summary>
        internal void SetPaused(bool paused)
        {
            _run.Value = (_run.Value.StartedAt, paused);
        }

        /// <summary>
        /// Начинает отсчёт от текущего времени Unity и снимает признак паузы.
        /// </summary>
        internal void Reset()
        {
            _run.Value = (_time.TimeAsDouble, false);
        }

        /// <summary>
        /// Завершает принадлежащие часам потоки состояния.
        /// </summary>
        public void Dispose()
        {
            _state.Dispose();
            _run.Dispose();
        }
    }
}