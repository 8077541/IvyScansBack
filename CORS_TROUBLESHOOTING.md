# CORS Troubleshooting Guide

## Current CORS Configuration

The API is configured to handle CORS in the following way:

### Environment Variable Setup

Set the `ALLOWED_ORIGINS` environment variable in Railway with comma-separated domains:

```
ALLOWED_ORIGINS=https://is.dominikjaniak.com,http://localhost:3000
```

### CORS Policies

1. **AllowFrontendApp**: Used in production with specific allowed origins
2. **ProductionDebug**: Temporary debugging policy that allows all origins
3. **AllowAll**: Development policy that allows all origins

### Current Active Policy

- Development: `AllowAll` (allows any origin)
- Production: `ProductionDebug` (temporarily allows all origins for debugging)

## Common CORS Issues and Solutions

### 1. "Access to fetch at ... has been blocked by CORS policy"

**Causes:**

- Frontend domain not in ALLOWED_ORIGINS
- Missing or incorrect environment variable setup
- Credentials being sent with incompatible CORS policy

**Solutions:**

1. Verify `ALLOWED_ORIGINS` environment variable in Railway
2. Check that frontend domain matches exactly (including https/http)
3. Ensure no trailing slashes in domain names

### 2. Preflight Requests Failing

**Symptoms:** OPTIONS requests failing before GET/POST requests

**Solutions:**

1. Ensure `.AllowAnyMethod()` is set
2. Check that `.AllowAnyHeader()` is configured
3. Verify CORS middleware is before authentication middleware

### 3. Credentials Issues

**Symptoms:** Requests work without credentials but fail with credentials

**Solutions:**

1. Cannot use `AllowAnyOrigin()` with `AllowCredentials()`
2. Must specify exact origins when using credentials
3. Frontend must send credentials: 'include' in fetch options

## Testing CORS

### Using Browser Developer Tools

1. Open browser console
2. Check Network tab for failed requests
3. Look for CORS-related error messages

### Using curl

```bash
# Test preflight request
curl -X OPTIONS https://isapi.dominikjaniak.com/api/comics \
  -H "Origin: https://is.dominikjaniak.com" \
  -H "Access-Control-Request-Method: GET" \
  -v

# Test actual request
curl -X GET https://isapi.dominikjaniak.com/api/comics \
  -H "Origin: https://is.dominikjaniak.com" \
  -v
```

### Current Debug Logs

The API logs the following for debugging:

- Environment ALLOWED_ORIGINS value
- Configured origins from appsettings
- Final allowed origins list
- Each origin being checked in ProductionDebug policy

## Recommended Steps

1. **Verify Environment Variable:**

   ```bash
   # In Railway deployment logs, check for:
   Environment ALLOWED_ORIGINS: https://is.dominikjaniak.com
   ```

2. **Switch to Specific Origins:**
   Once CORS is working with ProductionDebug, switch back to AllowFrontendApp:

   ```csharp
   app.UseCors("AllowFrontendApp"); // Production
   ```

3. **Remove Debug Logging:**
   After fixing, remove console.log statements and debug policies

## Current Frontend Domain

- Production: `https://is.dominikjaniak.com`
- API: `https://isapi.dominikjaniak.com`

## Expected ALLOWED_ORIGINS Value

```
https://is.dominikjaniak.com
```

Note: No trailing slash, exact protocol match required.
