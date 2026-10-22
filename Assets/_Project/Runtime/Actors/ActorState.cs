namespace Project.Runtime.Actors
{
    /// <summary>
    /// Хранит пищевую роль, порядок появления и признак жизни одного участника симуляции.
    /// </summary>
    internal sealed class ActorState
    {
        internal FoodRole Role { get; }
        internal long SpawnOrder { get; }
        internal bool IsAlive { get; private set; } = true;

        internal ActorState(FoodRole role, long spawnOrder)
        {
            Role = role;
            SpawnOrder = spawnOrder;
        }

        /// <summary>
        /// Помечает актора погибшим. Повторный вызов возвращает false и ничего не меняет.
        /// </summary>
        internal bool TryDie()
        {
            if (IsAlive is false)
                return false;

            IsAlive = false;
            return true;
        }
    }
}