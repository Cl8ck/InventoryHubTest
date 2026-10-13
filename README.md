# InventoryHub

InventoryHub is a small full-stack inventory application built as a Coursera test and student practice project. It demonstrates a Blazor WebAssembly client communicating with an ASP.NET Core API.

## Features

- View products with their name, price, stock count, and nested category.
- Add a product with a name, price, stock count, and category.
- Validate product input: names and categories must not be blank, price must be between `0.01` and `999999.99`, and stock cannot be negative.
- Keep newly added products in the current server session so they appear in that session's product list.
- Share product and category models between the client and server.

## Project structure

| Project | Purpose |
| --- | --- |
| `Client` | Blazor WebAssembly user interface. Product list is at `/products`; the add-product form is at `/products/add`. |
| `Server` | ASP.NET Core API and server-side session storage. |
| `Shared` | Product, category, and product-creation request models used by both projects. |

## Requirements

- .NET 10 SDK

## Run locally

Start the API in one terminal:

```sh
dotnet run --project Server/Server.csproj --launch-profile http
```

The API listens at `http://localhost:5220`. In a second terminal, start the Blazor client:

```sh
dotnet run --project Client/Client.csproj --launch-profile http
```

Open `http://localhost:5136`. Use **Product List** to view products or **Add Product** to create one. The client API URL is configured in `Client/wwwroot/appsettings.json`; development CORS origins are configured in `Server/appsettings.Development.json`.

## Product API

The API returns product data in this shape:

```json
{
  "id": 1,
  "name": "Wireless Mouse",
  "price": 29.99,
  "stock": 42,
  "category": {
    "id": 1,
    "name": "Accessories"
  }
}
```

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/products` | List the default products and products added in the current session. |
| `GET` | `/api/products/{id}` | Get a product by ID; returns `404` when it is not found. |
| `POST` | `/api/products` | Add a product to the current session; returns `201 Created`. Invalid input returns `400 Bad Request`. |

Example create request:

```json
{
  "name": "Desk Lamp",
  "price": 34.95,
  "stock": 12,
  "categoryName": "Lighting"
}
```

Request examples are also available in `Server/Products.http`. In development, the API OpenAPI document is available at `/openapi/v1.json`.

## Storage and deployment notes

Products added through the API are stored in ASP.NET Core's in-memory distributed cache and associated with the browser's server-session cookie. They are not saved to a database: added products are lost when the server restarts or the session expires, and they are not shared between sessions.

GitHub Pages can host the static Blazor WebAssembly client, but it cannot run this ASP.NET Core API. To deploy the application, host the API separately, set the client's `ApiBaseUrl` to that API's HTTPS address, and configure the API's production CORS origins to allow the published client. Cross-site session cookies may also require additional browser and hosting configuration. The checked-in configuration is for local development, not a production deployment.

## GitHub Copilot assistance

GitHub Copilot assisted with development by helping implement and refine the shared product models, API controller and service, Blazor product list and add-product form, input validation, session handling, and navigation. It also helped troubleshoot build and browser-test issues and review deployment considerations. The project was built and tested locally; Copilot assistance does not imply that the application has been independently audited or deployed.

## Student project notice

© 2026 Coursera test project. For student purposes only.
