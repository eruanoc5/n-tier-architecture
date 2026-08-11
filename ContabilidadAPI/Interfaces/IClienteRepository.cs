using ContabilidadAPI.Models;

namespace ContabilidadAPI.Interfaces;

public interface IClienteRepository
{
    List<Cliente> ObtenerTodos();

    List<Cliente> ObtenerHombres();

    List<Cliente> ObtenerMujeres();
}