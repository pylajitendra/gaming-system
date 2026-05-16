CREATE DATABASE RankingDb;
GO

USE RankingDb;
GO

CREATE TABLE Rankings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PlayerId INT NOT NULL,
    GameId INT NOT NULL,
    Points INT NOT NULL
);


ALTER TABLE Rankings
ADD Rank INT;


-- TEMP table to simulate score sync (will be removed when Service Bus added)
CREATE TABLE Scores (
    Id INT PRIMARY KEY,
    PlayerId INT NOT NULL,
    GameId INT NOT NULL,
    Points INT NOT NULL,
    CreatedAt DATETIME
);


select * from Rankings


SELECT * FROM Rankings
WHERE GameId = 4
AND Points = 390;


DELETE FROM Rankings
WHERE GameId = 4
AND Points = 390;

DELETE FROM Rankings
WHERE GameId = 103;

TRUNCATE TABLE Rankings;