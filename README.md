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

Base URL da API:
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
- (Opcional) **SSMS/Azure Data Studio** e **MongoDB Compass**

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
```

---

## 5) Banco de dados (EF Core) — Migrations

Execute os comandos no root do repositório (onde estão as pastas `LSA.OrderFlow.Api` e `LSA.OrderFlow.Infrastructure`).

### 5.1) Aplicar migrations no banco
```bash
dotnet ef database update \
  --project .\LSA.OrderFlow.Infrastructure\LSA.OrderFlow.Infrastructure.csproj \
  --startup-project .\LSA.OrderFlow.Api\LSA.OrderFlow.Api.csproj \
  --context OrderFlowDbContext
```

### 5.2) Reset completo (opcional)
```bash
dotnet ef database drop -f \
  --project .\LSA.OrderFlow.Infrastructure\LSA.OrderFlow.Infrastructure.csproj \
  --startup-project .\LSA.OrderFlow.Api\LSA.OrderFlow.Api.csproj \
  --context OrderFlowDbContext
```

Depois:
```bash
dotnet ef database update \
  --project .\LSA.OrderFlow.Infrastructure\LSA.OrderFlow.Infrastructure.csproj \
  --startup-project .\LSA.OrderFlow.Api\LSA.OrderFlow.Api.csproj \
  --context OrderFlowDbContext
```

### 5.3) Criar migration nova (se necessário)
```bash
dotnet ef migrations add <MigrationName> \
  --project .\LSA.OrderFlow.Infrastructure\LSA.OrderFlow.Infrastructure.csproj \
  --startup-project .\LSA.OrderFlow.Api\LSA.OrderFlow.Api.csproj \
  --context OrderFlowDbContext \
  --output-dir Sql\Migrations
```

---

## 6) Seed do desafio (Customer fixo + 3 produtos fixos)

IDs fixos:

### Customer
- `11111111-1111-1111-1111-111111111111`

### Products
- `22222222-2222-2222-2222-222222222221` — Keyboard — 199.90 BRL  
- `22222222-2222-2222-2222-222222222222` — Mouse — 79.90 BRL  
- `22222222-2222-2222-2222-222222222223` — Headset — 249.90 BRL  

Validação rápida no SQL:
```sql
SELECT Id, Name, Email, Phone FROM Customers;
SELECT Id, Name, UnitPrice, Currency FROM Products;
```

---

## 7) Rodando a API

```bash
dotnet run --project .\LSA.OrderFlow.Api\LSA.OrderFlow.Api.csproj
```

Swagger:
- `https://localhost:7069/swagger`

---

## 8) Health check

```bash
curl -k https://localhost:7069/health
```

---

## 9) Correlation ID

Header suportado:
- `X-Correlation-Id`

Exemplo:
```bash
curl -k -i -H "X-Correlation-Id: demo-123" https://localhost:7069/health
```

---

## 10) Contratos oficiais (Swagger)

### 10.1) CreateOrderCommand
```json
{
  "customerId": "uuid",
  "items": [
    { "productId": "uuid", "quantity": 1 }
  ]
}
```

### 10.2) UpdateOrderCommand
```json
{
  "orderId": "uuid",
  "newStatus": 0,
  "replaceItems": [
    { "productId": "uuid", "quantity": 1 }
  ]
}
```

---

## 11) Endpoints

- `POST   /api/orders`
- `GET    /api/orders?page=1&pageSize=20&search=Pending`
- `GET    /api/orders/{id}`
- `PUT    /api/orders/{id}`
- `DELETE /api/orders/{id}`

---

## 12) Teste completo (copiar e colar)

Os exemplos abaixo usam `curl` com `-k` (ignorar certificado local).

### 12.1) Criar pedido
```bash
curl -k -X POST "https://localhost:7069/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "11111111-1111-1111-1111-111111111111",
    "items": [
      { "productId": "22222222-2222-2222-2222-222222222221", "quantity": 2 },
      { "productId": "22222222-2222-2222-2222-222222222222", "quantity": 1 }
    ]
  }'
```

### 12.2) Listar pedidos
```bash
curl -k "https://localhost:7069/api/orders?page=1&pageSize=20"
```

Com filtro:
```bash
curl -k "https://localhost:7069/api/orders?page=1&pageSize=20&search=Pending"
```

### 12.3) Detalhar pedido
Substitua `<ORDER_ID>` por um id retornado na listagem.

```bash
curl -k "https://localhost:7069/api/orders/<ORDER_ID>"
```

### 12.4) Atualizar pedido (status)
Substitua `<ORDER_ID>`.

```bash
curl -k -X PUT "https://localhost:7069/api/orders/<ORDER_ID>" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "<ORDER_ID>",
    "newStatus": 2
  }'
```

### 12.5) Atualizar pedido (substituir itens)
Substitua `<ORDER_ID>`.

```bash
curl -k -X PUT "https://localhost:7069/api/orders/<ORDER_ID>" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "<ORDER_ID>",
    "replaceItems": [
      { "productId": "22222222-2222-2222-2222-222222222223", "quantity": 1 }
    ]
  }'
```

### 12.6) Remover pedido
Substitua `<ORDER_ID>`.

```bash
curl -k -X DELETE "https://localhost:7069/api/orders/<ORDER_ID>"
```

---

## 13) Verificação do Outbox (SQL)

Verificar mensagens e projeção:
```sql
SELECT TOP 20
    Id, Type, OccurredOnUtc, ProcessedOnUtc, RetryCount, NextAttemptOnUtc, Error
FROM OutboxMessages
ORDER BY OccurredOnUtc DESC;
```

---

## 14) Verificação do Read Model (Mongo)

Database:
- `orderflow` (config `MongoDatabase`)

Collection:
- `orders_read`

---

## 15) Estrutura do repositório

- `LSA.OrderFlow.Api` — Controllers, Middleware, Health, Swagger
- `LSA.OrderFlow.Application` — Commands/Queries/Handlers/DTOs/Validators
- `LSA.OrderFlow.Domain` — Entidades/VOs/Regras
- `LSA.OrderFlow.Infrastructure` — EF Core (SQL), Mongo (Read), Outbox, Projections
- `LSA.OrderFlow.CrossCutting` — IoC / composição de dependências