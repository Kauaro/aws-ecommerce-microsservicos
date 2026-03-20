using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.SQS;

public class SqsPublisher : IEventPublisher
{

	private readonly IAmazonSQS _sqsClient;


	private static readonly JsonSerializerSettings _jsonSettings = new()
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver()
	};



	public SqsPublisher(IAmazonSQS sqsClient)
	{
		_sqsClient = sqsClient;
	}



	public async Task PublicarAsync<T>(T evento, string queueUrl)
	{
		var mensagem = JsonConvert.SerializeObject(evento, _jsonSettings);

		var request = new SendMessageRequest
		{
			QueueUrl = queueUrl,
			MessageBody = mensagem
		};

		await _sqsClient.SendMessageAsync(request);
	}
}