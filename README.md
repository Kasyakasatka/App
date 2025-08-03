User Management App

About the project

The User Management App is a web application built with ASP.NET Core for managing users. It includes features for user registration, authentication, and administration.



Features

Registration and Authentication: Users can create accounts and log in.



Admin Panel: The administrator can view, block, unblock, and delete users.



OTP Password Reset: A secure password reset mechanism using a one-time password (OTP) sent via email has been implemented.



Security: Protection against CSRF and SQL injection is provided, and a unique index for email is used in the database.



Responsive Design: The user interface is adapted to display correctly on various devices.



Technologies

Backend: ASP.NET Core 9.0, C#



Database: PostgreSQL



ORM: Entity Framework Core



Frontend: HTML5, CSS3, JavaScript



Styling: Bootstrap 5, Font Awesome



Validation: FluentValidation



Deployment: Docker, Render



Local Project Launch

Clone the repository:



git clone https://github.com/new



Navigate to the project folder:



cd UserManagementApp



Run Docker Compose:



docker-compose up --build



Open the application:

The application will be available at http://localhost.



Deployment on Render

To deploy on Render.com, follow these steps:



Create a new Web Service on Render and connect your repository.



Specify build and run commands:



Build Command: dotnet publish -c Release -o ./publish



Start Command: dotnet ./publish/UserManagementApp.dll



Add environment variables:

Specify the database connection string from Render in the ConnectionStrings\_\_DefaultConnection variable.

