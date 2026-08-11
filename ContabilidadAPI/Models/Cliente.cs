namespace ContabilidadAPI.Models;

public class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Nit { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Sexo { get; set; } = string.Empty;
}