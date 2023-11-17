using Inveon.MessageBus;

namespace Inveon.Services.OrderAPI.RabbitMQ.Interfaces
{
    public interface IRabbitMQOrderMessageSender
    {
        void SendMessage(BaseMessage baseMessage, String queueName);
    }
}
