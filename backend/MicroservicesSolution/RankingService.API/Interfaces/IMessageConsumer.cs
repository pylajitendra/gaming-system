namespace RankingService.API.Interfaces
{
    public interface IMessageConsumer
    {
        void Consume<T>(string queueName, Action<T> onMessageReceived);
    }
}