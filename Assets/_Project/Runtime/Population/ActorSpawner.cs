using System.Collections.Generic;
using Project.Runtime.Actors;
using Project.Runtime.Actors.Contacts;
using Project.Runtime.Randomness;

namespace Project.Runtime.Population
{
    /// <summary>
    /// Планирует появление акторов; при неудаче сохраняет выбранную конфигурацию до свободного места.
    /// </summary>
    internal sealed class ActorSpawner
    {
        private readonly PopulationSettings _settings;
        private readonly ActorFactory _factory;
        private readonly ActorSpawnPlacement _placement;
        private readonly IRandomSource _random;
        private ActorDefinition _pendingDefinition;
        private float _spawnRemainingSeconds;
        private long _spawnOrder;

        internal ActorSpawner(PopulationSettings settings, ActorFactory factory, ActorSpawnPlacement placement,
            IRandomSource random)
        {
            _settings = settings;
            _factory = factory;
            _placement = placement;
            _random = random;
        }

        /// <summary>
        /// Создаёт не более одного актора за шаг; возвращает false, пока не наступил срок или нет места.
        /// </summary>
        internal bool Advance(float deltaTime, IReadOnlyList<Actor> actors, IActorContactHandler contacts,
            out Actor actor)
        {
            actor = null;
            _spawnRemainingSeconds -= deltaTime;
            if (_spawnRemainingSeconds > 0f)
                return false;

            if (_pendingDefinition == null)
                _pendingDefinition = _settings.ActorDefinitions[_random.Range(0, _settings.ActorDefinitions.Count)];

            if (_placement.TryFindPosition(_pendingDefinition.Prefab, actors, out var position) is false)
            {
                // Пропущенные появления не накапливаются; выбранная конфигурация ждёт свободного места.
                _spawnRemainingSeconds = _settings.RetryIntervalSeconds;
                return false;
            }

            actor = _factory.Create(_pendingDefinition, position, ++_spawnOrder, contacts);
            _pendingDefinition = null;
            _spawnRemainingSeconds = _random.Range(_settings.SpawnIntervalSeconds.x, _settings.SpawnIntervalSeconds.y);
            return true;
        }

        /// <summary>
        /// Сбрасывает выбор, порядковые номера и таймер; следующая попытка не требует ожидания.
        /// </summary>
        internal void Reset()
        {
            _pendingDefinition = null;
            _spawnRemainingSeconds = 0f;
            _spawnOrder = 0;
        }
    }
}