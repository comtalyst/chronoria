# Timelette (a.k.a. Project Chronoria)
A web application that create time capsules in form of emails that are scheduled to be sent in the future.  

This repository is for the backend components of the project. For the frontend, please visit https://github.com/comtalyst/chronoria-client/.

## Technology Stacks
- **ASP.NET Core** for all backend services
  - **Entity Framework Core** for data access
- **Microsoft Azure** for hosting and other cloud-based services
  - **App Service** for backend hosting
  - **Blob Storage** for user-uploaded files storage
  - **Database for PostgreSQL** for database system
  - **Key Vault** for configuration secrets management
  - **Service Bus** for messaging system between microservices
  - **Static Web Apps** for frontend hosting
  - **Twillio SendGrid** for email services
- **React** for frontend
- **Tailwind** for frontend

## Architecture
The backend system is distributed into 3 separately-deployed microservices:
- WebAPI
- PersistentWorkers
- ConsumerWorkers  

Each service communicates with each other using the producer-consumer messaging system proved by **Azure Service Bus**.

### WebAPI  
WebAPI is the interface of the system that is designed to interact with the client side web application (frontend). It is designed to be as RESTful as possible. The main endpoints currently being used are:  
- `POST /api/submit/file` for submitting capsule creation request, with file upload
- `POST /api/submit/text` for submitting capsule creation request, without file upload
- `GET /confirm?id=...` for the sender to confirm the creation of the capsule; `id` is exposed to the sender's email only before confirmation and is unique by capsule
- `GET /cancel?id=...&recipientEmail=...` for submitting capsule cancellation request of the capsule with `id`; `recipientEmail` is used for a validation purpose
- `GET /downloadlink?id=...&recipientEmail=...` for access the url to the attached file of the capsule with `id`; `recipientEmail` is used for a validation purpose; the url received can be expired

This component is designed to be responsive to the client side, thus being lightweight in most tasks. It is scalable using replication, which reduces the average response time. 

### PersistentWorkers  
PersistentWorkers handles schedulers and lightweight tasks that have to be active all the time. In this case, there are 2 services being hosted on this component:
- `ExpireClearScheduler` for clearing out expired capsules that receive no confirmation by the sender.
- `CapsuleReleaseScheduler` for "digging out" the capsules when it reaches the scheduled time.

Currently, all schedulers are having a shared design:
- In each loop iteration, they calculate the expected time for the next action. For example, the `CapsuleReleaseScheduler` will access the database to see the nearest capsule release schedule in the future, and goes into sleep until that time comes. The constraints on capsule scheduling help prevent new capsule release schedule being inserted while the scheduler is sleeping.
- On each scheduled time, the scheduler will send a message for **ConsumerWorkers** to process the heavier task.
