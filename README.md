# ?? AWS E-Commerce — Microsserviços com .NET

Sistema de e-commerce baseado em microsserviços na AWS, desenvolvido com C# .NET 8, Lambda, SQS, DynamoDB e SES.

![CI/CD](https://github.com/Kauaro/aws-ecommerce-microsservicos/actions/workflows/build.yml/badge.svg)

---

## ??? Arquitetura
```
Cliente
   ?
API Gateway
   ?
OrderService (Lambda + .NET)
   ?
SQS (OrderCreatedQueue)
   ?
PaymentService (Lambda + .NET)
   ?
SQS (PaymentProcessedQueue)
   ?
NotificationService (Lambda + .NET)
   ?
SES ? E-mail
```

---

## ?? Microsserviços

| Serviço | Responsabilidade | Tecnologias |
|---|---|---|
| OrderService | Recebe e registra pedidos | .NET 8 · Lambda · API Gateway · DynamoDB · SQS |
| PaymentService | Processa pagamentos com idempotência | .NET 8 · Lambda · SQS · DynamoDB |
| NotificationService | Envia notificações por e-mail | .NET 8 · Lambda · SQS · SES |

---

## ?? Como rodar localmente

### Pré-requisitos
- .NET 10 SDK
- Docker Desktop
- AWS CLI

### 1. Sobe o LocalStack
```bash
docker run --rm -d \
  -p 4566:4566 \
  -v localstack-data:/var/lib/localstack \
  --name localstack \
  localstack/localstack
```

### 2. Cria os recursos
```bash
# Tabelas DynamoDB
aws dynamodb create-table --table-name Pedidos --attribute-definitions AttributeName=pedidoId,AttributeType=S --key-schema AttributeName=pedidoId,KeyType=HASH --billing-mode PAY_PER_REQUEST --endpoint-url http://localhost:4566 --region us-east-1 --profile local

aws dynamodb create-table --table-name Pagamentos --attribute-definitions AttributeName=pedidoId,AttributeType=S --key-schema AttributeName=pedidoId,KeyType=HASH --billing-mode PAY_PER_REQUEST --endpoint-url http://localhost:4566 --region us-east-1 --profile local

# Filas SQS
aws sqs create-queue --queue-name OrderCreatedQueue --endpoint-url http://localhost:4566 --region us-east-1 --profile local
aws sqs create-queue --queue-name OrderCreatedDLQ --endpoint-url http://localhost:4566 --region us-east-1 --profile local
aws sqs create-queue --queue-name PaymentProcessedQueue --endpoint-url http://localhost:4566 --region us-east-1 --profile local
```

### 3. Sobe os serviços
```bash
# Terminal 1
cd OrderService && dotnet run

# Terminal 2
cd PaymentService && dotnet run

# Terminal 3
cd NotificationService && dotnet run
```

### 4. Testa
Acessa `http://localhost:5000/swagger` e cria um pedido.

---

## ?? Conceitos aplicados

- **Event-Driven Architecture** — serviços desacoplados via SQS
- **Idempotência** — pagamentos não são processados em duplicidade
- **Dead Letter Queue** — mensagens com falha são preservadas
- **Clean Architecture** — Domain, Application, Infrastructure
- **CI/CD** — GitHub Actions com build automático

---

## ??? Roadmap

- [x] Fase 1 — OrderService + DynamoDB
- [x] Fase 2 — Mensageria SQS + PaymentService + NotificationService
- [x] Fase 3 — CI/CD GitHub Actions
- [ ] Fase 4 — AuthService com ECS + RDS + Redis