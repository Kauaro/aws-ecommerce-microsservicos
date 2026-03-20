using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Infrastructure.DynamoDB;

public class PagamentoRepository : IPagamentoRepository
{

    private readonly ITable _table;


    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };



    public PagamentoRepository(IAmazonDynamoDB dynamoDbClient)
    {
        _table = new TableBuilder(dynamoDbClient, "Pagamentos")
            .AddHashKey("pedidoId", DynamoDBEntryType.String)
            .Build();
    }


    public async Task<Pagamento?> BuscarPorPedidoIdAsync(string pedidoId)
    {
        var document = await _table.GetItemAsync(pedidoId);
        if (document == null) return null;
        return JsonConvert.DeserializeObject<Pagamento>(document.ToJson(), _jsonSettings);
    }


    public async Task SalvarAsync(Pagamento pagamento)
    {
        var json = JsonConvert.SerializeObject(pagamento, _jsonSettings);
        var document = Document.FromJson(json);
        await _table.PutItemAsync(document);
    }
}