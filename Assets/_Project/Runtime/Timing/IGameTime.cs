namespace Project.Runtime.Timing
{
    /// <summary>
    /// Предоставляет игровое время приложения и управление его общим масштабом.
    /// </summary>
    internal interface IGameTime
    {
        double TimeAsDouble { get; }
        float DeltaTime { get; }
        float TimeScale { get; set; }
    }
}