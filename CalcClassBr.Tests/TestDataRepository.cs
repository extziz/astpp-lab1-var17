using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace CalcClassBr.Tests
{
    /// <summary>
    /// Один рядок таблиці SubTestCases — окремий тестовий випадок для методу Sub.
    /// </summary>
    public class SubTestCase
    {
        public int TestCaseId { get; set; }
        public string Category { get; set; }
        public long OperandA { get; set; }
        public long OperandB { get; set; }
        public long? ExpectedResult { get; set; }
        public bool ExpectsError { get; set; }
        public string ExpectedError { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Короткий підпис випадку — потрапляє у повідомлення про помилку тесту,
        /// щоб одразу було видно, який саме рядок БД не пройшов перевірку.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[#{0}, {1}] Sub({2}, {3}) — {4}",
                TestCaseId, Category, OperandA, OperandB, Description);
        }
    }

    /// <summary>
    /// Доступ до бази даних з наборами тестових даних.
    /// Дані зберігаються у файлі TestData.db, який копіюється у теку збірки.
    /// </summary>
    public static class TestDataRepository
    {
        private const string DatabaseFileName = "TestData.db";

        private static string GetDatabasePath()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Database", DatabaseFileName);
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    "Не знайдено файл бази даних з тестовими даними: " + path);
            return path;
        }

        /// <summary>
        /// Зчитує всі тестові випадки для методу Sub.
        /// </summary>
        /// <param name="category">
        /// Назва категорії (Valid, ArgOutOfRange, ResultOverflow) або null — усі випадки.
        /// </param>
        public static IEnumerable<SubTestCase> GetSubTestCases(string category = null)
        {
            var result = new List<SubTestCase>();

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = GetDatabasePath(),
                Mode = SqliteOpenMode.ReadOnly
            };

            using (var connection = new SqliteConnection(builder.ToString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT t.TestCaseId, c.CategoryName, t.OperandA, t.OperandB,
                                 t.ExpectedResult, t.ExpectsError, t.ExpectedError, t.Description
                          FROM SubTestCases t
                          JOIN TestCategories c ON c.CategoryId = t.CategoryId
                          WHERE (@category IS NULL OR c.CategoryName = @category)
                          ORDER BY t.TestCaseId;";
                    command.Parameters.AddWithValue("@category", (object)category ?? DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new SubTestCase
                            {
                                TestCaseId = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                OperandA = reader.GetInt64(2),
                                OperandB = reader.GetInt64(3),
                                ExpectedResult = reader.IsDBNull(4) ? (long?)null : reader.GetInt64(4),
                                ExpectsError = reader.GetInt64(5) != 0,
                                ExpectedError = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Description = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
