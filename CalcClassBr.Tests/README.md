# Лабораторна робота №1 — Модульне тестування ПЗ

**Варіант 17:** бібліотека `CalcClassBr`, метод `Sub`.

## Структура

| Шлях | Призначення |
|---|---|
| `SubTests.cs` | Модульні тести методу `CalcClass.Sub` |
| `TestDataRepository.cs` | Доступ до БД з наборами тестових даних |
| `Database/CreateDatabase.sql` | Скрипт створення БД для MS SQL Server |
| `Database/CreateDatabase.sqlite.sql` | Той самий набір даних для SQLite |
| `Database/TestData.db` | Готова БД SQLite (використовується тестами) |

## Структура бази даних

**`TestCategories`** — довідник категорій (3 записи), кожна відповідає гілці коду методу `Sub`.

**`SubTestCases`** — 30 тестових випадків:

| Категорія | К-сть | Опис |
|---|---|---|
| `Valid` | 17 | Аргументи й різниця в межах `int` — очікується коректний результат |
| `ArgOutOfRange` | 7 | Аргумент поза межами `int` — очікується `ArgumentOutOfRangeException` |
| `ResultOverflow` | 6 | Різниця поза межами `int` — очікується `ArgumentOutOfRangeException` |

## Запуск

Усі команди виконуються з кореневої теки рішення (`Calculator/`).

### Прогін тестів

```bash
# Стислий підсумок
dotnet test CalcClassBr.Tests

# Детальний список усіх 76 випадків (для демонстрації)
dotnet test CalcClassBr.Tests --logger "console;verbosity=normal"

# Прогін із вимірюванням покриття коду
dotnet test CalcClassBr.Tests --collect:"XPlat Code Coverage"
```

### Демонстрація методу Sub

```bash
# Діалоговий режим
dotnet run --project SubDemo

# Одноразове обчислення
dotnet run --project SubDemo -- 10 3
```

### Калькулятор з графічним інтерфейсом

`GraphInterface` — застосунок WinForms (.NET Framework 4.7.2), тому запускається
лише у Windows: відкрити `Calculator_Exam_CommandProject.sln` у Visual Studio
і натиснути F5, або запустити готовий `GraphInterface/bin/Debug/GraphInterface.exe`.

Перестворити БД SQLite за потреби:

```bash
cd CalcClassBr.Tests/Database
rm -f TestData.db && sqlite3 TestData.db < CreateDatabase.sqlite.sql
```

## Результати

- **76 тестів, усі проходять** (0 помилок).
- **Покриття методу `Sub`: 100% рядків і 100% гілок** (8/8 і 4/4 умовних переходів).
