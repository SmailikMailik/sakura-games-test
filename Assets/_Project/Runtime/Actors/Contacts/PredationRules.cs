namespace Project.Runtime.Actors.Contacts
{
    /// <summary>
    /// Проверяет возможность поедания по пищевой роли и порядку появления участников.
    /// </summary>
    internal static class PredationRules
    {
        /// <summary>
        /// Живой хищник может съесть добычу или хищника, появившегося позже него.
        /// </summary>
        internal static bool CanEat(ActorState eater, ActorState victim)
        {
            if (eater == victim || eater.IsAlive is false || victim.IsAlive is false)
                return false;

            if (eater.Role != FoodRole.Predator)
                return false;

            if (victim.Role == FoodRole.Prey)
                return true;

            return eater.SpawnOrder < victim.SpawnOrder;
        }
    }
}