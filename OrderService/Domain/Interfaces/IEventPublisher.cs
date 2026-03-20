namespace OrderService.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublicarAsync<T>(T evento, string queueUrl);
}