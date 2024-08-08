# OCC Clone Prototipo

Este proyecto es un prototipo de OCC, consiste en varios microservicios desarrollados con .Net. El sistema permite gestionar: 
- usuarios
- roles
- publicaciones

  
## Estructura del Prototipo
```plaintext
UsuariosService
├── Applications
│   ├── Commands
│   ├── DTO
│   ├── Queries
├── Controllers
├── Database
│   ├── Migrations
│   └── SeedData
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
│   ├── Data
│   └── Interface
└── Program.cs
```
