using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PaymentService.Infrastructure.SQS;

public class SqsConsumer
{

    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;


    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };


    public SqsConsumer(IAmazonSQS sqsClient, IConfiguration configuration)
    {
        _sqsClient = sqsClient;
        _queueUrl = configuration["AWS:OrderCreatedQueueUrl"]!;
    }


    public async Task<(T? mensagem, string receiptHandle)> ReceberAsync<T>()
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 5
        };

        var response = await _sqsClient.ReceiveMessageAsync(request);

        if (response.Messages == null || !response.Messages.Any())
            return (default, string.Empty);

        var message = response.Messages[0];
        var objeto = JsonConvert.DeserializeObject<T>(message.Body, _jsonSettings);
        return (objeto, message.ReceiptHandle);
    }

    public async Task DeletarMensagemAsync(string receiptHandle)
    {
        await _sqsClient.DeleteMessageAsync(_queueUrl, receiptHandle);
    }
}