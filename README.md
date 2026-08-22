## Chat app

That's a showcase project. Real-time chatting app built with microservices and kinda VSA.


### **THE README IS OUTDATED I'M LAZY THE COMMENT WILL BE REMOVED AS SOON AS I UPDATE THE README**

## Tools

- .NET 10
- ASP.NET Core
- Entity Framework Core
- SignalR


## Infrastructure

- Docker
- PostgreSQL for DB
- Redis for caching (in the future)
- RabbitMQ as a message broker
- Nginx as an API Gateway (in the future)


## Has the following structure

- API Gateway (in the future)
- User service for handling user profiles related stuff
- Media service for media (not implemented yet)
- Chat service for real time chatting (in progress rn)
- Auth service for authentication



## Usage


```git clone https://github.com/F1elle/chatapp.git```

```cd chatapp```

```docker-compose up```
