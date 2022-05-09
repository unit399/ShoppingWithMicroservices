using MassTransit;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutConsumer>
    {
        public Task Consume(ConsumeContext<BasketCheckoutConsumer> context)
        {
            throw new NotImplementedException();
        }
    }
}
