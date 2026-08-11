using Dapper;
using Microsoft.Data.SqlClient;
using ContabilidadAPI.Interfaces;
using ContabilidadAPI.Models;

namespace ContabilidadAPI.Repositories;

public class ClienteSqlRepository : IClienteRepository
{
    private readonly IConfiguration _configuration;

    public ClienteSqlRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Cliente> ObtenerHombres()
    {
        throw new NotImplementedException();
    }

    public List<Cliente> ObtenerMujeres()
    {
        throw new NotImplementedException();
    }

    public List<Cliente> ObtenerTodos()
    {
        using var connection = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));

        string sql = """
            SELECT
                Id,
                Nombre,
                Nit,
                Telefono
            FROM Clientes
            """;

        return connection.Query<Cliente>(sql).ToList();
    }
}