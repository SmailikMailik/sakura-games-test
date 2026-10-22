namespace Project.Runtime.Randomness
{
    /// <summary>
    /// Предоставляет случайные значения без доступа к глобальному состоянию Unity.
    /// </summary>
    internal interface IRandomSource
    {
        /// <summary>
        /// Возвращает целое значение от minimum включительно до maximum исключительно.
        /// </summary>
        int Range(int minimum, int maximum);

        /// <summary>
        /// Возвращает вещественное значение между minimum и maximum.
        /// </summary>
        float Range(float minimum, float maximum);
    }
}