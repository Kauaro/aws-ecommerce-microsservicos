using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.DynamoDB;

public class PedidoRepository : IPedidoRepository
{
	private readonly IAmazonDynamoDB _dynamoDbClient;
	private readonly ITable _table;

	private static readonly JsonSerializerSettings _jsonSettings = new()
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver()
	};

	public PedidoRepository(IAmazonDynamoDB dynamoDbClient)
	{
		_dynamoDbClient = dynamoDbClient;
		_table = new TableBuilder(dynamoDbClient, "Pedidos")
			.AddHashKey("pedidoId", DynamoDBEntryType.String)
			.Build();
	}

	public async Task<Pedido> CriarAsync(Pedido pedido)
	{
		var json = JsonConvert.SerializeObject(pedido, _jsonSettings);
		var document = Document.FromJson(json);
		await _table.PutItemAsync(document);
		return pedido;
	}

	public async Task<Pedido?> BuscarPorIdAsync(string pedidoId)
	{
		var document = await _table.GetItemAsync(pedidoId);
		if (document == null) return null;
		return JsonConvert.DeserializeObject<Pedido>(document.ToJson(), _jsonSettings);
	}
}