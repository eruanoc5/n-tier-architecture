using ContabilidadAPI.Interfaces;
using ContabilidadAPI.Models;

namespace ContabilidadAPI.Repositories;

public class ClienteRepository : IClienteRepository
{
    public List<Cliente> ObtenerTodos()
    {
        return
        [
            new Cliente
            {
                Id = 1,
                Nombre = "Juan",
                Nit = "123456-7",
                Telefono = "5555-5555",
                Sexo = "M"
            },

            new Cliente
            {
                Id = 1,
                Nombre = "Juana",
                Nit = "123456-7",
                Telefono = "5555-5555",
                Sexo = "F"
            },
        ];
    }

    public List<Cliente> ObtenerHombres()
    {
        return
        [
            new Cliente
            {
                Id = 1,
                Nombre = "Juan",
                Nit = "123456-7",
                Telefono = "5555-5555",
                Sexo = "M"
            }
        ];
    }

    public List<Cliente> ObtenerMujeres()
    {
        return
        [
            new Cliente
            {
                Id = 1,
                Nombre = "Juana",
                Nit = "123456-7",
                Telefono = "5555-5555",
                Sexo = "F"
            }
        ];
    }
}