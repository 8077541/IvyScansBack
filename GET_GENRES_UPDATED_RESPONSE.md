# GET Genres API - Response Format

## Overview

The GET genres endpoint returns a simple array of genre names. This provides a clean, straightforward list of all available genres in the system.

## Endpoint Details

- **URL**: `/api/genres`
- **Method**: `GET`
- **Authentication**: Not required
- **Authorization**: Public endpoint

## Response Format

### Current Response Format

```json
[
  "Action",
  "Romance",
  "Comedy",
  "Drama",
  "Fantasy",
  "Sci-Fi",
  "Horror",
  "Slice of Life",
  "Adventure",
  "Mystery"
]
]
```

## Usage Examples

### Using JavaScript/Fetch

```javascript
const getGenres = async () => {
  try {
    const response = await fetch("/api/genres");
    const genres = await response.json();

    console.log("Available genres:", genres);
    // Output: ["Action", "Romance", "Comedy", ...]

    return genres;
  } catch (error) {
    console.error("Error fetching genres:", error);
    throw error;
  }
};

// Usage
getGenres()
  .then((genres) => {
    genres.forEach((genre) => console.log(genre));
  })
  .catch((error) => console.error(error));
```

### Using cURL

```bash
curl -X GET "https://your-api-domain.com/api/genres" \
  -H "Content-Type: application/json"
```

### Using Python (requests)

```python
import requests

def get_genres():
    url = "https://your-api-domain.com/api/genres"
    response = requests.get(url)

    if response.status_code == 200:
        genres = response.json()
        print("Available genres:", genres)
        return genres
    else:
        print(f"Error: {response.status_code}")
        return None

# Usage
genres = get_genres()
if genres:
    for genre in genres:
        print(f"- {genre}")
```

## Features

- **Simple Format**: Returns a straightforward array of genre names
- **No Authentication Required**: Public endpoint accessible by all users
- **Fast Response**: Lightweight data structure for quick loading
- **Easy Integration**: Simple string array works with most frontend frameworks

## Related Endpoints

- `DELETE /api/genres/{name}` - Delete a genre by name
- `GET /api/comics?genre={genreName}` - Get comics by genre name
- `POST /api/comics` - Create comic with genre names

## Notes

- Genre names are case-sensitive
- Empty genre list returns `[]`
- Genres are returned in database order
- For delete operations, use the exact genre name from this endpoint
  const genres = await fetch("/api/genres").then((res) => res.json());
  genres.forEach((genre) => {
  console.log(genre.name); // "Action", "Romance", etc.
  console.log(genre.id); // "550e8400-e29b-41d4-a716-446655440000", etc.
  });

````

### Accessing Genre Names

```javascript
// Extract just the names if needed (compatibility layer)
const genreNames = genres.map((genre) => genre.name);
````

### Using Genre IDs for Operations

```javascript
// Now you can delete genres using their IDs
const deleteGenre = async (genreId, token) => {
  const response = await fetch(`/api/genres/${genreId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// Usage
const selectedGenre = genres.find((g) => g.name === "Action");
if (selectedGenre) {
  await deleteGenre(selectedGenre.id, userToken);
}
```

## Benefits of the New Format

1. **Genre Management**: Can now delete specific genres using their unique IDs
2. **Data Integrity**: Unique identification prevents naming conflicts
3. **Future Extensibility**: Structure allows for additional genre properties
4. **Consistent API Design**: Matches the pattern used by other resources (comics, users, etc.)

## Usage Examples

### React Component Example

```jsx
import React, { useState, useEffect } from "react";

const GenreManager = () => {
  const [genres, setGenres] = useState([]);

  useEffect(() => {
    fetch("/api/genres")
      .then((res) => res.json())
      .then(setGenres);
  }, []);

  const handleDeleteGenre = async (genreId) => {
    try {
      const response = await fetch(`/api/genres/${genreId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${userToken}`,
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        // Remove from local state
        setGenres(genres.filter((g) => g.id !== genreId));
        alert("Genre deleted successfully");
      } else {
        const error = await response.json();
        alert(`Error: ${error.message}`);
      }
    } catch (error) {
      alert("Network error occurred");
    }
  };

  return (
    <div>
      <h2>Genres</h2>
      {genres.map((genre) => (
        <div key={genre.id} className="genre-item">
          <span>{genre.name}</span>
          <button
            onClick={() => handleDeleteGenre(genre.id)}
            className="delete-btn"
          >
            Delete
          </button>
        </div>
      ))}
    </div>
  );
};
```

### Vue.js Component Example

```vue
<template>
  <div>
    <h2>Genres</h2>
    <div v-for="genre in genres" :key="genre.id" class="genre-item">
      <span>{{ genre.name }}</span>
      <button @click="deleteGenre(genre.id)">Delete</button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      genres: [],
    };
  },
  async mounted() {
    const response = await fetch("/api/genres");
    this.genres = await response.json();
  },
  methods: {
    async deleteGenre(genreId) {
      try {
        const response = await fetch(`/api/genres/${genreId}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${this.userToken}`,
            "Content-Type": "application/json",
          },
        });

        if (response.ok) {
          this.genres = this.genres.filter((g) => g.id !== genreId);
          alert("Genre deleted successfully");
        } else {
          const error = await response.json();
          alert(`Error: ${error.message}`);
        }
      } catch (error) {
        alert("Network error occurred");
      }
    },
  },
};
</script>
```

## Testing the Updated Endpoint

### Using cURL

```bash
curl -X GET "http://localhost:5000/api/genres"
```

### Expected Response

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Action"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Romance"
  }
]
```

## Backward Compatibility Note

This is a **breaking change** that requires frontend updates. Consider versioning your API (e.g., `/api/v2/genres`) if you need to maintain backward compatibility for existing clients.
