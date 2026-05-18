namespace ScoreService.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}