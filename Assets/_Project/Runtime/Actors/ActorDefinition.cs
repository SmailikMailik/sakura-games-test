using Project.Runtime.Actors.Movement.Profiles;
using UnityEngine;

namespace Project.Runtime.Actors
{
    /// <summary>
    /// Описывает тип участника симуляции: его prefab, пищевую роль и профиль движения.
    /// </summary>
    [CreateAssetMenu(menuName = "Zoo/Actor definition")]
    internal sealed class ActorDefinition : ScriptableObject
    {
        [SerializeField] private FoodRole _role;
        [SerializeField] private Actor _prefab;
        [SerializeField] private MotionProfile _motion;

        internal FoodRole Role => _role;
        internal Actor Prefab => _prefab;
        internal MotionProfile Motion => _motion;
    }
}