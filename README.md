# Ivy Scans API

A modern ASP.NET Core Web API for managing webcomics, built for portfolio demonstration. This API provides comprehensive functionality for comic management, user authentication, reading progress tracking, and more.

## 🌟 Features

- **Comic Management**: CRUD operations for comics, chapters, and images
- **User Authentication**: JWT-based authentication with refresh tokens
- **Reading Progress**: Track user reading history and bookmarks
- **Genre System**: Categorize comics with multiple genres
- **Rating System**: User rating and review functionality
- **RESTful API**: Clean, well-documented REST endpoints
- **Swagger Documentation**: Interactive API documentation
- **Cross-Platform**: Built with .NET 9.0

## 🚀 Live Demo

- **API URL**: `https://your-railway-domain.up.railway.app`
- **Swagger Documentation**: `https://your-railway-domain.up.railway.app/swagger`

## 🛠 Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL (Production) / SQL Server (Development)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Documentation**: Swagger/OpenAPI
- **Deployment**: Railway
- **Containerization**: Docker

## 📦 Installation & Setup

### Prerequisites

- .NET 9.0 SDK
- SQL Server (for local development)
- Visual Studio 2022 or VS Code

### Local Development

1. **Clone the repository**

   ```bash
   git clone <your-repo-url>
   cd webcomic-reader/BACK
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Update connection string**
   Update `appsettings.Development.json` with your SQL Server connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=IvyScansDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

4. **Run database migrations**

   ```bash
   cd IvyScans.API
   dotnet ef database update
   ```

5. **Run the application**

   ```bash
   dotnet run
   ```

6. **Access the API**
   - API: `https://localhost:7071`
   - Swagger: `https://localhost:7071/swagger`

## 🚂 Railway Deployment

### Prerequisites

1. **Railway Account**: Sign up at [railway.app](https://railway.app)
2. **Domain**: Your custom domain (optional)
3. **GitHub Repository**: Code hosted on GitHub

### Deployment Steps

1. **Create Railway Project**

   - Connect your GitHub repository
   - Railway will auto-detect the Dockerfile

2. **Add PostgreSQL Database**

   - In Railway dashboard, click "New" → "Database" → "PostgreSQL"
   - Note the database credentials

3. **Configure Environment Variables**
   Add these environment variables in Railway:

   ```bash
   # Database
   DATABASE_URL=postgresql://username:password@host:port/database
   DatabaseProvider=PostgreSQL

   # JWT Configuration
   JWT_SECRET=your-super-secure-secret-key-min-32-chars

   # CORS Origins (comma-separated)
   ALLOWED_ORIGINS=https://your-frontend-domain.com,https://your-custom-domain.com

   # ASP.NET Core Environment
   ASPNETCORE_ENVIRONMENT=Production
   ```

4. **Deploy**

   - Push to your main branch
   - Railway will automatically build and deploy

5. **Run Database Migrations**
   - In Railway dashboard, go to your service
   - Open the terminal/console
   - Run: `dotnet ef database update`

### Custom Domain Setup

1. **In Railway Dashboard**:

   - Go to your service → Settings → Domains
   - Click "Custom Domain"
   - Add your domain (e.g., `api.yoursite.com`)

2. **DNS Configuration**:

   - Add a CNAME record pointing to Railway's provided URL
   - Example: `api.yoursite.com` → `your-service.up.railway.app`

3. **Update CORS**:
   Update the `ALLOWED_ORIGINS` environment variable to include your custom domain.

## 🔧 Configuration

### Environment Variables

| Variable           | Description                       | Example                                     |
| ------------------ | --------------------------------- | ------------------------------------------- |
| `DATABASE_URL`     | PostgreSQL connection string      | `postgresql://user:pass@host:5432/db`       |
| `JWT_SECRET`       | JWT signing secret (min 32 chars) | `your-secure-secret-key-32-chars-min`       |
| `ALLOWED_ORIGINS`  | CORS allowed origins              | `https://mysite.com,https://api.mysite.com` |
| `DatabaseProvider` | Database provider                 | `PostgreSQL` or `SqlServer`                 |

### appsettings Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string"
  },
  "DatabaseProvider": "PostgreSQL",
  "Jwt": {
    "SecretKey": "Your JWT secret",
    "Issuer": "IvyScansApi",
    "Audience": "IvyScansClient"
  },
  "AllowedOrigins": ["https://your-frontend.com"]
}
```

## 📚 API Documentation

### Authentication

The API uses JWT Bearer tokens for authentication.

**Login Endpoint**:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password"
}
```

**Response**:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "refresh_token_here",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "username": "username"
  }
}
```

**Using the Token**:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Main Endpoints

| Endpoint                    | Method | Description         | Auth Required |
| --------------------------- | ------ | ------------------- | ------------- |
| `/api/auth/login`           | POST   | User login          | No            |
| `/api/auth/register`        | POST   | User registration   | No            |
| `/api/auth/refresh`         | POST   | Refresh token       | Yes           |
| `/api/comics`               | GET    | Get all comics      | No            |
| `/api/comics/{id}`          | GET    | Get comic by ID     | No            |
| `/api/comics`               | POST   | Create comic        | Yes           |
| `/api/comics/{id}/chapters` | GET    | Get comic chapters  | No            |
| `/api/genres`               | GET    | Get all genres      | No            |
| `/api/user/bookmarks`       | GET    | Get user bookmarks  | Yes           |
| `/api/user/history`         | GET    | Get reading history | Yes           |

### Response Format

```json
{
  "success": true,
  "data": { ... },
  "message": "Success message",
  "errors": []
}
```

## 🏗 Project Structure

```
IvyScans.API/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── ComicsController.cs
│   ├── GenresController.cs
│   └── UserController.cs
├── Data/                 # Database Context
│   └── ApplicationDbContext.cs
├── Models/               # Entity Models
│   ├── Comic.cs
│   ├── Chapter.cs
│   ├── User.cs
│   └── DTO/
├── Services/             # Business Logic
│   ├── AuthService.cs
│   ├── ComicService.cs
│   └── Interfaces.cs
├── Migrations/           # EF Migrations
└── Program.cs           # Application Entry Point
```

## 🔍 Database Schema

### Core Tables

- **Users**: User accounts and authentication
- **Comics**: Comic series information
- **Chapters**: Individual comic chapters
- **ChapterImages**: Chapter page images
- **Genres**: Comic genres/categories
- **ComicGenres**: Many-to-many comic-genre relationship
- **UserBookmarks**: User bookmarked comics
- **UserRatings**: User comic ratings
- **ReadingHistory**: User reading progress
- **RefreshTokens**: JWT refresh tokens

## 🚀 Performance & Scaling

### Current Configuration

- Optimized for portfolio/demo usage
- Auto-migration on production startup
- Lightweight Docker container
- PostgreSQL for reliability

### Scaling Considerations

- Add Redis for caching
- Implement pagination
- Add rate limiting
- Setup CDN for images
- Database connection pooling

## 🔒 Security Features

- JWT token authentication
- Password hashing (BCrypt)
- CORS configuration
- Input validation
- SQL injection prevention (EF Core)
- HTTPS enforcement

## 🧪 Testing

Run the test suite:

```bash
dotnet test
```

## 📝 Contributing

This is a portfolio project, but suggestions and feedback are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contact

- **Developer**: [Your Name]
- **Email**: [your.email@example.com]
- **Portfolio**: [https://yourportfolio.com]
- **LinkedIn**: [Your LinkedIn Profile]

## 🎯 Portfolio Notes

This API demonstrates:

- Modern .NET Core development practices
- Clean architecture principles
- Database design and relationships
- Authentication and authorization
- RESTful API design
- Cloud deployment (Railway)
- Docker containerization
- Environment-based configuration
- Comprehensive documentation

Built as part of a full-stack webcomic reader application showcasing modern web development skills.
