# Gobernanza Digital

API REST para una municipalidad: genera las boletas de cobro de los servicios municipales y controla cuáles se pagaron, consultando a una pasarela de pagos externa (PayLink).

Trabajo práctico de Programación 2. Grupo: Cristian, Agostina y Franco.

## Tecnologías
- C# con .NET 10 y ASP.NET Core Web API
- Entity Framework Core con SQLite
- Swagger para probar los endpoints

## Cómo ejecutarlo
1. Abrir `GobernanzaDigital/GobernanzaDigital.slnx` en Visual Studio.
2. Elegir el perfil **https** y apretar F5.
3. Se abre Swagger en `https://localhost:7233/swagger`. La base de datos (`gobernanza.db`) se crea sola la primera vez.

## Flujo básico
1. `POST /api/contribuyentes` y `POST /api/servicios`: cargar los datos.
2. `POST /api/asignaciones`: asignar un servicio a un contribuyente.
3. `POST /api/generaciones` con `{ "periodo": "2026/11" }`: genera las boletas.
4. `POST /api/pasarelas`: cargar la pasarela de pagos (URL base y API Key).
5. `POST /api/conciliaciones` con `{ "pasarelaId": 1 }`: consulta los pagos y marca las boletas pagadas.
6. `GET /api/contribuyentes/{id}/boletas`: ver las boletas (filtros opcionales `estado` y `periodo`).

## Probar sin PayLink
En Development hay un simulador de PayLink en `/mock-paylink`. Se carga una pasarela con `urlBase` `https://localhost:7233/mock-paylink` y `apiKey` `clave-demo`. Según el último carácter del código de pago responde: 0-7 aprobado, 8-9 pendiente, A-C rechazado, D-F sin pagos.

## Organización del código
`Controllers` (HTTP), `Services` e `Interfaces` (lógica), `Models` (tablas), `DTOs` (datos de entrada y salida), `Clients` (conexión con PayLink), `Data` (Entity Framework), `Exceptions` y `Middleware` (errores).

La documentación completa (base de datos y endpoints) está en la carpeta `docs`.
