
# Online Marketplace

Marketplace API endpoints that allow users to buy and sell products, manage inventory, and process transactions within the marketplace.


## Features

- User Authentication: Secure authentication using JSON Web Token (JWT).
- Product Search: Search for any products by name, and add them to the shopping cart before checking out.
- Email Subscription Service: Manage user subscriptions and send promotional emails or notifications.
- Microservices Architecture: Built using multiple microservices to ensure modularity and scalability.
- Multiple Databases: Utilizes multiple databases to handle different data types and optimize performance.
- Messaging Queue: Implements a messaging broker for faster communications between services.


## Technologies Used

- DotNet
- C#
- JWT
- PostgreSQL
- Redis
- RabbitMQ
## API Reference

## Get All Items in Inventory

GET /Inventory

| Parameter | Type | Description |
| --- | --- | --- |
| N/A | N/A | Retrieves all items in the inventory |

#### Get Item by ID

GET /Inventory/{id}

| Parameter | Type | Description |
| --- | --- | --- |
| id | int | **Required**. Id of the item to fetch |

#### Add New Item to Inventory

POST /Inventory/New

| Parameter | Type | Description |
| --- | --- | --- |
| CreateProductDTO | CreateProductDTO | **Required**. Data for the new product, including name, price, etc. |

#### Update Item in Inventory

PUT /Inventory/Update/{id}

| Parameter | Type | Description |
| --- | --- | --- |
| id | int | **Required**. Id of the product to update |
| UpdateProductDTO | UpdateProductDTO | **Required**. Updated product data |

#### Delete Item from Inventory

DELETE /Inventory/Delete/{id}

| Parameter | Type | Description |
| --- | --- | --- |
| id | int | **Required**. Id of the product to delete |

#### Get All Products in Shopping Cart

GET /ShoppingCart/

| Parameter | Type | Description |
| --- | --- | --- |
| N/A | N/A | Get all the items stored in shopping cart. Returns id of the user and id of the item|

#### Add New Item to Shopping Cart

POST /ShoppingCart/Add

| Parameter | Type | Description |
| --- | --- | --- |
| AddToShoppingCartDTO | AddToShoppingCartDTO | **Required**. Data for the item to add to the cart |

#### Remove Item from Shopping Cart

DELETE /ShoppingCart/Remove/{id}

| Parameter | Type | Description |
| --- | --- | --- |
| id | int | **Required**. Id of the item to remove |

#### Finalize Shopping Cart (Checkout)

POST /ShoppingCart/Checkout

| Parameter | Type | Description |
| --- | --- | --- |
| N/A | N/A | Finalizes the shopping cart's checkout process |
