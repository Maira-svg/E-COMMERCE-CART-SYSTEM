Daraz Ultimate – Full E‑Commerce Platform (Frontend + Backend Setup)
This document provides everything you need to run Daraz Ultimate, a complete e‑commerce frontend application with product browsing, shopping cart, user authentication, admin panel, AI chatbot, and simulated payments. The frontend is a single HTML/CSS/JS file that communicates with a REST API. Below you will find the required backend specification, database schema, and step‑by‑step setup instructions using XAMPP (PHP + MySQL). An optional Docker deployment method is also included.

📌 Table of Contents
Overview

Tech Stack

Database Schema (MySQL)

API Endpoints Required

Setup with XAMPP

Folder Structure

Sample PHP Code for Endpoints

Docker Deployment (Optional)

Testing the Application

Troubleshooting

License

Overview
Daraz Ultimate is a premium online shopping store that allows users to:

Browse products across 9+ categories (Electronics, Fashion, Gaming, etc.)

Add/remove items to/from a persistent cart (database‑backed)

Register / login (session‑based authentication)

Admin users can add, edit, or delete products

Simulate checkout with a dummy payment gateway

Interact with an AI chatbot for store assistance

The frontend is already built – a single index.html file containing all styles, logic, and UI. The backend must be implemented by you. This README describes exactly which API endpoints are needed, the database structure, and how to set everything up with XAMPP (or Docker).

Tech Stack
Layer	Technology
Frontend	HTML5, CSS3 (Bootstrap 5), JavaScript (Vanilla)
Backend	PHP 7+ (MySQLi / PDO) – REST API
Database	MySQL (via phpMyAdmin)
Server	Apache (XAMPP)
Optional	Docker (LAMP stack)
Database Schema (MySQL)
Create a database named daraz_ultimate and run the following SQL statements.

1. Users Table
sql
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role ENUM('USER', 'ADMIN') DEFAULT 'USER',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
2. Products Table
sql
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    category VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    imageUrl VARCHAR(500) NOT NULL
);
Seed data: The frontend contains 88 predefined products. Insert them using a PHP script or manually via phpMyAdmin.

3. Cart Table
sql
CREATE TABLE cart (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);
4. Orders Table
sql
CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    order_number VARCHAR(50) UNIQUE NOT NULL,
    total_amount DECIMAL(10,2) NOT NULL,
    payment_method VARCHAR(50),
    status VARCHAR(50) DEFAULT 'COMPLETED',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
5. Order Items Table
sql
CREATE TABLE order_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    product_name VARCHAR(200),
    price DECIMAL(10,2),
    quantity INT NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
);
API Endpoints Required
The frontend makes fetch calls to the following endpoints. All must return JSON and use session cookies for authentication.

Method	Endpoint	Description
POST	/api/Auth/register	Register new user
POST	/api/Auth/login	Login user, start session
POST	/api/Auth/logout	Destroy session
GET	/api/Auth/check	Return current logged‑in user
GET	/api/Product	Get all products
POST	/api/Product	Create a new product (admin only)
PUT	/api/Product/{id}	Update a product (admin only)
DELETE	/api/Product/{id}	Delete a product (admin only)
GET	/api/Cart	Get cart items for logged user
POST	/api/Cart/add	Add product to cart
PUT	/api/Cart/update	Update cart item quantity
DELETE	/api/Cart/remove/{id}	Remove cart item
POST	/api/Cart/checkout	Create order, clear cart
Important: The frontend expects the {id} placeholder in URLs (e.g., /api/Product/5). You can implement routing by parsing PATH_INFO or using rewrite rules.

Setup with XAMPP
Step 1 – Install XAMPP
Download XAMPP and start the Apache and MySQL modules.

Step 2 – Place the Frontend File
Copy the provided index.html (the complete Daraz Ultimate frontend) into the following directory:

text
C:\xampp\htdocs\daraz\index.html   (Windows)
/opt/lampp/htdocs/daraz/index.html (Linux)
Step 3 – Create the Backend Structure
Inside htdocs/daraz/, create this folder and file layout:

text
daraz/
├── index.html
├── api/
│   ├── Auth/
│   │   ├── register.php
│   │   ├── login.php
│   │   ├── logout.php
│   │   └── check.php
│   ├── Product.php
│   ├── Cart.php
│   └── Cart/
│       ├── add.php
│       ├── update.php
│       ├── remove.php
│       └── checkout.php
├── config/
│   └── database.php
└── .htaccess
Step 4 – Database Configuration
Create config/database.php:

php
<?php
$host = 'localhost';
$user = 'root';
$pass = '';
$dbname = 'daraz_ultimate';

$conn = new mysqli($host, $user, $pass, $dbname);
if ($conn->connect_error) {
    die(json_encode(['error' => 'Database connection failed']));
}
?>
Step 5 – Enable URL Rewriting (Optional)
Create .htaccess to remove .php extensions and enable PATH_INFO:

apache
RewriteEngine On
RewriteCond %{REQUEST_FILENAME} !-f
RewriteCond %{REQUEST_FILENAME} !-d
RewriteRule ^(.*)$ index.php [QSA,L]
But a simpler approach: let your PHP files read the raw URL. For example, api/Product.php can handle /api/Product/123 by inspecting $_SERVER['PATH_INFO'].

Step 6 – Run the Application
Open your browser and go to:
http://localhost/daraz/

Folder Structure
Path	Purpose
index.html	Frontend (already provided)
api/Auth/*.php	Registration, login, logout, session check
api/Product.php	CRUD for products (GET, POST, PUT, DELETE)
api/Cart.php	GET cart items for logged user
api/Cart/add.php	Add item to cart
api/Cart/update.php	Change quantity
api/Cart/remove.php	Delete cart item
api/Cart/checkout.php	Process checkout & create order
config/database.php	Database connection
Sample PHP Code for Endpoints
Below are minimal working examples for key endpoints. You can expand them with error handling and validation.

Example 1: api/Auth/register.php
php
<?php
header('Content-Type: application/json');
require_once '../../config/database.php';

$data = json_decode(file_get_contents('php://input'), true);
$name = $data['name'];
$email = $data['email'];
$password = password_hash($data['password'], PASSWORD_DEFAULT);
$role = $data['role'] ?? 'USER';

$stmt = $conn->prepare("INSERT INTO users (name, email, password, role) VALUES (?, ?, ?, ?)");
$stmt->bind_param("ssss", $name, $email, $password, $role);
if ($stmt->execute()) {
    echo json_encode(['message' => 'Registration successful', 'name' => $name, 'role' => $role]);
} else {
    http_response_code(400);
    echo json_encode(['message' => 'Email already exists']);
}
?>
Example 2: api/Auth/login.php
php
<?php
session_start();
header('Content-Type: application/json');
require_once '../../config/database.php';

$data = json_decode(file_get_contents('php://input'), true);
$email = $data['email'];
$password = $data['password'];

$stmt = $conn->prepare("SELECT id, name, password, role FROM users WHERE email = ?");
$stmt->bind_param("s", $email);
$stmt->execute();
$result = $stmt->get_result();
$user = $result->fetch_assoc();

if ($user && password_verify($password, $user['password'])) {
    $_SESSION['user_id'] = $user['id'];
    $_SESSION['user_name'] = $user['name'];
    $_SESSION['user_role'] = $user['role'];
    echo json_encode(['message' => 'Login successful', 'name' => $user['name'], 'role' => $user['role']]);
} else {
    http_response_code(401);
    echo json_encode(['message' => 'Invalid credentials']);
}
?>
Example 3: api/Product.php (GET all products)
php
<?php
header('Content-Type: application/json');
require_once '../config/database.php';

$method = $_SERVER['REQUEST_METHOD'];

if ($method === 'GET') {
    $result = $conn->query("SELECT * FROM products");
    $products = $result->fetch_all(MYSQLI_ASSOC);
    echo json_encode($products);
}
// Add POST, PUT, DELETE with similar logic (check admin role)
?>
Example 4: api/Cart.php (GET cart items)
php
<?php
session_start();
header('Content-Type: application/json');
require_once '../config/database.php';

if (!isset($_SESSION['user_id'])) {
    http_response_code(401);
    echo json_encode([]);
    exit;
}

$user_id = $_SESSION['user_id'];
$stmt = $conn->prepare("
    SELECT cart.id, cart.product_id, cart.quantity, products.title, products.price, products.imageUrl 
    FROM cart 
    JOIN products ON cart.product_id = products.id 
    WHERE cart.user_id = ?
");
$stmt->bind_param("i", $user_id);
$stmt->execute();
$result = $stmt->get_result();
$cart = [];
while ($row = $result->fetch_assoc()) {
    $cart[] = [
        'id' => $row['id'],
        'productId' => $row['product_id'],
        'quantity' => $row['quantity'],
        'product' => [
            'title' => $row['title'],
            'price' => $row['price'],
            'imageUrl' => $row['imageUrl']
        ]
    ];
}
echo json_encode($cart);
?>
Full implementations for PUT, DELETE, and checkout are left as an exercise. Follow the same pattern: read input, update database, return JSON.

Docker Deployment (Optional)
If you prefer containerization, use the following docker-compose.yml file. It sets up Apache with PHP, MySQL, and phpMyAdmin.

docker-compose.yml
yaml
version: '3.8'
services:
  webserver:
    image: php:8.2-apache
    ports:
      - "80:80"
    volumes:
      - ./:/var/www/html
    depends_on:
      - db
  db:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: root
      MYSQL_DATABASE: daraz_ultimate
    ports:
      - "3306:3306"
  phpmyadmin:
    image: phpmyadmin/phpmyadmin
    ports:
      - "8080:80"
    environment:
      PMA_HOST: db
Steps:

Place index.html and all backend PHP files in the same directory as docker-compose.yml.

Run docker-compose up -d.

Access the store at http://localhost and phpMyAdmin at http://localhost:8080.

Testing the Application
Register a new user (choose Customer or Administrator).

Login with that account.

Browse products, use Add to Cart, update quantities, remove items.

Proceed to checkout – any dummy card number works (e.g., 4242 4242 4242 4242).

Admin access: If you registered as ADMIN, click the Admin Panel button on the left sidebar. You can add, edit, or delete products.

Chatbot: Click the orange robot icon at the bottom‑right corner and ask questions (e.g., “Show me electronics”, “How does cart work?”).

Troubleshooting
Issue	Solution
404 Not Found on API calls	Ensure your PHP files are placed exactly as shown in the folder structure.
Session not persisting	Add session_start() at the top of every API endpoint file.
Cart is always empty	Verify that user_id is stored in $_SESSION after login.
CORS errors	Add the following headers to each PHP file:
header("Access-Control-Allow-Origin: http://localhost");
header("Access-Control-Allow-Credentials: true");
header("Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS");
Admin panel not accessible	Make sure the logged‑in user has role = 'ADMIN' in the users table.
Payment page does not appear	Check that the frontend can reach /api/Cart/checkout and that the endpoint returns a valid order object.
License
This project is open‑source and free to use for personal, educational, or commercial purposes.

Happy Shopping!
Daraz Ultimate – Where quality meets convenience
