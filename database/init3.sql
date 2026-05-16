CREATE DATABASE ScoreDb;
GO

USE ScoreDb;
GO

CREATE TABLE Scores (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PlayerId INT NOT NULL,
    GameId INT NOT NULL,
    Points INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

select * from Scores

DELETE FROM Scores
WHERE Id = 3;

TRUNCATE TABLE Scores;