using UnityEngine;

namespace Project.Runtime.Actors.Movement.Body
{
    /// <summary>
    /// Задаёт восстановление скорости акторов после физических столкновений.
    /// </summary>
    [CreateAssetMenu(menuName = "Zoo/Actor physics settings")]
    internal sealed class ActorPhysicsSettings : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _recoveryAcceleration = 8f;

        internal float RecoveryAcceleration => _recoveryAcceleration;
    }
}