# Timelette (a.k.a. Project Chronoria)
Link (now live!): https://www.timelette.app/ 
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
- On each scheduled time, the scheduler will send a message for **ConsumerWorkers** to process the heavier tasks.  

This component is implemented as `WebHost` and deployed as an App Service. However, I believe it is not the best practice to host the endpoint-less service in this way.

### ConsumerWorkers  
ConsumerWorkers is another background service that is intended for heavier workloads that do not require immediate response to the client. Its interface is the consumers that consume messages from **Azure Service Bus**, containing the parameters for the predefined tasks. For example, when we need to send an email to the user, this component will retrieve a message from the service bus that contains the some content of the email and the recipient address before telling the email service to send it. It is scalable using replication.   

This component is implemented as `WebHost` and deployed as an App Service. However, I believe it is not the best practice to host the endpoint-less service in this way.

## Concerns/TODOs
As I learned through the development of this project, there are some minor concerns and issues that are not yet solved at the initial launch. Some are recommended minor additions which may be implemented soon. Some requires the change of the architecture, which might be pick up later if the demand of the application increases, or when urgency is observed. Nevertheless, all these issues are recorded and will serve as a learning experience in which I will keep in mind for the next projects.

- The latter 2 components should be hosted in a different way that does not have to waste the endpoint supports of **Azure App Service**. Implementing them as `HostedServices` and deploying them as container apps instead might be one possible solution.
- Anti-DDOS for the APIs would be useful, if I have more budget...
- Also from the limited budget, instances replication is currently disabled
- Add more performance measurement metrics (e.g., delay of capsule delivery time relative to the scheduled time)
- Use authentication system for users
- Find a better way to implement the schedulers, if exist
- Current outage and error handling methods is not ideal
  - Create some healthcheck endpoint(?) on the latter 2 components
  - Make use of dead-lettering if messages for **ConsumerWorkers** fail
- Separate the files in the blob storage between the hot (frequently accessed) and cold (archived)
- The design for the database architecture seemed to have a major flaw--data can be lost when moving between databases
  - Currently, the core data is splitted into 3 different databases: `Pending`, `Active`, `Archived`
  - When a capsule changes its status (e.g. the user confirms it), it needs to be moved between databases
  - Since this process involve multiple databases, controlling the atomocity of transactions is not typical (and not currently implemented)
  - If we somehow lost connection to the database between the process, then the data might be lost
  - The concerns over race conditions from multiple services accessing a capsule data at the same time also creates a requirement for the concerning capsule data to be deleted immediately when one of the services acquire it (similar to acquiring a lock). This is possible through the use of **Entity Framework Core**. However, the immediate deletion creates a point in time where the data is on the primary memory only.
- Other security concerns, if exist
