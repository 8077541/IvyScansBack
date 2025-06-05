# DELETE Genre Endpoint Documentation

## Overview

The DELETE genre endpoint allows authorized users to delete a genre from the system. The endpoint includes safety checks to prevent deletion of genres that are currently being used by comics.

## Endpoint Details

- **URL**: `/api/genres/{id}`
- **Method**: `DELETE`
- **Authentication**: Required (Bearer token)
- **Authorization**: Any authenticated user

## Request Format

### URL Parameters

- `id` (string, required): The unique identifier of the genre to delete

### Headers

```
Authorization: Bearer <your-jwt-token>
Content-Type: application/json
```

## Response Format

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Genre 'Action' has been successfully deleted"
}
```

### Error Responses

#### Genre Not Found (404 Not Found)

```json
{
  "success": false,
  "message": "Genre not found"
}
```

#### Genre In Use (409 Conflict)

```json
{
  "success": false,
  "message": "Cannot delete genre 'Action' as it is used by 3 comic(s): One Piece, Naruto, Dragon Ball..."
}
```

#### Invalid Request (400 Bad Request)

```json
{
  "success": false,
  "message": "Genre ID is required"
}
```

#### Unauthorized (401 Unauthorized)

```json
{
  "message": "Unauthorized"
}
```

## Usage Examples

### Using cURL

```bash
# Delete a genre
curl -X DELETE "https://your-api-domain.com/api/genres/genre-id-here" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json"
```

### Using JavaScript/Fetch

```javascript
const deleteGenre = async (genreId, token) => {
  try {
    const response = await fetch(`/api/genres/${genreId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    const result = await response.json();

    if (response.ok) {
      console.log("Genre deleted successfully:", result.message);
    } else {
      console.error("Error deleting genre:", result.message);
    }

    return result;
  } catch (error) {
    console.error("Network error:", error);
    throw error;
  }
};

// Usage
deleteGenre("genre-id-here", "your-jwt-token")
  .then((result) => console.log(result))
  .catch((error) => console.error(error));
```

### Using Python (requests)

```python
import requests

def delete_genre(genre_id, token):
    url = f"https://your-api-domain.com/api/genres/{genre_id}"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

    response = requests.delete(url, headers=headers)

    if response.status_code == 200:
        print("Genre deleted successfully")
    elif response.status_code == 404:
        print("Genre not found")
    elif response.status_code == 409:
        print("Genre is in use and cannot be deleted")
    elif response.status_code == 401:
        print("Unauthorized - check your token")
    else:
        print(f"Error: {response.status_code}")

    return response.json()

# Usage
result = delete_genre("genre-id-here", "your-jwt-token")
print(result)
```

## Important Notes

### Safety Features

1. **Cascade Prevention**: The endpoint prevents deletion of genres that are currently used by comics
2. **Authentication Required**: Only authenticated users can delete genres
3. **Detailed Error Messages**: Clear feedback about why deletion failed

### Best Practices

1. Always check if a genre is in use before attempting deletion
2. Consider implementing soft deletes for audit trail purposes
3. Implement proper role-based access control if needed (admin-only deletion)
4. Log genre deletion activities for audit purposes

### Business Logic

- Genres that are currently assigned to comics cannot be deleted
- The system will list up to 3 comic titles that are using the genre
- Empty genres (not assigned to any comics) can be safely deleted

## Testing the Endpoint

You can test this endpoint using:

1. **Swagger UI**: Available at `/swagger` when running the API
2. **Postman**: Import the API collection and test with proper authentication
3. **Browser Dev Tools**: Use the examples above in the browser console

## Related Endpoints

- `GET /api/genres` - Get all genres
- `GET /api/comics?genre={genreName}` - Get comics by genre
- `DELETE /api/comics/{id}` - Delete comic (which also removes genre relationships)
