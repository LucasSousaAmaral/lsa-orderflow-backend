# LSA.OrderFlow — Desafio Técnico (C# / Clean Architecture / CQRS / DDD / SQL Server / MongoDB)

API RESTful para gerenciar **Pedidos (Orders)** de uma loja online, com:
- **Clean Architecture**
- **CQRS** (Commands no SQL / Queries no Mongo)
- **DDD**
- **SQL Server** como **write model** (EF Core)
- **MongoDB** como **read model**
- **Outbox Pattern** + Background Worker para projeções SQL → Mongo
- **Health check** (`/health`) e **Correlation ID** (`X-Correlation-Id`)

---

## 1) URLs (ambiente local)

Base URL da API (conforme Swagger informado):
- `https://localhost:7069`

Endpoints:
- **Swagger:** `https://localhost:7069/swagger`
- **Health:** `https://localhost:7069/health`
- **Orders:** `https://localhost:7069/api/orders`

---

## 2) Arquitetura

### Write path (SQL Server)
1. Request → Command (MediatR)
2. `Order` (aggregate) é alterado no Domínio
3. EF Core persiste `Orders` e `OrderItems`
4. Evento é gravado em `OutboxMessages`
5. Commit no SQL

### Read path (MongoDB)
1. `OutboxProcessor` (BackgroundService) lê `OutboxMessages`
2. `ProjectionDispatcher` projeta para Mongo
3. Mongo recebe/atualiza `orders_read`
4. Queries (`GET`) consultam o Mongo

---

## 3) Pré-requisitos

- **.NET SDK** (recomendado 8+)
- **SQL Server** (local ou remoto)
- **MongoDB** (local ou remoto)

---

## 4) Configuração

Edite: `LSA.OrderFlow.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "Sql": "Server=SEU_SERVIDOR_AQUI;Database=OrderFlowDb;User Id=sa;Password=SUA_SENHA_AQUI;TrustServerCertificate=True",
    "Mongo": "mongodb://localhost:27017"
  },
  "MongoDatabase": "orderflow"
}