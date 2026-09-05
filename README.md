# Devsu Banking

Implementación de la prueba técnica de Devsu con .NET 10, PostgreSQL y RabbitMQ.

## Arquitectura

La solución contiene dos microservicios independientes:

- `Clients.Api`: administra `Person` y `Client`, incluyendo el CRUD completo.
- `Accounts.Api`: administra `Account`, `Transaction` y el reporte de estado de cuenta. Las cuentas y los movimientos no tienen `DELETE`; los movimientos son inmutables.

Cada servicio tiene sus propias capas `Api`, `Application`, `Domain` e `Infrastructure`. Cada uno utiliza una base de datos PostgreSQL independiente. `clientId` se conserva como referencia lógica y no existe una foreign key entre bases de datos.

Los cambios de clientes se publican como el evento `client.changed` en RabbitMQ. El servicio de cuentas consume el evento y mantiene una proyección local idempotente. Esto evita llamadas síncronas entre microservicios y permite escalar cada servicio de forma independiente.

### Consistencia eventual

La proyección local de clientes puede tardar unos instantes en actualizarse después de crear o editar un cliente. Es una decisión intencional: el reporte puede mostrar temporalmente información desactualizada mientras el consumidor procesa el evento. El saldo y el movimiento, en cambio, se actualizan en una transacción local única.

### PostgreSQL

Se eligió PostgreSQL porque ofrece transacciones ACID, `numeric(18,2)` para importes monetarios, índices y restricciones de integridad maduros, una imagen oficial pequeña para Docker y buena integración con EF Core mediante Npgsql. La separación de bases de datos mantiene los límites de los microservicios y evita acoplamiento físico.

## Requisitos

- Docker Desktop con Compose.
- .NET SDK 10 para ejecutar localmente y correr pruebas.

## Ejecutar en menos de diez minutos

1. Opcionalmente copie `.env.example` a `.env` y cambie las credenciales.
2. Levante toda la solución:

```bash
docker compose up --build
```

Las APIs estarán disponibles en:

- Clientes: `http://localhost:8081`
- Cuentas: `http://localhost:8082`
- Swagger de clientes: `http://localhost:8081/swagger`
- Swagger de cuentas: `http://localhost:8082/swagger`
- RabbitMQ: `http://localhost:15672`

La aplicación crea las tablas y carga datos de ejemplo automáticamente al iniciar. `BaseDatos.sql` contiene el esquema SQL explícito para revisión o despliegues controlados.

Para detener y eliminar también los datos locales:

```bash
docker compose down -v
```

## Endpoints

Las rutas están versionadas bajo `/api/v1`.

### Clientes

- `GET /api/v1/clientes`
- `GET /api/v1/clientes/{id}`
- `POST /api/v1/clientes`
- `PUT /api/v1/clientes/{id}`
- `PATCH /api/v1/clientes/{id}`
- `DELETE /api/v1/clientes/{id}`

La contraseña de entrada nunca se persiste en texto plano: se almacena como hash BCrypt y nunca se retorna en una respuesta.

### Cuentas

- `GET /api/v1/cuentas?clientId={clientId}`
- `GET /api/v1/cuentas/{id}`
- `POST /api/v1/cuentas`
- `PUT /api/v1/cuentas/{id}`
- `PATCH /api/v1/cuentas/{id}`

### Movimientos

- `POST /api/v1/movimientos/{accountId}`

El valor se envía siempre positivo y el campo `type` determina si es `Deposit` o `Withdrawal`. Para reintentos se recomienda enviar `idempotencyKey`. Un retiro sin fondos responde `422` con el mensaje `Saldo no disponible`.

### Reportes

```text
GET /api/v1/reportes?cliente={clientId}&fechaInicio=2022-02-01&fechaFin=2022-02-28
```

La respuesta incluye las cuentas asociadas, su saldo actual y el detalle de movimientos dentro del rango inclusivo. Las fechas usan ISO 8601.

## Errores

Las APIs usan un middleware global y devuelven `application/problem+json`:

```json
{
  "title": "Regla de negocio incumplida",
  "status": 422,
  "detail": "Saldo no disponible",
  "traceId": "..."
}
```

- `400`: entrada o dominio inválido.
- `404`: recurso inexistente.
- `409`: conflicto, por ejemplo identificación o número de cuenta duplicado.
- `422`: regla de negocio incumplida.
- `500`: error inesperado.

## Pruebas

Ejecute todas las pruebas con un solo comando:

```bash
dotnet test DevsuBanking.slnx
```

Incluye pruebas unitarias de `Client` y `Account`, más una prueba de integración con EF Core que verifica la persistencia conjunta del saldo y el movimiento.

## Decisiones técnicas

- Las entidades no se exponen directamente: las APIs usan DTOs.
- El saldo se actualiza junto con el movimiento y se conserva el saldo anterior y posterior para auditoría.
- Los movimientos no se editan ni se eliminan; una corrección debe registrarse como un movimiento compensatorio.
- Se usa `decimal(18,2)` para dinero.
- EF Core aplica índices únicos para `client_id`, identificación y número de cuenta.
- La concurrencia de cuenta se controla mediante la columna de versión configurada como `rowversion` lógica en EF Core.
- La clave de idempotencia de movimientos es única cuando está presente.
- La semilla es idempotente y usa contraseñas hash.

## Colección Postman

Importe `postman/DevsuBanking.postman_collection.json`. Ajuste `accountId` con el identificador retornado al crear o consultar una cuenta.
