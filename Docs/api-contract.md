# API Contract v1.0

## 1. Purpose

This document defines the standard API communication contract between microservices.

All services (.NET, Django, etc.) MUST follow these rules.

---

# 2. Communication

Protocol:

- HTTP REST
- JSON
- UTF-8

Headers:

Content-Type: application/json

Accept: application/json

---

# 3. Base URL

Format:

/{resource}

Example:

GET /users

---

# 4. Naming

Endpoints use nouns.

Correct:

GET /users
POST /users
GET /users/{id}

Incorrect:

GET /getUsers

---

JSON naming:

camelCase

Example:

{
 "firstName":"Ali",
 "createdAt":"2026-06-18T10:00:00Z"
}

---

# 5. HTTP Methods

GET    Read
POST   Create
PUT    Replace/Update
DELETE Remove

---

# 6. Success Response

All successful responses:

{
 "data": {},
 "requestId": "uuid"
}

Example:

{
 "data":{
   "id":10,
   "name":"Ali"
 },
 "requestId":"abc"
}

The requestId will be sent by the gateway.

---

# 7. Pagination

Pagination data MUST be inside data.

Example:

{
 "data":{
   "items":[
     {
       "id":1
     }
   ],
   "page":1,
   "pageSize":20,
   "total":100
 },
 "requestId":"abc"
}

---

# 8. Error Response

All errors MUST follow:

{
 "type":"ValidationError",
 "title":"Validation Error",
 "status":400,
 "detail":"Validation failed",
 "errors":[
   {
     "code":"VALIDATION_ERROR",
     "description":"Field is invalid",
     "type":"Validation"
   }
 ],
 "requestId":"uuid"
}

This example is for validation errors, you should change parameters by the error type.

---

# 9. Common Error Codes

VALIDATION_ERROR
UNAUTHORIZED
FORBIDDEN
NOT_FOUND
CONFLICT
INTERNAL_ERROR

---

# 10. Authentication

Authorization header:

Bearer {token}

Only the gateway and user-service should handle this section.

---

# 11. Request Id

Every request MUST have:

X-Request-ID

If missing, gateway creates it.

Every service MUST forward it.

---

# 13. Date Format

ISO-8601 UTC

Example:

2026-06-18T12:00:00Z

---

# 14. Enum

Enums MUST be string.

Correct:

{
 "status":"ACTIVE"
}

Incorrect:

{
 "status":1
}

---

# 15. Health Check

Every service MUST expose:

GET /health

Response:

{
 "status":"ok",
 "service":"user-service"
}

---

# 17. Database Isolation

A service MUST NOT access another service database directly.

Communication:

HTTP / Events only

---

# 18. API Documentation

Implementing ...