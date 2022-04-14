# Timelette (a.k.a. Project Chronoria)

<img src="https://user-images.githubusercontent.com/16837889/163303085-45215b04-ebd9-4e4d-8d3a-c83d2bdbaa34.png" width="512" height="512" alt="mascot">
  
Link (now live!): https://www.timelette.app/   

A web application that creates time capsules in form of emails that are scheduled to be sent in the future.  

This repository is for the backend components of the project. For the frontend, please visit https://github.com/comtalyst/chronoria-client/.

## Motivations  
This is just a personal project developed by me to sharpen my C#, familiarize with cloud services, practice designing distributed systems, just and to entertain myself by coding as usual. ☜(ﾟヮﾟ☜)


## Technology Stacks
- **ASP.NET Core** for all backend services
  - **Entity Framework Core** for data access
- **Microsoft Azure** for hosting and other cloud-based services
  - **App Service** for backend hosting
  - **Blob Storage** for user-uploaded files storage
  - **Database for PostgreSQL** for relational database system
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

Each service communicates with each other using the producer-consumer messaging system proved by **Azure Service Bus**. The purpose of this architecture is to support scaling and to separate client-responsive tasks and other background tasks.  
The design of implementation of all services are similar, which is based on .NET typical Object-Oriented Programming with dependency injections paradigm.

### WebAPI  
WebAPI is the interface of the system that is designed to interact with the client-side web application (frontend). It is designed to be as RESTful as possible. The main endpoints currently being used are:  
- `POST /api/submit/file` for submitting capsule creation request, with file upload
- `POST /api/submit/text` for submitting capsule creation request, without file upload
- `GET /confirm?id=...` for the sender to confirm the creation of the capsule; `id` is exposed to the sender's email only before confirmation and is unique to capsule
- `GET /cancel?id=...&recipientEmail=...` for submitting capsule cancellation request of the capsule with `id`; `recipientEmail` is used for a validation purpose
- `GET /downloadlink?id=...&recipientEmail=...` for access the URL to the attached file of the capsule with `id`; `recipientEmail` is used for a validation purpose; the URL received can be expired

This component is designed to be responsive to the client-side, thus being lightweight in most tasks. It is scalable using replication, which reduces the average response time. 

### PersistentWorkers  
PersistentWorkers handles schedulers and lightweight tasks that have to be active all the time. In this case, 2 services are being hosted on this component:
- `ExpireClearScheduler` for clearing out expired capsules that receive no confirmation by the sender.
- `CapsuleReleaseScheduler` for "digging out" the capsules when it reaches the scheduled time.

Currently, all schedulers are having a shared design:
- In each loop iteration, they calculate the expected time for the next action. For example, the `CapsuleReleaseScheduler` will access the database to see the nearest capsule release schedule in the future and goes to sleep until that time comes. The constraints on capsule scheduling help prevent a new capsule release schedule from being inserted while the scheduler is sleeping.
- On each scheduled time, the scheduler will send a message for **ConsumerWorkers** to process the heavier tasks.  

This component is implemented as `WebHost` and deployed as an App Service. However, I believe it is not the best practice to host the endpoint-less service in this way.

### ConsumerWorkers  
ConsumerWorkers is another background service that is intended for heavier workloads that do not require immediate response to the client. Its interface is the consumers that consume messages from **Azure Service Bus**, containing the parameters for the predefined tasks. For example, when we need to send an email to the user, this component will retrieve a message from the service bus that contains some content of the email and the recipient's address before telling the email service to send it. It is scalable using replication.   

This component is implemented as `WebHost` and deployed as an App Service. However, I believe it is not the best practice to host the endpoint-less service in this way.

## Concerns/TODOs
As I learned through the development of this project, some minor concerns and issues are not yet solved at the initial launch. Some are recommended minor additions which may be implemented soon. Some require a change of the architecture, which might be picked up later if the demand for the application increases, or when urgency is observed. Nevertheless, all these issues are recorded and will serve as a learning experience that I will keep in mind for the next projects (⌐■_■).

- The latter 2 components should be hosted in a different way that does not have to waste the endpoint supports of **Azure App Service**. Implementing them as `HostedServices` and deploying them as container apps instead might be one possible solution.
- Anti-DDOS for the APIs would be useful if I have more budget...
- Also from the limited budget, instances replication is currently disabled
- Add more performance measurement metrics (e.g., delay of capsule delivery time relative to the scheduled time)
- Use an authentication system for users
- Find a better way to implement the schedulers, if exist
- Current outage and error handling methods is not ideal
  - Create some health check endpoint(?) on the latter 2 components
  - Make use of dead-lettering if messages for **ConsumerWorkers** fail
- Separate the files in the blob storage between the hot (frequently accessed) and cold (archived)
- The design for the database architecture seemed to have a major flaw--data can be lost when moving between databases
  - Currently, the core data is split into 3 different databases: `Pending`, `Active`, and `Archived`. The purpose (what I thought) is to split the database by functionality for scaling purposes.
  - When a capsule changes its status (e.g. the user confirms it), it needs to be moved between databases
  - Since this process involve multiple databases, controlling the atomicity of transactions is not typical (and not currently implemented)
  - If we somehow lost connection to the database during the process, then the data might be lost
  - The concerns over race conditions from multiple services accessing capsule data at the same time also create a requirement for the concerning capsule data to be deleted immediately when one of the services acquires it (similar to acquiring a lock). This is possible through the use of **Entity Framework Core**. However, the immediate deletion creates a point in time where the data is on the primary memory only.
  - The resolution might be to use one unified database with the status indicator as a column or to create a routine task that gradually archives data securely.
- More unit tests coverage (currently they are implemented for sensitive/complex parts only)
- More automated CI/CD pipeline
- Other security concerns, if exist

## Installation
In `/server`, there are 3(+3 tests) components existed as folders, each representing a **Microsoft Visual Studio** C# project. Since all of them are included in a unified solution, you could import the `.sln` file into **Microsoft Visual Studio**. And...that's it! ☜(ﾟヮﾟ☜)  
Note that secret configs are stored in `secrets.json`, which is bound to your machine rather than in this repository.  
Currently, no CI/CD pipeline has been set up. All components are using manual deployment at the time being.
