using Microsoft.AspNetCore.Mvc;
using ContabilidadAPI.Services;

namespace ContabilidadAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var clientes = _clienteService.ObtenerClientes();

        return Ok(clientes);
    }
}