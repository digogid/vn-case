CREATE TABLE Page (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name varchar(50) NOT NULL
)

CREATE TABLE UserData (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    PageId INT NOT NULL,
    Ip VARCHAR(16) NOT NULL,
    Browser VARCHAR(100) NOT NULL,
    Input VARCHAR(300) NOT NULL
)

INSERT INTO Page (Name) VALUES
('Home'), ('Landing'), ('Checkout'), ('Confirmacao')