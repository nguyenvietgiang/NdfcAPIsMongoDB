# Nam Định FC API (ASP.Net Core WebAPI + MongoDB)

This project is an ASP.NET Core Web API built with .NET 6.0 and MongoDB as the database. It serves as an API backend for managing the "Nam Đinh FC" football team. The API provides endpoints for CRUD operations, authentication using JWT, CRUD operations with Excel files, and sending email notifications.

## System Requirements

- .NET 6.0 SDK
- MongoDB
- Any compatible code editor (Visual Studio Code, Visual Studio, etc.)

## Installation and Setup

1. Clone the repository or download it as a ZIP file.
2. Open the project in your preferred code editor.
3. Configure the MongoDB connection:
   - Open the `appsettings.json` file.
   - Update the `ConnectionString` property with your MongoDB connection string.
4. Run the application using the `dotnet run` command or by pressing `F5` in Visual Studio.

## Features

### Chat Realtime với SignalR

### JWT Authentication

- The API supports JWT-based authentication for secure access to protected routes.
- Users can register and login to obtain an access token for authorization.

### CRUD Operations for Football Team

- The API provides endpoints for creating, reading, updating, and deleting football team members.
- Users with appropriate authorization can perform these CRUD operations on the team members.

### CRUD Operations with Excel

- The API supports importing and exporting data to and from Excel files.
- Users can upload an Excel file to import football team data or export team member data.
- The API handles the mapping of Excel columns to the appropriate database fields for seamless data transfer.

### Email Notifications

- The API can send email notifications for various events related to the "Nam Đinh FC" team.
- Email templates can be customized and configured within the API.
- Users can receive notifications for match updates, team announcements, and more.
  
### Logging and Caching

## API Documentation

The API endpoints and their descriptions are documented using Swagger. Once the application is running, you can access the Swagger UI by navigating to `/swagger` on your local machine.

## Conclusion

The Nam Đinh FC Web API provides a comprehensive backend solution for managing the "Nam Đinh FC" football team. With features such as JWT authentication, CRUD operations with Excel files, and email notifications, it offers a powerful toolset for building applications related to the team.

Please note that this is a simplified example, and you may need to customize and expand the code and functionalities to meet your specific requirements.

