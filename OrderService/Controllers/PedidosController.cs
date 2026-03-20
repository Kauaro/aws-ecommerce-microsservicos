using Microsoft.AspNetCore.Mvc;
using OrderService.Application.UseCases;
using OrderService.Domain.Entities;

namespace OrderService.Controllers;


    [ApiController]
    [Route ("pedidos")]
    public class PedidosController : ControllerBase
    {


        private readonly CriarPedidoUseCase _criarPedidoUseCase;

        public PedidosController(CriarPedidoUseCase criarPedidoUseCase)
        {
            _criarPedidoUseCase = criarPedidoUseCase;
        }


        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] Pedido pedido)
        {
            var resultado = await _criarPedidoUseCase.ExecutarAsync(pedido);
            return CreatedAtAction(nameof(CriarPedido), new { id = resultado.PedidoId }, resultado);
        }        

    }

