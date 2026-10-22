using System;
using Project.Runtime.Population;

namespace Project.Runtime.UI
{
    /// <summary>
    /// Показатели популяции, прошедшее время и признак паузы для одного обновления HUD.
    /// </summary>
    internal readonly struct HudSnapshot
    {
        internal PopulationSnapshot Population { get; }
        internal TimeSpan Elapsed { get; }
        internal bool IsPaused { get; }

        internal HudSnapshot(PopulationSnapshot population, TimeSpan elapsed, bool isPaused)
        {
            Population = population;
            Elapsed = elapsed;
            IsPaused = isPaused;
        }
    }
}