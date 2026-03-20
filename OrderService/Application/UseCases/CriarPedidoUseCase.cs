using Amazon.Runtime.EventStreams;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.UseCases
{

	public class CriarPedidoUseCase
	{
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IEventPublisher _eventPublisher;
		private readonly string _queueUrl;

        public CriarPedidoUseCase(
			IPedidoRepository pedidoRepository,
			IEventPublisher eventPublisher,
			IConfiguration configuration)
		{
            _pedidoRepository = pedidoRepository;
			_eventPublisher = eventPublisher;
			_queueUrl = configuration["AWS:OrderCreatedQueueUrl"];
        }


		public async Task<Pedido> ExecutarAsync(Pedido pedido)
		{
            //Salva no DynamoDB
            await _pedidoRepository.CriarAsync(pedido);

            //Publica evento no SQS
			var evento = new PedidoCriadoEvent
			{
				PedidoId = pedido.PedidoId,
				ClienteNome = pedido.ClienteNome,
				ClienteEmail = pedido.ClienteEmail,
				ValorTotal = pedido.ValorTotal,
			};

			await _eventPublisher.PublicarAsync(evento, _queueUrl);

			return pedido;
        }
    }
}