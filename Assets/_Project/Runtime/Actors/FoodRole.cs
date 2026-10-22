namespace Project.Runtime.Actors
{
    /// <summary>
    /// Роль участника в пищевой цепи: добыча или хищник.
    /// </summary>
    internal enum FoodRole : byte
    {
        Prey = 0,
        Predator = 1
    }
}