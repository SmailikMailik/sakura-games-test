using System.Collections.Generic;
using Project.Runtime.Actors;
using Project.Runtime.Randomness;
using Project.Runtime.Space;
using UnityEngine;

namespace Project.Runtime.Population
{
    /// <summary>
    /// Ищет свободную точку в границах камеры и физическом мире текущей симуляции.
    /// </summary>
    internal sealed class ActorSpawnPlacement
    {
        private readonly PopulationSettings _settings;
        private readonly CameraWorldArea _area;
        private readonly IRandomSource _random;
        private readonly PhysicsScene _physicsScene;
        private readonly Collider[] _overlaps = new Collider[1];
        private readonly int _obstacleLayerMask = LayerMask.GetMask(WorldObstacle.LayerName);

        internal ActorSpawnPlacement(PopulationSettings settings, CameraWorldArea area, IRandomSource random,
            PhysicsScene physicsScene)
        {
            _settings = settings;
            _area = area;
            _random = random;
            _physicsScene = physicsScene;
        }

        /// <summary>
        /// Возвращает свободную точку для prefab; false означает отсутствие места за отведённые попытки.
        /// </summary>
        internal bool TryFindPosition(Actor prefab, IReadOnlyList<Actor> actors,
            out Vector3 position)
        {
            position = default;
            var actorRadius = prefab.Radius;
            var colliderOffset = prefab.ColliderCenterOffset;
            var bounds = _area.Bounds;
            var boundaryMargin = actorRadius + _settings.BoundaryPadding;
            var requiredSize = boundaryMargin * 2f;
            if (bounds.width <= requiredSize || bounds.height <= requiredSize)
                return false;

            var minX = bounds.xMin + boundaryMargin;
            var maxX = bounds.xMax - boundaryMargin;
            var minZ = bounds.yMin + boundaryMargin;
            var maxZ = bounds.yMax - boundaryMargin;

            for (var i = 0; i < _settings.MaxPlacementAttempts; i++)
            {
                var candidatePosition = new Vector3(_random.Range(minX, maxX), 0f, _random.Range(minZ, maxZ));
                if (IsClearOfActors(candidatePosition, actorRadius, actors) is false)
                    continue;

                var colliderCenter = candidatePosition + colliderOffset;
                if (IsObstacleFree(colliderCenter, actorRadius) is false)
                    continue;

                position = candidatePosition;
                return true;
            }

            return false;
        }

        private bool IsObstacleFree(Vector3 colliderCenter, float radius)
        {
            // Метод Unity API не помечен readonly; вызываем его на явной локальной копии структуры.
            var physicsScene = _physicsScene;

            var clearanceRadius = radius + _settings.ObstacleClearance;
            return physicsScene.OverlapSphere(colliderCenter, clearanceRadius, _overlaps, _obstacleLayerMask,
                QueryTriggerInteraction.Ignore) == 0;
        }

        private bool IsClearOfActors(Vector3 candidatePosition, float actorRadius, IReadOnlyList<Actor> actors)
        {
            foreach (var actor in actors)
            {
                var minimumDistance = actorRadius + actor.Radius + _settings.ActorSeparationPadding;
                var minimumDistanceSquared = minimumDistance * minimumDistance;
                var distanceSquared = (actor.Position - candidatePosition).sqrMagnitude;
                if (distanceSquared < minimumDistanceSquared)
                    return false;
            }

            return true;
        }
    }
}