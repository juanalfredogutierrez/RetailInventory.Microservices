# JMCloudLab.RetailInventoryPlatform

## Descripción General

JMCloudLab.RetailInventoryPlatform es una plataforma de gestión de inventario desarrollada bajo una arquitectura de microservicios.

La solución permite gestionar productos, compras, ventas e inventario, incorporando autenticación mediante JWT, comunicación asíncrona mediante RabbitMQ y un API Gateway centralizado con Ocelot.

El objetivo principal fue implementar una arquitectura escalable, desacoplada y alineada con buenas prácticas de desarrollo empresarial utilizando .NET y Angular.

---

# Arquitectura de la Solución

La solución está compuesta por los siguientes componentes:

```text
Angular Frontend
        │
        ▼
API Gateway (Ocelot)
        │
 ┌──────┼─────────────┐
 ▼      ▼             ▼
Auth   Producto    Compra
Service Service    Service
        │
        ▼
 InventarioService
        │
        ▼
     SQL Server
```

La comunicación entre servicios se realiza mediante APIs REST y eventos publicados a través de RabbitMQ.

---

# Tecnologías Utilizadas

## Backend

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* MediatR
* CQRS
* RabbitMQ
* JWT Authentication
* Ocelot API Gateway

## Frontend

* Angular 21
* Standalone Components
* Angular Signals
* Reactive Forms
* Route Guards
* HTTP Interceptors

---

# Patrones y Principios Aplicados

## Clean Architecture

La solución backend se encuentra organizada en capas:

```text
API
Application
Domain
Infrastructure
```

Beneficios:

* Separación de responsabilidades
* Mayor mantenibilidad
* Escalabilidad
* Facilidad para realizar pruebas

---

## CQRS

Se implementó el patrón CQRS utilizando MediatR.

Ejemplos:

### Commands

```text
CreateProductoCommand
RegistrarCompraCommand
RegistrarVentaCommand
```

### Queries

```text
GetProductosQuery
```

---

## Comunicación Asíncrona

RabbitMQ es utilizado para desacoplar procesos entre servicios.

Ejemplo:

```text
Compra Registrada
       │
       ▼
RabbitMQ
       │
       ▼
InventarioService
       │
       ▼
Actualización de Stock
```

---

# Microservicios Implementados

## AuthService

Responsabilidades:

* Autenticación de usuarios
* Generación de JWT
* Validación de credenciales

Funcionalidades:

* Login
* Generación de Token JWT
* Inclusión de información de usuario y rol dentro del token

---

## ProductoService

Responsabilidades:

* Gestión de productos

Funcionalidades implementadas:

* Registro de productos
* Consulta de productos

---

## CompraService

Responsabilidades:

* Registro de compras

Funcionalidades implementadas:

* Registro de compras
* Publicación de eventos hacia RabbitMQ

---

## InventarioService

Responsabilidades:

* Gestión de stock
* Procesamiento de eventos

Funcionalidades implementadas:

* Actualización de stock por compras
* Actualización de stock por ventas

---

# API Gateway

Se implementó Ocelot como punto único de entrada.

Responsabilidades:

* Enrutamiento de solicitudes
* Seguridad
* Centralización de acceso a microservicios

---

# Seguridad

La autenticación fue implementada mediante JWT.

Características:

* Login protegido
* Auth Guard
* HTTP Interceptor
* Validación de token
* Redirección automática al Login cuando el token expira

---

# Funcionalidades Frontend

## Login

Implementado:

* Inicio de sesión mediante JWT
* Almacenamiento de token
* Protección de rutas
* Manejo de expiración de sesión

---

## Dashboard

Implementado:

* Layout principal
* Sidebar de navegación
* Topbar

---

## Módulo de Productos

Implementado:

* Listado de productos
* Registro de productos

---

## Módulo de Compras

Implementado:

* Registro de compras
* Selección de productos
* Integración con inventario

---

## Módulo de Ventas

Implementado:

* Registro de ventas
* Integración con inventario

---

# Flujo de Inventario

## Flujo de Compra

```text
Compra
   │
   ▼
RabbitMQ
   │
   ▼
InventarioService
   │
   ▼
Stock Actualizado
```

## Flujo de Venta

```text
Venta
   │
   ▼
RabbitMQ
   │
   ▼
InventarioService
   │
   ▼
Stock Actualizado
```

---

# Estructura del Proyecto

## Backend

```text
src
│
├── ApiGateway
├── AuthService
├── ProductoService
├── CompraService
├── InventarioService
└── BuildingBlocks
```

## Frontend

```text
JMCloudLab.RetailInventoryPlatform.Web
│
├── core
├── features
│   ├── auth
│   ├── dashboard
│   ├── productos
│   ├── compras
│   └── ventas
│
├── layout
└── shared
```

---

# Ejecución de la Solución

## Requisitos Previos

* .NET 8 SDK
* Node.js 22+
* Angular CLI
* SQL Server
* RabbitMQ

---

## Backend

Restaurar paquetes:

```bash
dotnet restore
```

Ejecutar los servicios:

```bash
dotnet run
```

---

## Frontend

Instalar dependencias:

```bash
npm install
```

Ejecutar la aplicación:

```bash
ng serve
```

Acceso:

```text
http://localhost:4200
```

---

# Credenciales de Prueba

```text
Usuario: admin
Contraseña: Admin123*
```

---

# Consideraciones

Debido al tiempo limitado definido para la evaluación técnica, se priorizó la implementación de la arquitectura, la integración entre microservicios, la seguridad mediante JWT y los flujos funcionales de negocio (Productos, Compras y Ventas).

---

# Mejoras Futuras

* Pruebas Unitarias
* Pruebas de Integración
* Actualización y eliminación de productos
* Dashboard con métricas en tiempo real
* Docker Compose
* CI/CD
* Logging centralizado
* Monitoreo y observabilidad

---

# Autor

Juan Gutierrez

Desarrollado como solución para evaluación técnica utilizando .NET, Angular, RabbitMQ y arquitectura basada en microservicios.
