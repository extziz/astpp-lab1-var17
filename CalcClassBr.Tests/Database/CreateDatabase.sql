-- =============================================================
-- Лабораторна робота №1. Варіант 17.
-- Бібліотека CalcClassBr, метод Sub(long a, long b)
-- Скрипт створення бази даних з наборами тестових даних.
-- Діалект: MS SQL Server (T-SQL).
-- =============================================================

IF DB_ID('CalcSubTestsDB') IS NULL
    CREATE DATABASE CalcSubTestsDB;
GO

USE CalcSubTestsDB;
GO

IF OBJECT_ID('dbo.SubTestCases', 'U') IS NOT NULL
    DROP TABLE dbo.SubTestCases;
GO

IF OBJECT_ID('dbo.TestCategories', 'U') IS NOT NULL
    DROP TABLE dbo.TestCategories;
GO

-- Довідник категорій тестів (відповідає гілкам коду методу Sub)
CREATE TABLE dbo.TestCategories
(
    CategoryId   INT           NOT NULL PRIMARY KEY,
    CategoryName NVARCHAR(50)  NOT NULL,
    Description  NVARCHAR(300) NOT NULL
);
GO

INSERT INTO dbo.TestCategories (CategoryId, CategoryName, Description) VALUES
    (1, N'Valid',           N'Обидва аргументи в межах int і різниця в межах int — повертається коректний результат (п. 4.2.2.1 специфікації).'),
    (2, N'ArgOutOfRange',   N'Хоча б один аргумент поза межами int — очікується ArgumentOutOfRangeException та Error 06 (п. 4.2.2.2).'),
    (3, N'ResultOverflow',  N'Аргументи в межах int, але різниця виходить за межі int — очікується ArgumentOutOfRangeException та Error 06 (п. 4.2.2.2).');
GO

-- Набір тестових даних для методу Sub
CREATE TABLE dbo.SubTestCases
(
    TestCaseId      INT            NOT NULL PRIMARY KEY,
    CategoryId      INT            NOT NULL,
    OperandA        BIGINT         NOT NULL,
    OperandB        BIGINT         NOT NULL,
    ExpectedResult  BIGINT         NULL,          -- NULL, якщо очікується виняток
    ExpectsError    BIT            NOT NULL,      -- 1 — очікується виняток
    ExpectedError   NVARCHAR(300)  NULL,          -- очікуване значення CalcClass.lastError
    Description     NVARCHAR(300)  NOT NULL,
    CONSTRAINT FK_SubTestCases_Category FOREIGN KEY (CategoryId)
        REFERENCES dbo.TestCategories (CategoryId)
);
GO

-- --------- Категорія 1: коректні обчислення ---------
INSERT INTO dbo.SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    ( 1, 1,           10,            3,           7, 0, N'', N'Два додатних числа, звичайний випадок'),
    ( 2, 1,            3,           10,          -7, 0, N'', N'Зменшуване менше від’ємника — від’ємний результат'),
    ( 3, 1,            0,            0,           0, 0, N'', N'Нуль мінус нуль'),
    ( 4, 1,            5,            0,           5, 0, N'', N'Віднімання нуля не змінює числа'),
    ( 5, 1,            0,            5,          -5, 0, N'', N'Нуль мінус додатне число'),
    ( 6, 1,           -7,           -7,           0, 0, N'', N'Однакові від’ємні числа дають нуль'),
    ( 7, 1,          -10,            5,         -15, 0, N'', N'Від’ємне мінус додатне'),
    ( 8, 1,           -10,           -5,          -5, 0, N'', N'Від’ємне мінус від’ємне'),
    ( 9, 1,   2147483647,            0,  2147483647, 0, N'', N'MAXINT мінус нуль — верхня межа діапазону'),
    (10, 1,  -2147483648,            0, -2147483648, 0, N'', N'MININT мінус нуль — нижня межа діапазону'),
    (11, 1,   2147483647,   2147483647,           0, 0, N'', N'MAXINT мінус MAXINT'),
    (12, 1,  -2147483648,  -2147483648,           0, 0, N'', N'MININT мінус MININT'),
    (13, 1,            0,   2147483647, -2147483647, 0, N'', N'Нуль мінус MAXINT — результат у межах int'),
    (14, 1,   2147483647,            1,  2147483646, 0, N'', N'На одиницю менше від MAXINT'),
    (15, 1,  -2147483647,            1, -2147483648, 0, N'', N'Результат точно дорівнює MININT'),
    (16, 1,   1073741823,  -1073741824,  2147483647, 0, N'', N'Результат точно дорівнює MAXINT'),
    (17, 1,          100,          100,           0, 0, N'', N'Однакові додатні числа дають нуль');

-- --------- Категорія 2: аргумент поза межами int ---------
INSERT INTO dbo.SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    (18, 2,   2147483648,            1, NULL, 1, N'Error 06', N'Перший аргумент на одиницю більший за MAXINT'),
    (19, 2,  -2147483649,            1, NULL, 1, N'Error 06', N'Перший аргумент на одиницю менший за MININT'),
    (20, 2,            1,   2147483648, NULL, 1, N'Error 06', N'Другий аргумент на одиницю більший за MAXINT'),
    (21, 2,            1,  -2147483649, NULL, 1, N'Error 06', N'Другий аргумент на одиницю менший за MININT'),
    (22, 2,   4294967296,   4294967296, NULL, 1, N'Error 06', N'Обидва аргументи поза межами int'),
    (23, 2,  9223372036854775807,      1, NULL, 1, N'Error 06', N'Перший аргумент — максимальне значення long'),
    (24, 2, -9223372036854775808,      1, NULL, 1, N'Error 06', N'Перший аргумент — мінімальне значення long');

-- --------- Категорія 3: переповнення результату ---------
INSERT INTO dbo.SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    (25, 3,   2147483647,  -2147483648, NULL, 1, N'Error 06', N'Різниця більша за MAXINT'),
    (26, 3,  -2147483648,   2147483647, NULL, 1, N'Error 06', N'Різниця менша за MININT'),
    (27, 3,   2147483647,           -1, NULL, 1, N'Error 06', N'Різниця на одиницю більша за MAXINT'),
    (28, 3,  -2147483648,            1, NULL, 1, N'Error 06', N'Різниця на одиницю менша за MININT'),
    (29, 3,  -2147483648,  2147483647, NULL, 1, N'Error 06', N'Максимальне за модулем від’ємне переповнення'),
    (30, 3,   1073741824,  -1073741824, NULL, 1, N'Error 06', N'Сума половин діапазону переповнює int');
GO
