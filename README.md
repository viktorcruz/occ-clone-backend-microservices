# OCC Clone Prototipo

Este proyecto es un prototipo de OCC, consiste en varios microservicios desarrollados con .Net, es un **clon/demo**. 

El sistema permite gestionar: 
- usuarios
- publicaciones
- trabajos
  
## Estructura del Prototipo
```plaintext

AuthService
├── Applications
├── Controllers
├── Domain
├── Infrastructure
├── Modules
└── Factories

PublicationsService
├── Applications
├── Controllers
├── Domain
├── Infrastructure
├── Modules
└── Persistence

SearchJobsServices
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
│   ├── Services
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
├── Persistence
|   ├── Data
|   └── Interface
└── Saga
    ├── CompensationActions
    ├── SagaHandler
    └── SagaState
```

## Tecnologías Utilizadas

- NET 8.0
- RabbitMQ
- Ocelot
- SQL Server
