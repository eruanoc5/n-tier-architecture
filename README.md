Ejemplo práctico desarrollado para el curso Análisis de Sistemas II.

El proyecto muestra la construcción progresiva de una Web API para gestionar clientes, utilizando el enfoque de Arquitectura en Capas.

Tecnologías
C#
ASP.NET Core Web API
.NET 10
SQL Server
Dapper
Swagger / OpenAPI
Arquitectura

La aplicación organiza sus responsabilidades de la siguiente manera:

Controller → Service → IClienteRepository → ClienteRepository → SQL Server

Cada componente tiene una responsabilidad específica dentro de la solución.

Controller

Gestiona las solicitudes HTTP y las respuestas de la API. Su responsabilidad principal es recibir la petición y delegar el trabajo correspondiente.

Service

Contiene la lógica y coordinación de las operaciones del sistema. El Service no necesita conocer los detalles de cómo se almacenan los datos.

Repository

Se encarga del acceso a los datos. En este ejemplo, ClienteRepository implementa las operaciones necesarias para consultar la información de los clientes.

IClienteRepository

Define el contrato que debe cumplir el Repository.

El Service depende de esta abstracción en lugar de depender directamente de una implementación concreta.

Dapper

Se utiliza como herramienta de acceso a datos para ejecutar las consultas SQL y mapear los resultados hacia los objetos de C#.

SQL Server

Se utiliza como sistema de almacenamiento persistente para la información de los clientes.

Estructura del proyecto
ContabilidadAPI

├── Controllers
    
    └── ClientesController.cs

├── Models
    
    └── Cliente.cs

├── Services

    └── ClienteService.cs

├── Repositories
    
    ├── IClienteRepository.cs
    
    └── ClienteRepository.cs

├── Program.cs

└── appsettings.json

Evolución del ejemplo

El ejercicio se construyó de manera progresiva.

Inicialmente, el Controller trabajaba directamente con los datos. Posteriormente se incorporó un Service para separar la lógica de la API.

Después se incorporó un Repository, separando la responsabilidad relacionada con el acceso a datos.

Finalmente se introdujo IClienteRepository para que el Service dependiera de una abstracción y se configuró la inyección de dependencias.

La persistencia, que inicialmente podía representarse mediante datos simulados, se reemplazó posteriormente por SQL Server utilizando Dapper.

De esta manera, el flujo final de la aplicación es:

Solicitud HTTP → Controller → Service → IClienteRepository → ClienteRepository → SQL Server

Configuración de la base de datos

El proyecto contiene dos implementaciones del Repository:

ClienteRepository → utiliza datos simulados.

ClienteSqlRepository → utiliza SQL Server mediante Dapper.

La versión con datos simulados permite ejecutar el proyecto sin configurar una base de datos.

Si desea realizar una prueba con data real. En appsettings.json, configurar la cadena de conexión correspondiente a la instalación local de SQL Server.
En Program.cs se debe registrar solamente una implementación.

builder.Services.AddScoped<IClienteRepository, ClienteSqlRepository>();
