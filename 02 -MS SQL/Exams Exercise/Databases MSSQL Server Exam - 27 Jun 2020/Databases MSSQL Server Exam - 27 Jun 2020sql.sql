--Section 01
CREATE DATABASE WMS

CREATE TABLE Clients
(
    ClientId  INT PRIMARY KEY IDENTITY,
    FirstName VARCHAR(50) NOT NULL,
    LastName  VARCHAR(50) NOT NULL,
    Phone     CHAR(12) CHECK (LEN(Phone) = 12)
)

CREATE TABLE Mechanics
(
    MechanicId INT PRIMARY KEY IDENTITY,
    FirstName  VARCHAR(50)  NOT NULL,
    LastName   VARCHAR(50)  NOT NULL,
    Address    VARCHAR(255) NOT NULL
)

CREATE TABLE Models
(
    ModelId INT PRIMARY KEY IDENTITY,
    Name    VARCHAR(50) UNIQUE
)

CREATE TABLE Jobs
(
    JobId      INT PRIMARY KEY IDENTITY,
    ModelId    INT  NOT NULL FOREIGN KEY REFERENCES Models (ModelId),
    Status     VARCHAR(11) CHECK (Status IN ('Pending', 'In Progress', 'Finished')) DEFAULT 'Pending',
    ClientId   INT  NOT NULL FOREIGN KEY REFERENCES Clients (ClientId),
    MechanicId INT FOREIGN KEY REFERENCES Mechanics (MechanicId),
    IssueDate  DATE NOT NULL,
    FinishDate DATE
)

CREATE TABLE Orders
(
    OrderId   INT PRIMARY KEY IDENTITY,
    JobId     INT NOT NULL FOREIGN KEY REFERENCES Jobs (JobId),
    IssueDate DATE,
    Delivered BIT NOT NULL DEFAULT 0
)

CREATE TABLE Vendors
(
    VendorId INT PRIMARY KEY IDENTITY,
    Name     VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Parts
(
    PartId       INT PRIMARY KEY IDENTITY,
    SerialNumber VARCHAR(50) UNIQUE NOT NULL,
    Description  VARCHAR(255),
    Price        DECIMAL(18, 2) CHECK (Price > 0),
    VendorId     INT                NOT NULL FOREIGN KEY REFERENCES Vendors (VendorId),
    StockQty     INT                NOT NULL DEFAULT 0 CHECK (StockQty >= 0)
)

CREATE TABLE OrderParts
(
    OrderId  INT NOT NULL FOREIGN KEY REFERENCES Orders (OrderId),
    PartId   INT NOT NULL FOREIGN KEY REFERENCES Parts (PartId),
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity > 0)

        CONSTRAINT PK_OrderParts
            PRIMARY KEY (OrderId, PartId)
)

CREATE TABLE PartsNeeded
(
    JobId    INT NOT NULL FOREIGN KEY REFERENCES Jobs (JobId),
    PartId   INT NOT NULL FOREIGN KEY REFERENCES Parts (PartId),
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity > 0),

    CONSTRAINT PK_PartsNeeded
        PRIMARY KEY (JobId, PartId)
)
--Section 02

-- 2. Insert
INSERT INTO Clients(FirstName,LastName,Phone)
VALUES
('Teri','Ennaco','570-889-5187'),
('Merlyn','Lawler','201-588-7810'),
('Georgene','Montezuma','925-615-5185'),
('Jettie'	,'Mconnell','908-802-3564'),
('Lemuel','Latzke','631-748-6479'),
('Melodie','Knipp','805-690-1682'),
('Candida','Corbley','908-275-8357')

INSERT INTO Parts(SerialNumber,[Description],Price,VendorId)
VALUES
('WP8182119','Door Boot Seal',117.86,2),
('W10780048','Suspension Rod',42.81,1),
('W10841140','Silicone Adhesive',6.77,4),
('WPY055980','High Temperature Adhesive',13.94,3)
-- 3. Update
UPDATE Jobs
SET MechanicId = 3 , [Status] = 'In Progress'
WHERE [Status] = 'Pending'
-- 4. Delete
DELETE FROM OrderParts
WHERE OrderId = 19
DELETE FROM Orders
WHERE OrderId = 19

--Section 03

-- 5. Mechanic Assignments
SELECT 
CONCAT(m.FirstName,' ',m.LastName) AS [Mechanic],
j.Status,
j.IssueDate
FROM Mechanics as m
JOIN Jobs as j ON j.MechanicId = m.MechanicId
ORDER BY m.MechanicId,j.IssueDate,j.JobId
-- 6. Current Clients
SELECT 
CONCAT(c.FirstName,' ',c.LastName) AS [Client],
DATEDIFF(DAY,j.IssueDate,'2017-04-24') AS [Days going],
[Status]
FROM Clients as c
JOIN Jobs as j ON c.ClientId = j.ClientId
WHERE j.Status != 'Finished'
ORDER BY [Days going] DESC,c.ClientId ASC

-- 7. Mechanic Performance
SELECT 
CONCAT(m.FirstName,' ',m.LastName) AS [Mechanic],
AVG(DATEDIFF(DAY,j.IssueDate,j.FinishDate)) AS [Average Days]
FROM Mechanics as m
JOIN Jobs as j ON m.MechanicId = j.MechanicId
GROUP BY m.FirstName,m.LastName,m.MechanicId
ORDER BY m.MechanicId

-- 8. Available Mechanics
SELECT m.FirstName + ' ' + m.LastName
    FROM Mechanics AS m
             LEFT JOIN Jobs J
                       ON m.MechanicId = J.MechanicId
    WHERE J.Status = 'Finished'
       OR J.JobId IS NULL
    ORDER BY m.MechanicId
	-- 9. Past Expenses
SELECT j.JobId,
       ISNULL(SUM(P.Price * Op.Quantity), 0.00) AS Total
    FROM Jobs AS j
             LEFT JOIN Orders O
                       ON j.JobId = O.JobId
             LEFT JOIN OrderParts Op
                       ON O.OrderId = Op.OrderId
             LEFT JOIN Parts P
                       ON P.PartId = Op.PartId
    WHERE j.Status = 'Finished'
    GROUP BY j.JobId
    ORDER BY Total DESC,
             j.JobId

-- 10. Missing Parts
SELECT p.PartId,
       p.Description,
       Pn.Quantity,
       p.StockQty,
       IIF(O.Delivered = 0, Op.Quantity, 0)
    FROM Parts AS p
             LEFT JOIN PartsNeeded Pn
                       ON p.PartId = Pn.PartId
             LEFT JOIN OrderParts Op
                       ON p.PartId = Op.PartId
             LEFT JOIN Orders O
                       ON O.OrderId = Op.OrderId
             LEFT JOIN Jobs J
                       ON J.JobId = Pn.JobId
    WHERE J.Status != 'Finished'
      AND (p.StockQty + IIF(O.Delivered = 0, Op.Quantity, 0)) < Pn.Quantity
    ORDER BY p.PartId

--Section 04
-- 11. Place Order
CREATE PROCEDURE usp_PlaceOrder(@JobId INT, @PartSerialNumber VARCHAR(50), @Quantity INT)
AS
    IF ((SELECT j.Status
             FROM Jobs AS j
             WHERE j.JobId = @JobId) = 'Finished')
        BEGIN
            THROW 50011, 'This job is not active!',1
        END
    IF (@Quantity <= 0)
        BEGIN
            THROW 50012, 'Part quantity must be more than zero!',1
        END

DECLARE
    @job INT = (SELECT j.JobId
                    FROM Jobs AS j
                    WHERE j.JobId = @JobId)
    IF (@job IS NULL)
        BEGIN
            THROW 50013, 'Job not found!', 1
        END

DECLARE
    @partId INT = (SELECT p.PartId
                       FROM Parts AS p
                       WHERE p.SerialNumber = @PartSerialNumber)
    IF (@partId IS NULL)
        BEGIN
            THROW 50014, 'Part not found!', 1
        END
    IF ((SELECT o.OrderId
             FROM Orders AS o
             WHERE o.JobId = @JobId
               AND o.IssueDate IS NULL) IS NULL)
        BEGIN
            INSERT INTO Orders(JobId, IssueDate, Delivered)
                VALUES (@JobId, NULL, 0)
        END

DECLARE
    @orderId INT = (SELECT o.OrderId
                        FROM Orders AS o
                        WHERE o.JobId = @JobId
                          AND o.IssueDate IS NULL)

DECLARE
    @orderPartsQty INT = (SELECT op.Quantity
                              FROM OrderParts AS op
                              WHERE op.OrderId = @orderId
                                AND op.PartId = @partId)
    IF (@orderPartsQty IS NULL)
        BEGIN
            INSERT INTO OrderParts(OrderId, PartId, Quantity)
                VALUES (@orderId, @partId, @Quantity)
        END
    ELSE
        BEGIN
            UPDATE OrderParts
            SET Quantity += @Quantity
                WHERE PartId = @partId
                  AND OrderId = @orderId
        END
GO

DECLARE @err_msg AS NVARCHAR(MAX);
BEGIN TRY
    EXEC usp_PlaceOrder 1, 'ZeroQuantity', 0
END TRY
BEGIN CATCH
    SET @err_msg = ERROR_MESSAGE();
    SELECT @err_msg
END CATCH

-- 12. Cost Of Order
CREATE FUNCTION udf_GetCost(@JobId INT)
    RETURNS DECIMAL(18, 2)
BEGIN

    DECLARE @sum DECIMAL(18, 2) = (SELECT SUM(p.Price)
                                       FROM Parts AS p
                                                JOIN OrderParts Op
                                                     ON p.PartId = Op.PartId
                                                JOIN Orders O
                                                     ON O.OrderId = Op.OrderId
                                                JOIN Jobs J
                                                     ON O.JobId = J.JobId
                                       WHERE J.JobId = @JobId)

    IF (@sum IS NULL)
        BEGIN
            SET @sum = 0
        END

    RETURN @sum
END

SELECT dbo.udf_GetCost(1)
