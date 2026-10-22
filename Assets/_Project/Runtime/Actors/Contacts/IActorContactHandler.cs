namespace Project.Runtime.Actors.Contacts
{
    /// <summary>
    /// Принимает сообщения о столкновениях участников симуляции.
    /// </summary>
    internal interface IActorContactHandler
    {
        /// <summary>
        /// Обрабатывает контакт двух участников симуляции.
        /// </summary>
        void HandleContact(Actor first, Actor second);
    }
}