# Railway Deployment Guide for Ivy Scans API

## 🚀 Step-by-Step Deployment

### 1. Prerequisites

- Railway account ([railway.app](https://railway.app))
- GitHub repository with your code
- Your custom domain (optional)

### 2. Create Railway Project

1. **Login to Railway**

   - Go to [railway.app](https://railway.app)
   - Login with GitHub

2. **Create New Project**
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your webcomic-reader repository
   - Select the BACK folder as root directory

### 3. Add PostgreSQL Database

1. **Add Database Service**

   - In your Railway project dashboard
   - Click "New" → "Database" → "Add PostgreSQL"
   - Wait for the database to provision

2. **Get Database Connection Details**
   - Click on the PostgreSQL service
   - Go to "Variables" tab
   - Copy the `DATABASE_URL` value

### 4. Configure Environment Variables

In your API service (not the database), add these environment variables:

```bash
# Database Configuration
DATABASE_URL=postgresql://postgres:password@host:5432/railway
DatabaseProvider=PostgreSQL

# JWT Configuration (generate a secure 32+ character secret)
JWT_SECRET=your-super-secure-jwt-secret-key-min-32-characters-long

# CORS Configuration (replace with your actual domains)
ALLOWED_ORIGINS=https://your-frontend-domain.com,https://api.your-domain.com

# ASP.NET Core Environment
ASPNETCORE_ENVIRONMENT=Production

# Optional: Custom port (Railway auto-detects)
PORT=8080
```

### 5. Deploy Your Application

1. **Automatic Deployment**

   - Push your changes to the main branch
   - Railway will automatically detect the Dockerfile
   - Wait for the build and deployment to complete

2. **Check Deployment Status**
   - Monitor the build logs in Railway dashboard
   - Ensure the deployment completes successfully

### 6. Run Database Migrations

1. **Access Railway Console**

   - Go to your API service in Railway
   - Click on "Deploy" tab
   - Click on the latest deployment
   - Click "View Logs" or open a shell

2. **Run Migration Command**
   ```bash
   dotnet ef database update
   ```

### 7. Set Up Custom Domain (Optional)

1. **Add Custom Domain in Railway**

   - Go to your API service
   - Settings → Domains
   - Click "Custom Domain"
   - Enter your domain (e.g., `api.yoursite.com`)

2. **Configure DNS**

   - In your domain registrar/DNS provider
   - Add a CNAME record:
     - Name: `api` (or your subdomain)
     - Value: `your-service-name.up.railway.app`
   - TTL: 300 seconds (5 minutes)

3. **Update CORS Settings**
   - Update the `ALLOWED_ORIGINS` environment variable
   - Include your custom domain
   - Redeploy if necessary

### 8. Test Your Deployment

1. **Check API Health**

   - Visit: `https://your-railway-url.up.railway.app/swagger`
   - Ensure Swagger documentation loads

2. **Test Endpoints**
   - Try the `/api/comics` endpoint
   - Test authentication endpoints
   - Verify database connectivity

### 9. Frontend Integration

Update your frontend application to use the new API URL:

```javascript
// In your frontend config
const API_BASE_URL = "https://your-railway-url.up.railway.app/api";
// or with custom domain:
const API_BASE_URL = "https://api.your-domain.com/api";
```

## 🔧 Troubleshooting

### Common Issues

1. **Build Failures**

   - Check that Dockerfile is in the correct location
   - Ensure all dependencies are properly installed
   - Review build logs for specific errors

2. **Database Connection Issues**

   - Verify DATABASE_URL is correctly set
   - Ensure PostgreSQL service is running
   - Check if migrations have been applied

3. **CORS Errors**

   - Verify ALLOWED_ORIGINS includes your frontend domain
   - Ensure environment variable is properly formatted
   - Check that CORS policy is correctly applied

4. **JWT Authentication Issues**
   - Ensure JWT_SECRET is set and at least 32 characters
   - Verify JWT configuration in appsettings
   - Check token expiration settings

### Environment Variables Checklist

- [ ] DATABASE_URL (from PostgreSQL service)
- [ ] DatabaseProvider=PostgreSQL
- [ ] JWT_SECRET (32+ characters)
- [ ] ALLOWED_ORIGINS (comma-separated domains)
- [ ] ASPNETCORE_ENVIRONMENT=Production

### Migration Commands

```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration (if needed)
dotnet ef migrations remove
```

## 📝 Environment Variables Template

Create a `.env.example` file for reference:

```bash
# Database
DATABASE_URL=postgresql://username:password@hostname:port/database
DatabaseProvider=PostgreSQL

# JWT
JWT_SECRET=your-32-plus-character-secret-key-here

# CORS
ALLOWED_ORIGINS=https://yourdomain.com,https://api.yourdomain.com

# Environment
ASPNETCORE_ENVIRONMENT=Production
PORT=8080
```

## 🎯 Final Steps

1. **Document Your API**

   - Update README with live API URL
   - Include Swagger documentation link
   - Add any specific usage instructions

2. **Monitor Performance**

   - Check Railway metrics dashboard
   - Monitor response times
   - Watch for any errors in logs

3. **Security Considerations**

   - Ensure HTTPS is enabled (Railway does this automatically)
   - Verify CORS settings are restrictive
   - Monitor for unusual traffic patterns

4. **Backup Strategy**
   - Railway handles database backups automatically
   - Consider exporting important data periodically
   - Document your environment variables securely

## 🔗 Useful Links

- [Railway Documentation](https://docs.railway.app/)
- [PostgreSQL on Railway](https://docs.railway.app/databases/postgresql)
- [Custom Domains](https://docs.railway.app/deploy/custom-domains)
- [Environment Variables](https://docs.railway.app/deploy/variables)

Your API should now be successfully deployed on Railway! 🎉
