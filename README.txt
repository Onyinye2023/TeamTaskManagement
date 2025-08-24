TEAM TASK MANAGEMENT API
A task management system that allows registered users to login, create teams, assign tasks, and track progress.

Tech Stack
•	Backend Framework: ASP.NET Core 8 (Web API)
•	ORM: Entity Framework Core
•	Database: SQL Server
•	Authentication: JWT (JSON Web Tokens)
•	Object Mapping: AutoMapper
•	Logging: Microsoft.Extensions.Logging
•	Dependency Injection: Built-in .NET Core DI

Setup Steps
1. Clone the Repository
git clone https://github.com/Onyinye2023/TeamTaskManagement.git
cd TeamTaskManagement

2. Configure Database
Open appsettings.json and update your connection string.

3. Apply Migrations
dotnet ef database update

4. Run the Application
dotnet run

 API Usage Examples
Authentication & User Management
1. SuperAdmin Login
POST /api/User/auth/login
{
  "email": "superadmin@gmail.com",
  "password": "SuperSecret123!"
}

2. Register a User (SuperAdmin only)
POST /api/User/auth/create-user

3. Create a Team
POST /api/Teams/team
The user who creates the team becomes the Team Admin.
Other users added will be Team Members.

4. Add a User to a Team
POST /api/Team/{teamId}/users
5. Get All Teams
GET /api/teams

6. Create a Task
POST /api/teams/{teamId}/tasks

7. Update a Task
PUT /api/tasks/{taskId}

8. Update Task Status
PATCH /api/tasks/{teamId}/status

9. Delete a Task
DELETE /api/tasks/{taskId}

10. Get All Tasks for a Team
GET /api/team/{teamId}/task

Assumptions
•	A SuperAdmin account already exists.
•	Only registered users can create or join teams.
•	Tasks can only be assigned to team members.
•	Deleting a user does not delete tasks (for auditing purposes).
•	All timestamps (CreatedAt, etc.) are stored in UTC (UtcNow).
•	Error handling uses structured logging with meaningful messages.

