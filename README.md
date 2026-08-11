# Arquitectura en Capas — API de Clientes

Ejemplo práctico desarrollado para el curso **Análisis de Sistemas II**.

Este proyecto muestra la evolución de una Web API para gestionar clientes y cómo, a medida que aparecen nuevas necesidades, se van separando responsabilidades mediante una **arquitectura en capas**.

El objetivo principal no es aprender a construir una API paso a paso, sino **observar cómo y por qué evoluciona su arquitectura**.

---

# 1. Objetivo del ejercicio

A partir de una API sencilla para consultar clientes, se analiza cómo separar progresivamente las responsabilidades de la aplicación:

```text
Controller
    ↓
Service
    ↓
Repository
    ↓
Datos
```

Posteriormente se introduce una abstracción mediante una interfaz:

```text
Controller
    ↓
Service
    ↓
IClienteRepository
    ↑
ClienteRepository
    ↓
Datos
```

Finalmente, los datos simulados son reemplazados por una base de datos real:

```text
Controller
    ↓
Service
    ↓
IClienteRepository
    ↑
ClienteRepository
    ↓
Dapper
    ↓
SQL Server
```

El ejercicio permite observar que la arquitectura **no aparece completa desde el principio**, sino que puede construirse progresivamente a partir de las responsabilidades que necesita el sistema.

---

# 2. Tecnologías utilizadas

* C#
* ASP.NET Core Web API
* .NET 10
* SQL Server
* Dapper
* Swagger / OpenAPI
* Visual Studio

---

# 3. Punto de partida: una API sencilla

La primera versión de la aplicación utiliza un Controller que contiene directamente los datos de los clientes.

Por ejemplo:

```csharp
private static readonly List<Cliente> clientes = new()
{
    new Cliente { Id = 1, Nombre = "Juan Pérez", Nit = "123456-7" },
    new Cliente { Id = 2, Nombre = "María López", Nit = "765432-1" }
};

[HttpGet]
public IActionResult Get()
{
    return Ok(clientes);
}
```

Conceptualmente tenemos:

```text
Controller
    ↓
Datos simulados
```

Esta solución funciona, pero el Controller está realizando varias responsabilidades:

* Recibir la petición HTTP.
* Ejecutar la lógica necesaria para obtener clientes.
* Conocer dónde están los datos.
* Devolver la respuesta.

Esto genera un primer problema de diseño: **el Controller sabe demasiado**.

---

# 4. Separación de responsabilidades

Para reducir la responsabilidad del Controller, se introduce una capa de servicios.

El flujo pasa a ser:

```text
Controller
    ↓
Service
    ↓
Datos
```

El Controller se encarga principalmente de atender la solicitud HTTP:

```csharp
[HttpGet]
public IActionResult Get()
{
    return Ok(_service.ObtenerTodos());
}
```

Mientras que el Service se ocupa de la operación relacionada con los clientes:

```csharp
public IEnumerable<Cliente> ObtenerTodos()
{
    return clientes;
}
```

Ahora existe una separación conceptual:

| Componente | Responsabilidad                   |
| ---------- | --------------------------------- |
| Controller | Atender solicitudes HTTP          |
| Service    | Coordinar la operación de negocio |
| Datos      | Proporcionar la información       |

---

# 5. Separación del acceso a datos

El siguiente problema aparece cuando observamos el Service.

El Service todavía conoce directamente cómo se obtienen los datos.

Para separar esta responsabilidad se introduce un Repository:

```text
Controller
    ↓
Service
    ↓
Repository
    ↓
Datos
```

El Repository concentra las operaciones relacionadas con el acceso a los datos.

Por ejemplo:

```csharp
public IEnumerable<Cliente> ObtenerTodos()
{
    return clientes;
}
```

El Service únicamente solicita los clientes:

```csharp
public IEnumerable<Cliente> ObtenerTodos()
{
    return _repository.ObtenerTodos();
}
```

La intención es que cada componente tenga una responsabilidad más clara.

---

# 6. Introducción de la interfaz

En este punto aparece otra cuestión importante.

Si el Service depende directamente de:

```csharp
ClienteRepository
```

entonces existe un acoplamiento entre el Service y una implementación concreta.

Para reducir este acoplamiento se introduce:

```csharp
IClienteRepository
```

La interfaz define el contrato:

```csharp
public interface IClienteRepository
{
    IEnumerable<Cliente> ObtenerTodos();
}
```

Y el Repository implementa dicho contrato:

```csharp
public class ClienteRepository : IClienteRepository
{
    public IEnumerable<Cliente> ObtenerTodos()
    {
        // implementación
    }
}
```

Ahora el Service depende de la abstracción:

```text
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
```

Esto es importante porque el Service ya no necesita conocer cuál es la implementación concreta que proporciona los datos.

---

# 7. De datos simulados a una base de datos real

Hasta este momento, el Repository todavía utiliza una lista en memoria.

Por ejemplo:

```csharp
private static readonly List<Cliente> clientes = new()
{
    new Cliente { Id = 1, Nombre = "Juan Pérez", Nit = "123456-7" },
    new Cliente { Id = 2, Nombre = "María López", Nit = "765432-1" }
};
```

Esta lista resulta útil para demostrar inicialmente la separación de responsabilidades, pero no representa un sistema real.

El siguiente paso consiste en reemplazar esta fuente de datos por:

```text
SQL Server
```

La ventaja de haber separado previamente el acceso a datos es que **no necesitamos modificar el Controller ni el Service**.

El cambio principal ocurre en el Repository.

---

# 8. SQL Server como nueva fuente de datos

Se crea una base de datos:

```sql
CREATE DATABASE ContabilidadDB;
```

Dentro de ella se crea la tabla:

```sql
CREATE TABLE Clientes
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Nit NVARCHAR(20) NOT NULL
);
```

Y se pueden agregar algunos registros:

```sql
INSERT INTO Clientes (Nombre, Nit)
VALUES
('Juan Pérez', '123456-7'),
('María López', '765432-1');
```

La estructura conceptual ahora es:

```text
SQL Server
    ↓
Tabla Clientes
```

---

# 9. Incorporación de Dapper

Para comunicar nuestra aplicación con SQL Server utilizamos **Dapper**.

Dapper permite ejecutar consultas SQL y mapear los resultados directamente hacia nuestros objetos de C#.

El Repository puede cambiar de una implementación basada en memoria:

```csharp
public IEnumerable<Cliente> ObtenerTodos()
{
    return clientes;
}
```

a una implementación basada en SQL:

```csharp
public IEnumerable<Cliente> ObtenerTodos()
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    using var connection = new SqlConnection(connectionString);

    var sql = "SELECT Id, Nombre, Nit FROM Clientes";

    return connection.Query<Cliente>(sql);
}
```

Aquí ocurre uno de los cambios más importantes del ejercicio.

**Antes:**

```text
ClienteRepository
    ↓
List<Cliente>
```

**Después:**

```text
ClienteRepository
    ↓
Dapper
    ↓
SQL Server
```

---

# 10. Configuración de la conexión

La conexión a la base de datos se configura mediante `appsettings.json`.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ContabilidadDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

La cadena puede variar dependiendo de la configuración de SQL Server utilizada.

El Repository obtiene esta configuración mediante `IConfiguration`.

Por ejemplo:

```csharp
private readonly IConfiguration _configuration;

public ClienteRepository(IConfiguration configuration)
{
    _configuration = configuration;
}
```

Esto permite que los detalles de conexión permanezcan fuera de la lógica del Controller y del Service.

---

# 11. El cambio arquitectónico

Es importante observar qué componentes **no tuvieron que cambiar**.

El flujo de la aplicación continúa siendo:

```text
Controller
    ↓
Service
    ↓
IClienteRepository
    ↑
ClienteRepository
```

Lo que cambió fue principalmente la implementación del Repository.

### Antes

```text
IClienteRepository
        ↑
ClienteRepository
        ↓
Lista en memoria
```

### Después

```text
IClienteRepository
        ↑
ClienteRepository
        ↓
     Dapper
        ↓
   SQL Server
```

Esto demuestra una de las ventajas de separar responsabilidades: **podemos cambiar la tecnología utilizada para almacenar y recuperar los datos sin tener que modificar toda la aplicación**.

---

# 12. Estructura final

La estructura del proyecto queda aproximadamente así:

```text
ContabilidadAPI
│
├── Controllers
│   └── ClientesController.cs
│
├── Models
│   └── Cliente.cs
│
├── Services
│   └── ClienteService.cs
│
├── Repositories
│   ├── IClienteRepository.cs
│   └── ClienteRepository.cs
│
├── Program.cs
├── appsettings.json
└── ...
```

Cada parte tiene una responsabilidad diferente:

| Elemento             | Responsabilidad                                 |
| -------------------- | ----------------------------------------------- |
| `ClientesController` | Recibir y responder solicitudes HTTP            |
| `ClienteService`     | Coordinar la operación relacionada con clientes |
| `IClienteRepository` | Definir el contrato de acceso a datos           |
| `ClienteRepository`  | Implementar el acceso a datos                   |
| `Cliente`            | Representar la información del cliente          |
| Dapper               | Facilitar la comunicación entre C# y SQL        |
| SQL Server           | Almacenar persistentemente los datos            |

---

# 13. Flujo completo

Cuando un usuario realiza:

```text
GET /api/Clientes
```

la solicitud recorre las capas:

```text
HTTP Request
     ↓
ClientesController
     ↓
ClienteService
     ↓
IClienteRepository
     ↓
ClienteRepository
     ↓
Dapper
     ↓
SQL Server
     ↓
Dapper
     ↓
ClienteRepository
     ↓
ClienteService
     ↓
ClientesController
     ↓
HTTP Response
```

El Controller no necesita saber cómo funciona SQL Server.

El Service tampoco necesita saber cómo se ejecuta la consulta SQL.

El Repository es el componente encargado de resolver esa parte de la infraestructura.

---

# 14. La idea arquitectónica detrás del ejemplo

El valor del ejercicio no está únicamente en conseguir que la API consulte una base de datos.

Lo importante es observar **cómo se distribuyen las responsabilidades**.

La aplicación comenzó de esta manera:

```text
Controller
    ↓
Datos
```

Y evolucionó hacia:

```text
Controller
    ↓
Service
    ↓
IClienteRepository
    ↑
ClienteRepository
    ↓
Dapper
    ↓
SQL Server
```

Cada nuevo elemento aparece para resolver una necesidad concreta:

```text
Controller
    ↓
Separar la atención HTTP

Service
    ↓
Separar la lógica de la presentación

Repository
    ↓
Separar el acceso a datos

Interface
    ↓
Reducir el acoplamiento con una implementación concreta

Dapper + SQL Server
    ↓
Proporcionar persistencia real
```

Por lo tanto, **agregar capas no debería hacerse simplemente porque una arquitectura dice que deben existir determinadas carpetas**.

La pregunta debe ser:

> **¿Qué responsabilidad estamos separando y qué problema de diseño estamos resolviendo?**

---

# 15. Preguntas para analizar

Después de ejecutar el ejemplo, analiza:

1. ¿Qué responsabilidades tenía inicialmente el Controller?

2. ¿Qué problema solucionó la creación del Service?

3. ¿Qué responsabilidad se trasladó al Repository?

4. ¿Por qué el Service depende de `IClienteRepository` y no directamente de `ClienteRepository`?

5. ¿Qué parte del sistema tuvo que modificarse para pasar de una lista en memoria a SQL Server?

6. ¿Por qué el Controller no necesitó modificarse al cambiar la fuente de datos?

7. Si reemplazáramos SQL Server por otra tecnología de persistencia, ¿qué componente tendría que cambiar principalmente?

8. ¿Qué ventajas y desventajas observas al introducir más capas?

9. ¿La existencia de Controllers, Services y Repositories significa automáticamente que estamos aplicando Clean Architecture?

---
