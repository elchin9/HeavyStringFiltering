# Heavy String Filtering API

This project is a backend system designed to receive large string inputs in multiple chunks, process the input asynchronously, and filter out sensitive or harmful words using fuzzy-matching algorithms such as Jaro-Winkler and Levenshtein. The solution is optimized for memory efficiency and scalable parallel processing.

---

## Features

- Upload string data in **multiple chunks**
- Asynchronous and streaming **background processing**
- Fuzzy filtering of offensive/sensitive words using **configurable similarity thresholds**
- In-memory **result caching** (non-queued results)
- Parallel processing with **SemaphoreSlim**
- CQRS pattern with **MediatR**
- Clean Architecture with clear layering:
  - **API**
  - **Application**
  - **Domain**
  - **Infrastructure**
- Unit tests included

---

## Technologies to Use

- .NET 8 / C#
- ASP.NET Core Web API
- **Async processing**
- Clean Architecture (recommended)
- MediatR (CQRS)
- Custom similarity algorithms (e.g. Levenshtein, Jaro-Winkler)
- Unit testing (xUnit, Moq)
- No database required

---

## Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/elchin9/HeavyStringFiltering.git
cd HeavyStringFiltering
```

### 2. Configure appsettings.json
```json
{
  "FilterConfig": {
    "Words": [ "bomb", "attack", "angry", "kill", "ugly", "burn", "crash", "death", "cancer" ],
    "Threshold": 0.8
  }
}
```

### 3. Run the application
```bash
dotnet run --project HeavyStringFiltering.API
```

---

## Running Unit Tests

The solution includes comprehensive unit tests for:

- Text filtering logic
- Chunked upload service
- Background workers

To run tests:
```bash
dotnet test
```

---

## How It Works

1. **Chunk Uploading**  
   You send parts of the full input via `POST api/upload` as multiple chunks. Each chunk contains:
   - `UploadId`: Unique identifier
   - `ChunkIndex`: Sequence number
   - `IsLastChunk`: Marks the final chunk
   - `Data`: String part

2. **Processing**
   - When `IsLastChunk` is received, all chunks are reassembled.
   - Text is enqueued for filtering in a background worker.

3. **Filtering**
   - Worker filters out words that are **at least 80% similar** to any configured keyword.
   - Filtering is done **in parallel** using `SemaphoreSlim`.

4. **Storage**
   - Result is **cached in-memory** using a `Guid` key.
---

## API Endpoints

### Upload Chunk
```http
POST api/upload
Content-Type: application/json
```
```json
{
  "uploadId": "string-guid",
  "chunkIndex": 0,
  "data": "Hello this is a chunk...",
  "isLastChunk": false
}
```

## Configuration

Settings can be configured via `appsettings.json`:

| Key             | Description                                |
|------------------|--------------------------------------------|
| `Words`         | List of banned words to filter             |
| `Threshold`     | Similarity threshold (0.0 to 1.0, e.g., 0.8) |

---

## Examples

Send 3 chunks:

```bash
curl -X POST http://localhost:5215/api/upload -H "Content-Type: application/json" -d '{"uploadId": "abc", "chunkIndex": 0, "data": "Hello bomb attack", "isLastChunk": false}'
curl -X POST http://localhost:5215/api/upload -H "Content-Type: application/json" -d '{"uploadId": "abc", "chunkIndex": 1, "data": "death ugly", "isLastChunk": false}'
curl -X POST http://localhost:5215/api/upload -H "Content-Type: application/json" -d '{"uploadId": "abc", "chunkIndex": 2, "data": "safe word", "isLastChunk": true}'
```

## Notes

- No database used. Only in-memory structures.
- Result is stored temporarily and can be accessed once.


## Requirements Satisfied from Task Document

- [x] Chunk-based uploading
- [x] Filter words ≥ 80% similar (configurable)
- [x] Background processing (non-blocking)
- [x] In-memory one-time result storage
- [x] Clean, maintainable architecture
- [x] Async
- [x] Unit tested