# AgriConnect Platform

## Overview
AgriConnect Platform is a web-based application designed to connect farmers with agricultural resources and services. The platform facilitates sustainable farming practices by providing a marketplace for agricultural products and services.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Development Environment Setup](#development-environment-setup)
- [Building and Running the Application](#building-and-running-the-application)
- [System Functionalities](#system-functionalities)
- [User Roles and Permissions](#user-roles-and-permissions)
- [Database Initialization](#database-initialization)
- [Troubleshooting](#troubleshooting)

## Prerequisites
- .NET 9.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 (recommended) or Visual Studio Code
- Git (for version control)

## Development Environment Setup

### 1. Clone the Repository
```bash
git clone [repository-url]
cd AgriConnectPlatform
```

### 2. Database Setup
1. Install SQL Server if not already installed
2. Create a new database named "AgriConnectDB"
3. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgriConnectDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Install Dependencies
```bash
dotnet restore
```

## Building and Running the Application

### Using Visual Studio
1. Open `AgriConnectPlatform.sln` in Visual Studio
2. Press F5 or click "Start" to run the application
3. The application will be available at:
   - https://localhost:7140
   - http://localhost:5198

### Using Command Line
```bash
dotnet build
dotnet run
```

## System Functionalities

### Core Features
1. **User Management**
   - User registration and authentication
   - Role-based access control
   - Profile management

2. **Farmer Features**
   - Product listing and management
   - Farm profile management
   - Product categorization
   - Sustainable farming practices documentation

3. **Employee Features**
   - User management
   - Content moderation
   - System administration
   - Analytics and reporting

4. **Product Management**
   - Product categorization
   - Product details and descriptions
   - Sustainable product verification
   - Product search and filtering

## User Roles and Permissions

### 1. Farmers
- Create and manage product listings
- Update farm profiles
- View and manage their products
- Access sustainable farming resources

### 2. Employees
- Manage user accounts
- Moderate content
- Access administrative features
- Generate reports
- Verify sustainable farming practices

## Database Initialization

The system comes with a database initializer that creates:
- Default roles (Farmer, Employee)
- Demo farmer accounts
- Sample products
- Employee account

Default credentials:
- Employee: employee@agriconnect.co.za / Password@123!
- Demo Farmers: farmer1@agriconnect.co.za through farmer4@agriconnect.co.za / Password@123!

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure database exists

2. **Build Errors**
   - Run `dotnet restore`
   - Clear bin and obj folders
   - Update .NET SDK

3. **Runtime Errors**
   - Check application logs
   - Verify database migrations
   - Ensure all environment variables are set

### Getting Help
For additional support:
1. Check the application logs
2. Review the error messages in the console
3. Contact the development team

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
[Your License Information]

## Contact
[Your Contact Information] 