-- =============================================================
-- Лабораторна робота №1. Варіант 17.
-- Бібліотека CalcClassBr, метод Sub(long a, long b)
-- Скрипт створення бази даних з наборами тестових даних.
-- Діалект: SQLite (використовується для автоматичного прогону тестів).
-- Еквівалентний скрипт для MS SQL Server — у файлі CreateDatabase.sql
-- =============================================================

PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS SubTestCases;
DROP TABLE IF EXISTS TestCategories;

CREATE TABLE TestCategories
(
    CategoryId   INTEGER           NOT NULL PRIMARY KEY,
    CategoryName TEXT  NOT NULL,
    Description  TEXT NOT NULL
);

INSERT INTO TestCategories (CategoryId, CategoryName, Description) VALUES
    (1, 'Valid',           'Обидва аргументи в межах int і різниця в межах int — повертається коректний результат (п. 4.2.2.1 специфікації).'),
    (2, 'ArgOutOfRange',   'Хоча б один аргумент поза межами int — очікується ArgumentOutOfRangeException та Error 06 (п. 4.2.2.2).'),
    (3, 'ResultOverflow',  'Аргументи в межах int, але різниця виходить за межі int — очікується ArgumentOutOfRangeException та Error 06 (п. 4.2.2.2).');

-- Набір тестових даних для методу Sub
CREATE TABLE SubTestCases
(
    TestCaseId      INTEGER            NOT NULL PRIMARY KEY,
    CategoryId      INTEGER            NOT NULL,
    OperandA        INTEGER         NOT NULL,
    OperandB        INTEGER         NOT NULL,
    ExpectedResult  INTEGER         NULL,          -- NULL, якщо очікується виняток
    ExpectsError    INTEGER            NOT NULL,      -- 1 — очікується виняток
    ExpectedError   TEXT  NULL,          -- очікуване значення CalcClass.lastError
    Description     TEXT  NOT NULL,
    CONSTRAINT FK_SubTestCases_Category FOREIGN KEY (CategoryId)
        REFERENCES TestCategories (CategoryId)
);

-- --------- Категорія 1: коректні обчислення ---------
INSERT INTO SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    ( 1, 1,           10,            3,           7, 0, '', 'Два додатних числа, звичайний випадок'),
    ( 2, 1,            3,           10,          -7, 0, '', 'Зменшуване менше від’ємника — від’ємний результат'),
    ( 3, 1,            0,            0,           0, 0, '', 'Нуль мінус нуль'),
    ( 4, 1,            5,            0,           5, 0, '', 'Віднімання нуля не змінює числа'),
    ( 5, 1,            0,            5,          -5, 0, '', 'Нуль мінус додатне число'),
    ( 6, 1,           -7,           -7,           0, 0, '', 'Однакові від’ємні числа дають нуль'),
    ( 7, 1,          -10,            5,         -15, 0, '', 'Від’ємне мінус додатне'),
    ( 8, 1,           -10,           -5,          -5, 0, '', 'Від’ємне мінус від’ємне'),
    ( 9, 1,   2147483647,            0,  2147483647, 0, '', 'MAXINT мінус нуль — верхня межа діапазону'),
    (10, 1,  -2147483648,            0, -2147483648, 0, '', 'MININT мінус нуль — нижня межа діапазону'),
    (11, 1,   2147483647,   2147483647,           0, 0, '', 'MAXINT мінус MAXINT'),
    (12, 1,  -2147483648,  -2147483648,           0, 0, '', 'MININT мінус MININT'),
    (13, 1,            0,   2147483647, -2147483647, 0, '', 'Нуль мінус MAXINT — результат у межах int'),
    (14, 1,   2147483647,            1,  2147483646, 0, '', 'На одиницю менше від MAXINT'),
    (15, 1,  -2147483647,            1, -2147483648, 0, '', 'Результат точно дорівнює MININT'),
    (16, 1,   1073741823,  -1073741824,  2147483647, 0, '', 'Результат точно дорівнює MAXINT'),
    (17, 1,          100,          100,           0, 0, '', 'Однакові додатні числа дають нуль');

-- --------- Категорія 2: аргумент поза межами int ---------
INSERT INTO SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    (18, 2,   2147483648,            1, NULL, 1, 'Error 06', 'Перший аргумент на одиницю більший за MAXINT'),
    (19, 2,  -2147483649,            1, NULL, 1, 'Error 06', 'Перший аргумент на одиницю менший за MININT'),
    (20, 2,            1,   2147483648, NULL, 1, 'Error 06', 'Другий аргумент на одиницю більший за MAXINT'),
    (21, 2,            1,  -2147483649, NULL, 1, 'Error 06', 'Другий аргумент на одиницю менший за MININT'),
    (22, 2,   4294967296,   4294967296, NULL, 1, 'Error 06', 'Обидва аргументи поза межами int'),
    (23, 2,  9223372036854775807,      1, NULL, 1, 'Error 06', 'Перший аргумент — максимальне значення long'),
    (24, 2, -9223372036854775808,      1, NULL, 1, 'Error 06', 'Перший аргумент — мінімальне значення long');

-- --------- Категорія 3: переповнення результату ---------
INSERT INTO SubTestCases
    (TestCaseId, CategoryId, OperandA, OperandB, ExpectedResult, ExpectsError, ExpectedError, Description)
VALUES
    (25, 3,   2147483647,  -2147483648, NULL, 1, 'Error 06', 'Різниця більша за MAXINT'),
    (26, 3,  -2147483648,   2147483647, NULL, 1, 'Error 06', 'Різниця менша за MININT'),
    (27, 3,   2147483647,           -1, NULL, 1, 'Error 06', 'Різниця на одиницю більша за MAXINT'),
    (28, 3,  -2147483648,            1, NULL, 1, 'Error 06', 'Різниця на одиницю менша за MININT'),
    (29, 3,  -2147483648,  2147483647, NULL, 1, 'Error 06', 'Максимальне за модулем від’ємне переповнення'),
    (30, 3,   1073741824,  -1073741824, NULL, 1, 'Error 06', 'Сума половин діапазону переповнює int');
