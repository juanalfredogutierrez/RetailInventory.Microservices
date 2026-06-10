# RetailInventory.Microservices

## Descripción

Plataforma de gestión de inventario desarrollada bajo una arquitectura basada en microservicios utilizando .NET 8, RabbitMQ, SQL Server y Ocelot.

La solución implementa autenticación JWT, gestión de productos, compras, ventas y actualización de inventario mediante comunicación asíncrona basada en eventos.

---

# Arquitectura General

```text
┌─────────────────────────────┐
│      Angular Frontend       │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│    API Gateway (Ocelot)     │
└──────────────┬──────────────┘
               │
     ┌─────────┼─────────┐
     │         │         │
     ▼         ▼         ▼

┌─────────┐ ┌─────────┐ ┌─────────┐
│  Auth   │ │Producto │ │ Compra  │
│ Service │ │ Service │ │ Service │
└─────────┘ └─────────┘ └────┬────┘
                             │
                             ▼

                    ┌────────────────┐
                    │ InventarioSvc  │
                    └───────┬────────┘
                            │
                            ▼

                     ┌────────────┐
                     │ RabbitMQ   │
                     └─────┬──────┘
                           │
                           ▼

                     ┌────────────┐
                     │ SQL Server │
                     └────────────┘
```

---

# Tecnologías

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* MediatR
* CQRS
* RabbitMQ
* JWT Authentication
* Ocelot API Gateway
* Docker
* Docker Compose

---

# Patrones Implementados

## Clean Architecture

```text
API
Application
Domain
Infrastructure
```

## CQRS

Separación de comandos y consultas mediante MediatR.

Ejemplos:

```text
Commands
- CreateProductoCommand
- RegistrarCompraCommand
- RegistrarVentaCommand

Queries
- GetProductosQuery
```

## Event Driven Architecture

Comunicación desacoplada entre servicios mediante RabbitMQ.

---

# Microservicios

## AuthService

* Autenticación de usuarios
* Generación de JWT
* Validación de credenciales

## ProductoService

* Registro de productos
* Consulta de productos

## CompraService

* Registro de compras
* Publicación de eventos

## InventarioService

* Gestión de inventario
* Actualización de stock
* Consumo de eventos RabbitMQ

## ApiGateway

* Enrutamiento centralizado
* Seguridad
* Punto único de acceso

---

# Flujo de Compra

```text
Usuario
   │
   ▼

CompraService
   │
   ▼

RabbitMQ
   │
   ▼

InventarioService
   │
   ▼

SQL Server
```

---

# Flujo de Venta

```text
Usuario
   │
   ▼

VentaService
   │
   ▼

RabbitMQ
   │
   ▼

InventarioService
   │
   ▼

SQL Server
```

---

# Docker Compose

La solución puede ejecutarse utilizando Docker Compose.

Servicios incluidos:

* ApiGateway
* AuthService
* ProductoService
* CompraService
* InventarioService
* SQL Server
* RabbitMQ

## Ejecutar

```bash
docker compose up -d
```

## Detener

```bash
docker compose down
```

---
# Scripts de Base de Datos

La solución incluye los scripts de creación de base de datos correspondientes a cada microservicio.

database/
│
├── AuthDb.sql
├── ProductoDb.sql
├── CompraDb.sql
└── InventarioDb.sql

---
## Documentación

La solución incluye documentación interactiva mediante Swagger.

Debido a las restricciones de tiempo de la evaluación técnica, no se incluyó una colección Postman exportada, aunque todos los endpoints pueden ser explorados y ejecutados desde Swagger.
Acceso:

- AuthService: /swagger
- ProductoService: /swagger
- CompraService: /swagger
- InventarioService: /swagger
  
# Mejoras Futuras

* Pruebas Unitarias
* Pruebas de Integración
* Observabilidad
* Logging centralizado
* CI/CD
* Monitoreo

# Autor

**Juan Gutierrez**

Desarrollador .NET / Angular

<img width="440" height="557" alt="image" src="https://github.com/user-attachments/assets/33037e1c-1355-4157-8b4c-5ad8c93dc236" />


<img width="1352" height="829" alt="image" src="https://github.com/user-attachments/assets/b033d50c-fda4-40c5-be8b-825756231a4f" />

