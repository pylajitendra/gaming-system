CREATE DATABASE GameDb;

USE GameDb;

CREATE TABLE Games (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE()
);

select * from Games

ALTER TABLE Games
ADD PlayerId INT;

UPDATE Games
SET PlayerId = 1
WHERE Id = 5;


UPDATE Games
SET PlayerId = 2
WHERE Id = 2;


