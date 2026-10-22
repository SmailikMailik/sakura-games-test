namespace Project.Runtime.Population
{
    /// <summary>
    /// Число живых акторов и счётчики смертей на момент получения снимка.
    /// </summary>
    internal readonly struct PopulationSnapshot
    {
        internal int PreyDeaths { get; }
        internal int PredatorDeaths { get; }
        internal int LivingCount { get; }

        internal PopulationSnapshot(int preyDeaths, int predatorDeaths, int livingCount)
        {
            PreyDeaths = preyDeaths;
            PredatorDeaths = predatorDeaths;
            LivingCount = livingCount;
        }
    }
}