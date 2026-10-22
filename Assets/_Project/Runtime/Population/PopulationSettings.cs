using System.Collections.Generic;
using Project.Runtime.Actors;
using UnityEngine;

namespace Project.Runtime.Population
{
    /// <summary>
    /// Задаёт состав акторов и параметры их размещения на площадке.
    /// </summary>
    [CreateAssetMenu(menuName = "Zoo/Population settings")]
    internal sealed class PopulationSettings : ScriptableObject
    {
        [SerializeField] private ActorDefinition[] _actorDefinitions;

        [Header("Спавн")]
        [SerializeField] private Vector2 _spawnIntervalSeconds = new(1f, 2f);
        [SerializeField, Min(0f)] private float _boundaryPadding = 0.3f;
        [SerializeField, Min(0f)] private float _actorSeparationPadding = 0.05f;
        [SerializeField, Min(0f)] private float _obstacleClearance = 0.05f;
        [SerializeField, Min(0f)] private float _retryIntervalSeconds = 0.02f;
        [SerializeField, Min(1)] private int _maxPlacementAttempts = 16;

        internal IReadOnlyList<ActorDefinition> ActorDefinitions => _actorDefinitions;
        internal Vector2 SpawnIntervalSeconds => _spawnIntervalSeconds;
        internal float BoundaryPadding => _boundaryPadding;
        internal float ActorSeparationPadding => _actorSeparationPadding;
        internal float ObstacleClearance => _obstacleClearance;
        internal float RetryIntervalSeconds => _retryIntervalSeconds;
        internal int MaxPlacementAttempts => _maxPlacementAttempts;
    }
}