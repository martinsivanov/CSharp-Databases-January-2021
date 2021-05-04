--Section 1--
CREATE DATABASE Bakery

CREATE TABLE Countries
(
    Id   INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Customers
(
    Id          INT PRIMARY KEY IDENTITY,
    FirstName   NVARCHAR(25) NOT NULL,
    LastName    NVARCHAR(25) NOT NULL,
    Gender      CHAR(1),
    CHECK (Gender IN ('M', 'F')),
    Age         INT          NOT NULL,
    PhoneNumber CHAR(10),
    CHECK (LEN(PhoneNumber) = 10),
    CountryId   INT          NOT NULL FOREIGN KEY REFERENCES Countries (Id)
)

CREATE TABLE Products
(
    Id          INT PRIMARY KEY IDENTITY,
    Name        NVARCHAR(25)  NOT NULL UNIQUE,
    Description NVARCHAR(250),
    Recipe      NVARCHAR(MAX) NOT NULL,
    Price       DECIMAL(18, 4),
    CHECK (Price >= 0)
)

CREATE TABLE Feedbacks
(
    Id          INT PRIMARY KEY IDENTITY,
    Description NVARCHAR(255),
    Rate        DECIMAL(18, 4) NOT NULL,
    CHECK (Rate BETWEEN 0 AND 10),
    ProductId   INT            NOT NULL FOREIGN KEY REFERENCES Products (Id),
    CustomerId  INT            NOT NULL FOREIGN KEY REFERENCES Customers (Id)
)

CREATE TABLE Distributors
(
    Id          INT PRIMARY KEY IDENTITY,
    Name        NVARCHAR(25) NOT NULL UNIQUE,
    AddressText NVARCHAR(30) NOT NULL,
    Summary     NVARCHAR(200),
    CountryId   INT          NOT NULL FOREIGN KEY REFERENCES Countries (Id)
)

CREATE TABLE Ingredients
(
    Id              INT PRIMARY KEY IDENTITY,
    Name            NVARCHAR(30)  NOT NULL,
    Description     NVARCHAR(200) NOT NULL,
    OriginCountryId INT           NOT NULL FOREIGN KEY REFERENCES Countries (Id),
    DistributorId   INT           NOT NULL FOREIGN KEY REFERENCES Distributors (Id)
)

CREATE TABLE ProductsIngredients
(
    ProductId    INT NOT NULL FOREIGN KEY REFERENCES Products (Id),
    IngredientId INT NOT NULL FOREIGN KEY REFERENCES Ingredients (Id),

    CONSTRAINT PK_ProductsIngredients
        PRIMARY KEY (ProductId, IngredientId)
)

--Section 2--

-- 02. Insert
INSERT INTO Distributors(Name,CountryId,AddressText,Summary)
VALUES
('Deloitte & Touche', 2,'6 Arch St #9757','Customizable neutral traveling'),
('Congress Title',13,'58 Hancock St','Customer loyalty'),
('Kitchen People',1,'3 E 31st St #77','Triple-buffered stable delivery'),
('General Color Co Inc',21,'6185 Bohn St #72','Focus group'),
('Beck Corporation',23,'21 E 64th Ave','Quality-focused 4th generation hardware')

INSERT INTO Customers(FirstName,LastName,Age,Gender,PhoneNumber,CountryId)
VALUES
('Francoise','Rautenstrauch',15	,'M','0195698399',5),
('Kendra','Loud',22,'F','0063631526',11),
('Lourdes','Bauswell',50,'M','0139037043',8),
('Hannah','Edmison',18,'F','0043343686',1),
('Tom','Loeza',31,'M','0144876096',23),
('Queenie','Kramarczyk',30,'F','0064215793',29),
('Hiu','Portaro',25,'M','0068277755',16),
('Josefa','Opitz',43,'F','0197887645',17)

-- 03. Update
UPDATE Ingredients
SET DistributorId = 35
WHERE [Name] IN ('Bay Leaf','Paprika','Poppy')

UPDATE Ingredients
SET OriginCountryId = 14
WHERE [OriginCountryId] = 8

-- 04. Delete

DELETE Feedbacks
WHERE CustomerId = 14 OR  ProductId = 5

---Section 3---

-- 05. Products By Price

SELECT [Name],Price,Description FROM Products
ORDER BY Price DESC, Name ASC

-- 06. Negative Feedback
SELECT f.ProductId,f.Rate,f.Description,f.CustomerId,c.Age,c.Gender
FROM Feedbacks f
JOIN Customers c
ON f.CustomerId = c.Id
WHERE f.Rate < 5.0
ORDER BY ProductId DESC,Rate ASC

-- 07. Customers without Feedback
SELECT 
CONCAT(c.FirstName,' ',c.LastName) AS CustomerName,
c.PhoneNumber,
c.Gender
FROM Customers c
LEFT JOIN Feedbacks f ON c.Id = f.CustomerId
WHERE f.Id IS NULL

-- 08. Customers by Criteria
SELECT c.FirstName,c.Age,c.PhoneNumber
FROM Customers c
JOIN Countries c2
ON c.CountryId = c2.Id
WHERE (Age >= 21 AND FirstName LIKE '%an%') OR (PhoneNumber LIKE '%38' AND c2.Name != 'Greece')
ORDER BY c.FirstName ASC, c.Age DESC

-- 09. Middle Range Distributors
SELECT d.Name as [DistributorName],
i.Name as [IngredientName],
p.Name [ProductName],
AVG(f.Rate) AS [AverageRate]
FROM Distributors d 
JOIN Ingredients i
ON i.DistributorId = d.Id
JOIN ProductsIngredients as [pi]
ON pi.IngredientId = i.Id
JOIN Products as p
ON pi.ProductId = p.Id
JOIN Feedbacks as f
ON p.Id = f.ProductId
GROUP BY d.Name ,i.Name,p.Name
HAVING AVG(f.Rate) BETWEEN 5 and 8
ORDER BY d.Name ,i.Name,p.Name

-- 10. Country Representative

SELECT 
temp.CountryName,
temp.DisributorName
FROM
	(SELECT 
		c.Name AS [CountryName],
		d.Name AS DisributorName,
		DENSE_RANK() OVER (PARTITION BY c.Name ORDER BY COUNT(i.Id) DESC) AS Ranking
			FROM Countries c
				JOIN Distributors d 
				ON c.Id = d.CountryId
				LEFT JOIN Ingredients i
				ON d.Id = i.DistributorId
				GROUP BY c.Name,d.Name) AS temp
WHERE temp.Ranking = 1
GROUP BY CountryName, DisributorName
ORDER BY CountryName, DisributorName

---Section 4---
-- 11. Customers With Countries

CREATE VIEW v_UserWithCountries 
AS
SELECT 
CONCAT(c.FirstName,' ',c.LastName) AS [CustomerName],
c.Age,
c.Gender,
c2.[Name]
FROM Customers c
JOIN Countries c2
ON c.CountryId = c2.Id

-- 12.	Delete Products
CREATE TRIGGER tr_DeleteProducts
    ON Products
    INSTEAD OF DELETE AS
BEGIN
    DECLARE @deletedProducts INT = (
        SELECT p.Id
            FROM Products AS p
                     JOIN deleted AS d
                          ON p.Id = d.Id)

    DELETE
        FROM Feedbacks
        WHERE ProductId = @deletedProducts

    DELETE
        FROM ProductsIngredients
        WHERE ProductId = @deletedProducts

    DELETE Products
        WHERE Id = @deletedProducts
END
    DELETE
        FROM Products
        WHERE Id = 7