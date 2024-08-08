# OCC Clone Prototipo

Este proyecto es un prototipo de OCC, consiste en varios microservicios desarrollados con .Net, es un **clon/demo**. 

El sistema permite gestionar: 
- usuarios
- roles
- publicaciones
  
## Estructura del Prototipo
```plaintext
OrchestrationService

PublicationsService
├── Applications
├── Controllers
├── Domain
├── Infrastructure
├── Modules
└── Persistence

RolesServices
├── Applications
├── Controllers
├── Domain
├── Infrastructure
├── Modules
└── Persistence

UsersService
├── Applications
│   ├── Commands
│   ├── DTO
│   ├── Queries
├── Controllers
├── Domain
│   ├── Core
│   ├── Entity
│   └── Interface
├── Infrastructure
│   ├── Interface
│   └── Repository
├── Modules
│   ├── Authentication
│   ├── Injection
│   ├── Mapper
│   └── Swagger
└── Persistence
    ├── Data
    └── Interface
```

## Tecnologías Utilizadas

- NET 8.0
- Ocelot
- SQL Server
