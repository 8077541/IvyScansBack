# Delete Comic API Endpoint

## Endpoint

`DELETE /api/comics/{id}`

## Description

Deletes a comic and all its related data including:

- All chapters and their images
- Comic-genre relationships
- User bookmarks for this comic
- User ratings for this comic
- Reading history entries for this comic

## Request

```http
DELETE /api/comics/your-comic-id-here
```

## Response

### Success (200 OK)

```json
{
  "message": "Comic 'Comic Title' and all related data have been successfully deleted"
}
```

### Comic Not Found (404 Not Found)

```json
{
  "message": "Comic not found"
}
```

### Server Error (500 Internal Server Error)

```json
{
  "message": "An error occurred while deleting the comic"
}
```

## Example using curl

```bash
curl -X DELETE "https://your-api-domain.com/api/comics/your-comic-id-here"
```

## Example using JavaScript/Fetch

```javascript
async function deleteComic(comicId) {
  try {
    const response = await fetch(`/api/comics/${comicId}`, {
      method: "DELETE",
    });

    const result = await response.json();

    if (response.ok) {
      console.log("Comic deleted successfully:", result.message);
    } else {
      console.error("Failed to delete comic:", result.message);
    }
  } catch (error) {
    console.error("Error deleting comic:", error);
  }
}
```

## Important Notes

- This operation is **irreversible**. Once a comic is deleted, all related data is permanently removed.
- The deletion is done in a transaction to ensure data consistency.
- If any part of the deletion fails, the entire operation is rolled back.
- Consider implementing authorization (admin roles) before deploying to production.
