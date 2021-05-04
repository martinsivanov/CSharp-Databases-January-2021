--Section 01
CREATE DATABASE ColonialJourney

CREATE TABLE Planets
(
Id INT PRIMARY KEY IDENTITY,
[Name] VARCHAR(30) NOT NULL,
)
CREATE TABLE Spaceports
(
Id INT PRIMARY KEY IDENTITY,
[Name] VARCHAR (50) NOT NULL,
PlanetId INT FOREIGN KEY REFERENCES Planets(Id) NOT NULL
)

CREATE TABLE Spaceships
(
Id INT PRIMARY KEY IDENTITY,
[Name] VARCHAR(50) NOT NULL,
Manufacturer VARCHAR(30) NOT NULL,
LightSpeedRate INT DEFAULT 0
)

CREATE TABLE Colonists
(
Id INT PRIMARY KEY IDENTITY,
FirstName VARCHAR(20) NOT NULL,
LastName VARCHAR(20) NOT NULL,
Ucn VARCHAR(10) UNIQUE NOT NULL,
BirthDate DATE NOT NULL
)

CREATE TABLE Journeys
(
Id INT PRIMARY KEY IDENTITY,
JourneyStart DATETIME NOT NULL,
JourneyEnd DATETIME NOT NULL,
Purpose VARCHAR(11) CHECK(Purpose IN('Medical', 'Technical', 'Educational', 'Military')),
DestinationSpaceportId INT FOREIGN KEY REFERENCES Spaceports(Id) NOT NULL,
SpaceshipId INT FOREIGN KEY REFERENCES Spaceships(Id) NOT NULL
)

CREATE TABLE TravelCards
(
Id INT PRIMARY KEY IDENTITY,
CardNumber VARCHAR(10) UNIQUE NOT NULL,
JobDuringJourney VARCHAR(8) CHECK(JobDuringJourney IN ('Pilot','Engineer','Trooper','Cleaner','Cook')),
[ColonistId] INT FOREIGN KEY REFERENCES Colonists(Id) NOT NULL,
JourneyId INT FOREIGN KEY REFERENCES Journeys(Id) NOT NULL
)
--Section 02
-- 2. Insert

INSERT INTO Planets([Name])
VALUES
('Mars'),
('Earth'),
('Jupiter'),
('Saturn')

INSERT INTO Spaceships([Name],Manufacturer,LightSpeedRate)
VALUES
('Golf','VW',3),
('WakaWaka','Wakanda',4),
('Falcon9','SpaceX',1),
('Bed','Vidolov',6)

-- 3. Update

UPDATE Spaceships
SET LightSpeedRate += 1
WHERE Id BETWEEN 8 AND 12

SELECT TOP(3) *
FROM Journeys
ORDER BY JourneyStart ASC

-- 4. Delete
DELETE 
FROM TravelCards
WHERE JourneyId IN (SELECT TOP(3)Id
FROM Journeys)
DELETE
FROM Journeys
WHERE Id IN(SELECT TOP(3)Id
FROM Journeys)

--Section 03---

-- 5. Select all military journeys
SELECT Id,
FORMAT(JourneyStart,'dd/MM/yyyy') AS JourneyStart,
FORMAT(JourneyEnd,'dd/MM/yyyy') AS JourneyEnd
FROM Journeys
WHERE Purpose = 'Military'
ORDER BY Journeys.JourneyStart ASC

-- 6. Select All Pilots
SELECT c.Id,
CONCAT(c.FirstName,' ',LastName) AS [FullName]
FROM Colonists as c
JOIN TravelCards as t
ON t.ColonistId = c.Id
WHERE t.JobDuringJourney = 'Pilot'
ORDER BY c.Id ASC

-- 7. Count Colonists
SELECT COUNT(c.Id) AS [count]
FROM Colonists as c
JOIN TravelCards as t
ON c.Id = t.ColonistId
WHERE t.JobDuringJourney = 'Engineer'
GROUP BY c.Id

-- 8. Select Spaceships With Pilots
SELECT s.[Name] AS [Name], s.Manufacturer
FROM Spaceships as s
JOIN Journeys as j
ON s.Id = j.SpaceshipId
JOIN TravelCards as tc
ON j.Id = tc.JourneyId
JOIN Colonists as c
ON tc.ColonistId = c.Id
WHERE c.BirthDate > '01-01-1989' AND tc.JobDuringJourney = 'Pilot'
ORDER BY s.Name ASC

--9.Select all planets and their journey count
SELECT p.Name, COUNT(j.DestinationSpaceportId) AS JourneysCount
FROM Planets as p
JOIN Spaceports as s
ON p.Id = s.PlanetId
JOIN Journeys as j
ON j.DestinationSpaceportId = s.Id
GROUP BY p.[Name]
ORDER BY JourneysCount DESC,p.[Name] ASC

-- 10. Select Special Colonists
SELECT * 
FROM
(SELECT tc.JobDuringJourney,
CONCAT(c.FirstName,' ',c.LastName) AS FullName,
DENSE_RANK() OVER (PARTITION BY tc.JobDuringJourney ORDER BY BirthDate) AS JobRank
FROM Colonists as c
JOIN TravelCards as tc
ON c.Id = tc.ColonistId) AS r
WHERE r.JobRank = 2

SELECT COUNT(tc.ColonistId) AS [Count]
FROM Planets as p
JOIN Spaceports as s
ON p.Id = s.PlanetId
JOIN Journeys as j
ON j.DestinationSpaceportId = s.Id
JOIN TravelCards as tc
ON tc.JourneyId = j.Id
WHERE p.[Name] = 'Otroyphus'
GROUP BY p.[Name]

--Section 04--

-- 11. Get Colonists Count
CREATE OR
ALTER FUNCTION udf_GetColonistsCount(@PlanetName VARCHAR(30))
    RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(C.Id)
                FROM Planets AS p
                         JOIN Spaceports S
                              ON p.Id = S.PlanetId
                         JOIN Journeys J
                              ON S.Id = J.DestinationSpaceportId
                         JOIN TravelCards Tc
                              ON J.Id = Tc.JourneyId
                         JOIN Colonists C
                              ON C.Id = Tc.ColonistId
                WHERE p.Name = @PlanetName)
END
GO

SELECT dbo.Udf_Getcolonistscount('Otroyphus')
-- 35

-- 12. Change Journey Purpose
CREATE OR
ALTER PROCEDURE usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose NVARCHAR(30))
AS
    IF (@JourneyId NOT IN (SELECT j.Id
                               FROM Journeys AS j))
        BEGIN
            THROW 50001,'The journey does not exist!', 1
        END
    IF ((SELECT j.Purpose
             FROM Journeys AS j
             WHERE j.Id = @JourneyId) = @NewPurpose)
        BEGIN
            THROW 50002,'You cannot change the purpose!', 1
        END

UPDATE Journeys
SET Purpose=@NewPurpose
    WHERE Id = @JourneyId
GO

EXEC usp_ChangeJourneyPurpose 4, 'Technical'
-- not output
-- 1 row affected

EXEC usp_ChangeJourneyPurpose 2, 'Educational'
-- You cannot change the purpose!

EXEC usp_ChangeJourneyPurpose 196, 'Technical'
-- The journey does not exist!