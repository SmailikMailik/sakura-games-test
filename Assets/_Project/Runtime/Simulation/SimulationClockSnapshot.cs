using System;

namespace Project.Runtime.Simulation
{
    /// <summary>
    /// Игровое время и состояние паузы на момент получения снимка.
    /// </summary>
    internal readonly struct SimulationClockSnapshot
    {
        internal TimeSpan Elapsed { get; }
        internal bool IsPaused { get; }

        internal SimulationClockSnapshot(TimeSpan elapsed, bool isPaused)
        {
            Elapsed = elapsed;
            IsPaused = isPaused;
        }
    }
}