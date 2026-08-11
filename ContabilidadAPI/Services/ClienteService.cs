using ContabilidadAPI.Interfaces;
using ContabilidadAPI.Models;

namespace ContabilidadAPI.Services;

public class ClienteService
{
    private readonly IClienteRepository _repository;

    private readonly string _query = "M";

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public List<Cliente> ObtenerClientes()
    {
        switch (_query)
        {
            case "M":
                return _repository.ObtenerHombres();

            case "F":
                return _repository.ObtenerMujeres();

            default:
                return _repository.ObtenerTodos();
        }
    }
}