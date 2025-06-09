CREATE TABLE partners (
    partner_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
    address NVARCHAR(510) COLLATE Cyrillic_General_CI_AS NOT NULL,
    phone VARCHAR(20) COLLATE Cyrillic_General_CI_AS,
    type TINYINT NOT NULL CHECK (type BETWEEN 0 AND 3),
    rating INT NOT NULL CHECK (rating > 0) -- Ensures rating is always > 0
);

CREATE TABLE products (
    product_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
    price DECIMAL(18,2) NOT NULL CHECK (price >= 0)
);

CREATE TABLE partners_products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    partner_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity >= 0),
    FOREIGN KEY (partner_id) REFERENCES partners(partner_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    UNIQUE (partner_id, product_id) -- Each pair should be unique
);

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(20) COLLATE Cyrillic_General_CI_AS NOT NULL UNIQUE,
    password NVARCHAR(510) COLLATE Cyrillic_General_CI_AS NOT NULL,
    role TINYINT NOT NULL CHECK (role BETWEEN 0 AND 3) -- Assuming 0–3 are valid roles
);
