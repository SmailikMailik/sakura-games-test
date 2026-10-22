using UnityEngine;

namespace Project.Runtime.Space
{
    /// <summary>
    /// Помечает препятствие для разворота акторов и задаёт имя слоя, запрещающего спавн.
    /// </summary>
    [AddComponentMenu("Zoo/World Obstacle")]
    [DisallowMultipleComponent]
    internal sealed class WorldObstacle : MonoBehaviour
    {
        internal const string LayerName = "Obstacles";
    }
}