--PROBLEM 8

SELECT f.AircraftId ,a.Manufacturer,a.FlightHours, COUNT(f.AircraftId) AS FlightDestinationsCount, ROUND(AVG(f.TicketPrice),2) AS AvgPrice
FROM Aircraft AS a
JOIN FlightDestinations AS f ON a.Id = f.AircraftId
GROUP BY  a.Manufacturer,a.FlightHours,f.AircraftId
HAVING COUNT(f.AircraftId) >= 2
ORDER BY FlightDestinationsCount DESC, f.AircraftId ASC

--PROBLEM 9
SELECT p.FullName, COUNT(f.AircraftId) AS CountOfAircraft, SUM(f.TicketPrice) AS TotalPayed
FROM Passengers AS p
JOIN FlightDestinations AS f ON f.PassengerId = p.Id
WHERE p.FullName LIKE '_a%'
GROUP BY p.FullName
HAVING COUNT(f.AircraftId) > 1
ORDER BY p.FullName

--PROBLEM 10
SELECT a.AirportName,f.[Start] AS DayTime, f.TicketPrice,p.FullName,ai.Manufacturer,ai.Model
FROM FlightDestinations AS f
JOIN Airports AS a ON a.Id = f.AirportId
JOIN Passengers as p ON p.Id = f.PassengerId
JOIN Aircraft AS ai ON ai.Id = f.AircraftId
WHERE (CAST(f.[Start] AS TIME) BETWEEN '06:00' and '20:00') AND TicketPrice > 2500
ORDER BY ai.Model ASC

GO
CREATE FUNCTION udf_FlightDestinationsByEmail(@email VARCHAR(100))
RETURNS INT 
AS
BEGIN
	DECLARE @return_value INT =
	(SELECT COUNT(f.Id)
	FROM Passengers AS p
	JOIN FlightDestinations AS f ON f.PassengerId = p.Id
	WHERE p.Email = @email
	GROUP BY p.Email);
	IF @return_value IS NULL
    SET @return_value = 0
	RETURN @return_value
END;

SELECT dbo.udf_FlightDestinationsByEmail ('PierretteDunmuir@gmail.com')

SELECT dbo.udf_FlightDestinationsByEmail('Montacute@gmail.com')
SELECT dbo.udf_FlightDestinationsByEmail('MerisShale@gmail.com')

--PROBLEM 12
GO
CREATE PROCEDURE usp_SearchByAirportName(@AirportName VARCHAR(70))
AS
BEGIN

    SELECT 
a.AirportName,
p.FullName,
CASE
	WHEN f.TicketPrice <= 400 THEN 'Low'
	WHEN f.TicketPrice > 400 AND f.TicketPrice <= 1500 THEN 'Medium'
	ELSE 'High'
END AS LevelOfTickerPrice
,ai.Manufacturer
,ai.Condition
,aty.TypeName
FROM Airports AS a
JOIN FlightDestinations AS f ON f.AirportId = a.Id
JOIN Passengers AS p ON f.PassengerId = p.Id
JOIN Aircraft AS ai ON ai.Id = f.AircraftId
JOIN AircraftTypes AS aty ON aty.Id = ai.TypeId
WHERE a.AirportName = @AirportName
ORDER BY ai.Manufacturer , p.FullName

END



EXEC usp_SearchByAirportName 'Sir Seretse Khama International Airport'


