--Section 01
CREATE DATABASE TripService
CREATE TABLE Cities
(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(20) NOT NULL,
CountryCode VARCHAR(2) NOT NULL
)

CREATE TABLE Hotels
(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(30) NOT NULL,
CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
EmployeeCount INT NOT NULL,
BaseRate DECIMAL(5,2)
)

CREATE TABLE Rooms
(
Id INT PRIMARY KEY IDENTITY,
Price DECIMAL(5,2) NOT NULL,
[Type] NVARCHAR(20) NOT NULL,
Beds INT NOT NULL,
HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
)
CREATE TABLE Trips
(
Id INT PRIMARY KEY IDENTITY,
RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
BookDate DATE NOT NULL,
ArrivalDate DATE NOT NULL,
ReturnDate DATE NOT NULL,
CancelDate DATE,
CONSTRAINT Checked_BookDate_ArrivalDate CHECK(DATEDIFF(DAY, BookDate, ArrivalDate) > 0),
CONSTRAINT Checked_ArrivalDate_ReturnDate CHECK(DATEDIFF(DAY, ArrivalDate, ReturnDate) > 0)
)

CREATE TABLE Accounts
(
Id INT PRIMARY KEY IDENTITY,
FirstName NVARCHAR(50) NOT NULL,
MiddleName NVARCHAR(20),
LastName NVARCHAR(50) NOT NULL,
CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
BirthDate DATE NOT NULL,
Email VARCHAR(100) UNIQUE NOT NULL
)

CREATE TABLE AccountsTrips
(
AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
TripId INT FOREIGN KEY REFERENCES Trips(Id) NOT NULL,
Luggage INT CHECK(Luggage >= 0) NOT NULL,
PRIMARY KEY (AccountId, TripId)
)
--Section 02
-- 2. Insert
INSERT INTO Accounts(FirstName,MiddleName,LastName,CityId,BirthDate,Email)
VALUES
('John','Smith','Smith',34,'1975-07-21','j_smith@gmail.com'),
('Gosho',NULL,'Petrov',11,'1978-05-16','g_petrov@gmail.com'),
('Ivan','Petrovich','Pavlov',59,'1849-09-26','i_pavlov@softuni.bg'),
('Friedrich','Wilhelm','Nietzsche',2,'1844-10-15','f_nietzsche@softuni.bg')

INSERT INTO Trips(RoomId,BookDate,ArrivalDate,ReturnDate,CancelDate)
VALUES
(101,'2015-04-12','2015-04-14','2015-04-20','2015-02-02'),
(102,'2015-07-07','2015-07-15','2015-07-22','2015-04-29'),
(103,'2013-07-17','2013-07-23','2013-07-24',NULL),
(104,'2012-03-17','2012-03-31','2012-04-01','2012-01-10'),
(109,'2017-08-07','2017-08-28','2017-08-29',NULL)

-- 3. Update
SELECT *
FROM Rooms

UPDATE Rooms
SET Price += (Price * 0.14)
WHERE HotelId IN (5,7,9)

-- 4. Delete

DELETE
FROM AccountsTrips
WHERE AccountId = 47
DELETE
FROM Accounts
WHERE Id = 47

--Section 03
-- 5. EEE-Mails
SELECT a.FirstName,
a.LastName,
FORMAT(BirthDate,'MM-dd-yyyy') AS BirthDate,
c.[Name] AS [Hometown],
a.Email
FROM Accounts as a
JOIN Cities as c
ON a.CityId = c.Id
WHERE Email LIKE 'e%'
ORDER BY c.Name

-- 6. City Statistics
SELECT c.[Name], COUNT(h.Id) AS [Hotels]
FROM Cities as c
JOIN Hotels as h
ON h.CityId = c.Id
GROUP BY c.[Name]
ORDER BY [Hotels] DESC, c.[Name]

-- 7. Longest and Shortest Trips
SELECT a.Id AS AccountId,
CONCAT(a.FirstName,' ',a.LastName) AS FullName,
MAX(DATEDIFF(DAY,t.ArrivalDate,t.ReturnDate)) AS LongestTrip,
MIN(DATEDIFF(DAY,t.ArrivalDate,t.ReturnDate)) AS ShortestTrip
 FROM Trips AS t
        JOIN AccountsTrips AS [at] ON t.Id = [at].TripId
        LEFT JOIN Accounts AS a ON [at].AccountId = a.Id
WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
GROUP BY a.Id,a.FirstName,a.LastName
ORDER BY LongestTrip DESC, ShortestTrip ASC

-- 8. Metropolis
SELECT TOP(10)c.Id, c.Name AS [City],c.CountryCode AS [Country],COUNT(a.Id) AS [Accounts]
FROM Accounts as a
JOIN Cities as c
ON a.CityId = c.Id
GROUP BY c.Id,c.[Name],c.CountryCode
ORDER BY Accounts DESC


-- 9. Romantic Getaways
SELECT a.Id,a.Email,c.[Name] AS [City],COUNT(at.TripId) AS Trips
FROM
Accounts as a
JOIN AccountsTrips as [at]
ON a.Id = at.AccountId
JOIN Trips as t
ON t.Id = at.TripId
JOIN Rooms as r
ON t.RoomId = r.Id
JOIN Hotels as h
ON h.Id = r.HotelId
JOIN Cities as c
ON c.Id = a.CityId AND c.Id = h.CityId
GROUP BY a.Id,a.Email,c.[Name]
ORDER BY COUNT(at.TripId) DESC, a.Id

-- 10. GDPR Violation
SELECT t.Id,
A2.FirstName + ' ' + ISNULL(A2.MiddleName + ' ', '') + A2.LastName AS [Full Name],
C.[Name] AS [From],
C2.[Name] AS [TO],
IIF(t.CancelDate IS NOT NULL, 'Canceled',
CAST(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate) AS NVARCHAR(100)) + ' days') AS Duration
FROM
Trips AS t
JOIN AccountsTrips A
     ON t.Id = A.TripId
JOIN Accounts A2
     ON A2.Id = A.AccountId
JOIN Cities C
     ON C.Id = A2.CityId
JOIN Rooms R2
     ON R2.Id = t.RoomId
JOIN Hotels H
     ON H.Id = R2.HotelId
JOIN Cities C2
     ON C2.Id = H.CityId
ORDER BY [Full Name],t.Id

-- 11. Available Room
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATE, @People INT)
    RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @RoomId INT = (SELECT TOP 1 r.Id
                               FROM Trips AS t
                                        JOIN Rooms AS r
                                             ON t.RoomId = r.Id
                                        JOIN Hotels AS h
                                             ON r.HotelId = h.Id
                               WHERE h.Id = @HotelId
                                 AND @Date NOT BETWEEN t.ArrivalDate AND t.ReturnDate
                                 AND t.CancelDate IS NULL
                                 AND r.Beds >= @People
                                 AND YEAR(@Date) = YEAR(t.ArrivalDate)
                               ORDER BY r.Price DESC)

    IF @RoomId IS NULL
        RETURN 'No rooms available'

    DECLARE @RoomPrice DECIMAL(15, 2) = (SELECT Price
                                             FROM Rooms
                                             WHERE Id = @RoomId)

    DECLARE @RoomType VARCHAR(50) = (SELECT Type
                                         FROM Rooms
                                         WHERE Id = @RoomId)

    DECLARE @BedsCount INT = (SELECT Beds
                                  FROM Rooms
                                  WHERE Id = @RoomId)

    DECLARE @HotelBaseRate DECIMAL(15, 2) = (SELECT BaseRate
                                                 FROM Hotels
                                                 WHERE Id = @HotelId)

    DECLARE @TotalPrice DECIMAL(15, 2) = (@HotelBaseRate + @RoomPrice) * @People

    RETURN CONCAT('Room ', @RoomId, ': ', @RoomType, ' (', @BedsCount, ' beds', ') - $', @TotalPrice)
END
-- 12. Switch Room
CREATE PROCEDURE usp_SwitchRoom(@TripId INT, @TargetRoomId INT)
AS
BEGIN
    IF ((SELECT TOP 1 h.Id
             FROM Trips AS t
                      JOIN Rooms AS r
                           ON r.Id = t.RoomId
                      JOIN Hotels AS h
                           ON h.Id = r.HotelId
             WHERE t.Id = @TripId) != (SELECT HotelId
                                           FROM Rooms
                                           WHERE @TargetRoomId = Id))
        THROW 50001, 'Target room is in another hotel!', 1

    IF ((SELECT Beds
             FROM Rooms
             WHERE @TargetRoomId = Id) < (SELECT COUNT(*) AS Count
                                              FROM AccountsTrips
                                              WHERE TripId = @TripId))
        THROW 50002, 'Not enough beds in target room!', 1

    UPDATE Trips
    SET RoomId = @TargetRoomId
        WHERE Id = @TripId
END
GO
EXEC usp_SwitchRoom 10, 11

SELECT RoomId
    FROM Trips
    WHERE Id = 10
-- 11

EXEC usp_SwitchRoom 10, 7
--Target room is in another hotel!

EXEC usp_SwitchRoom 10, 8
-- Not enough beds in target room!