# Microservices Practical Project

This repository contains a hands-on project demonstrating a microservices architecture using .NET 8. The project is structured around several services including Product, Order, Bank, Identity, and Gateway services. Communication between these services is managed using RabbitMQ with MassTransit, and we implement the transactional outbox pattern/saga pattern to ensure data consistency and reliable operations.

# High Level Architecture
![alt text](https://github.com/hptruc/msa-fundamental-hands-on/blob/main/Resources/high_level_architecture.png?raw=true)

# Communication between ProductService, OrderService and BankService
## Workflow
![alt text](https://github.com/hptruc/msa-fundamental-hands-on/blob/main/Resources/workflow.png?raw=true)

## Communication: Saga Orchestration State Machine
### Description
To complete an order, many steps are required from submission to completion (Product Service > Order Service > Bank Service). While there are several approaches to handle this workflow, I chose to implement it using Saga Orchestration combined with a pub/sub mechanism. This approach provides a centralized solution through a state machine, making the management of the workflow states logical and efficient.
### Pros
- **Centralized Control:** The state machine acts as a central controller, making it easier to manage and monitor the flow of transactions.
- **Data Consistency:** Ensures data consistency across multiple services by defining a clear series of steps and compensating actions.
- **Scalability:** Enhances scalability by allowing services to operate independently, only communicating when necessary to complete a transaction.
- **Flexibility:** Offers flexibility to add, modify, or remove steps in the transaction process without significant changes to the overall system.
### Cons
- **Dependency on Orchestrator:** If the orchestrator fails, all workflows stop. However, modern applications often deploy multiple instances to mitigate this issue, so it is less of a concern.
- **Latency:** The orchestrator controls state and sends commands to all services in the workflow. This means a service does not directly send commands to another service but must go through the orchestrator, increasing round trips and latency.
- **Complexity:** Adds complexity to the system by introducing an additional layer of orchestration and the need to define compensating actions.

# How to run
## Prerequisites
- Docker Desktop or WSL2 (WSL2 is used for development)
- VS Code with the following extensions:
    - MongoDB
    - PostgreSQL
    - SQLite Viewer
    - REST Client
## Build Docker Images
1. Open a terminal.
2. Access WSL.
3. Navigate to the root of the project.
4. Run the following command:

```bash
docker compose build --no-cache
```
## Run Docker Images
To start containers in detached mode (no logs), run:

```bash
docker compose up -d
```
# How to Test
- Open the `reverse-proxy.http` file in the REST Client folder.
- Execute **Step 1** to obtain an access token and assign it to the `@token` variable.
- Execute **Step 2** without changing the hardcoded values. This step skips the creation of products and directly tests the creation of an order and payment. Extract the order ID from the response and assign it to the `@orderId` variable in **Step 3**. At this point, the order status should be "Submitted".
- Execute **Step 3** to check whether the order is completed or failed. If the status is "Completed", this indicates that the OrderService has successfully communicated with the BankService and the order has been paid.
- Execute **Step 4** to confirm that the BankService has created a record for the payment order with a status of "Completed".
