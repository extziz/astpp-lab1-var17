using System;
using System.Collections.Generic;
using System.Linq;
using CalcClassBr;
using ErrorLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalcClassBr.Tests
{
    /// <summary>
    /// Лабораторна робота №1. Варіант 17.
    /// Модульні тести методу CalcClassBr.CalcClass.Sub(long a, long b).
    ///
    /// Набір тестових даних зберігається в базі даних (Database/TestData.db)
    /// і зчитується через TestDataRepository. Кожна категорія даних відповідає
    /// окремій гілці коду методу Sub, що забезпечує повне покриття.
    /// </summary>
    [TestClass]
    public class SubTests
    {
        private const long MAXINT = int.MaxValue;   //  2147483647
        private const long MININT = int.MinValue;   // -2147483648

        #region Джерела даних з бази даних

        public static IEnumerable<object[]> ValidCases()
        {
            return TestDataRepository.GetSubTestCases("Valid")
                                     .Select(c => new object[] { c });
        }

        public static IEnumerable<object[]> ArgOutOfRangeCases()
        {
            return TestDataRepository.GetSubTestCases("ArgOutOfRange")
                                     .Select(c => new object[] { c });
        }

        public static IEnumerable<object[]> ResultOverflowCases()
        {
            return TestDataRepository.GetSubTestCases("ResultOverflow")
                                     .Select(c => new object[] { c });
        }

        public static IEnumerable<object[]> AllErrorCases()
        {
            return TestDataRepository.GetSubTestCases()
                                     .Where(c => c.ExpectsError)
                                     .Select(c => new object[] { c });
        }

        #endregion

        #region Категорія 1 — коректні обчислення (п. 4.2.2.1 специфікації)

        /// <summary>
        /// Для аргументів у межах [MININT; MAXINT], різниця яких також лежить
        /// у цих межах, метод повинен повертати правильну з погляду математики різницю.
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(ValidCases), DynamicDataSourceType.Method)]
        public void Sub_OperandsAndResultInRange_ReturnsCorrectDifference(SubTestCase testCase)
        {
            int actual = CalcClass.Sub(testCase.OperandA, testCase.OperandB);

            Assert.AreEqual(testCase.ExpectedResult.Value, actual,
                "Неправильний результат віднімання. " + testCase);
        }

        /// <summary>
        /// Після успішного обчислення властивість lastError повинна бути порожньою.
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(ValidCases), DynamicDataSourceType.Method)]
        public void Sub_OperandsAndResultInRange_ClearsLastError(SubTestCase testCase)
        {
            CalcClass.Sub(testCase.OperandA, testCase.OperandB);

            Assert.AreEqual(string.Empty, CalcClass.lastError,
                "Після коректного обчислення lastError має бути порожнім. " + testCase);
        }

        #endregion

        #region Категорія 2 — аргумент поза межами int (п. 4.2.2.2 специфікації)

        /// <summary>
        /// Якщо хоча б один з аргументів виходить за межі int,
        /// метод повинен кидати ArgumentOutOfRangeException.
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(ArgOutOfRangeCases), DynamicDataSourceType.Method)]
        public void Sub_ArgumentOutOfIntRange_ThrowsArgumentOutOfRangeException(SubTestCase testCase)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => CalcClass.Sub(testCase.OperandA, testCase.OperandB),
                "Очікувався ArgumentOutOfRangeException. " + testCase);
        }

        #endregion

        #region Категорія 3 — переповнення результату (п. 4.2.2.2 специфікації)

        /// <summary>
        /// Якщо аргументи в межах int, але їх різниця виходить за межі int,
        /// метод повинен кидати ArgumentOutOfRangeException.
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(ResultOverflowCases), DynamicDataSourceType.Method)]
        public void Sub_ResultOutOfIntRange_ThrowsArgumentOutOfRangeException(SubTestCase testCase)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => CalcClass.Sub(testCase.OperandA, testCase.OperandB),
                "Очікувався ArgumentOutOfRangeException через переповнення. " + testCase);
        }

        #endregion

        #region Перевірка повідомлення про помилку (поле lastError)

        /// <summary>
        /// У будь-якому помилковому випадку в поле lastError має бути записано
        /// повідомлення Error 06 (ERROR_06 з бібліотеки ErrorLibrary).
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(AllErrorCases), DynamicDataSourceType.Method)]
        public void Sub_ErrorCase_SetsLastErrorToError06(SubTestCase testCase)
        {
            try
            {
                CalcClass.Sub(testCase.OperandA, testCase.OperandB);
                Assert.Fail("Очікувався виняток, але метод повернув результат. " + testCase);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.AreEqual(ErrorsExpression.ERROR_06, CalcClass.lastError,
                    "У lastError має бути записано ERROR_06. " + testCase);
                StringAssert.StartsWith(CalcClass.lastError, testCase.ExpectedError,
                    "Повідомлення про помилку має починатися з коду помилки. " + testCase);
            }
        }

        #endregion

        #region Додаткові перевірки властивостей методу

        /// <summary>
        /// Віднімання нуля не змінює зменшуваного (правий нейтральний елемент).
        /// </summary>
        [DataTestMethod]
        [DataRow(0L)]
        [DataRow(1L)]
        [DataRow(-1L)]
        [DataRow(12345L)]
        [DataRow(MAXINT)]
        [DataRow(MININT)]
        public void Sub_SubtractingZero_ReturnsOriginalValue(long a)
        {
            Assert.AreEqual((int)a, CalcClass.Sub(a, 0));
        }

        /// <summary>
        /// Різниця однакових чисел завжди дорівнює нулю.
        /// </summary>
        [DataTestMethod]
        [DataRow(0L)]
        [DataRow(7L)]
        [DataRow(-7L)]
        [DataRow(MAXINT)]
        [DataRow(MININT)]
        public void Sub_EqualOperands_ReturnsZero(long a)
        {
            Assert.AreEqual(0, CalcClass.Sub(a, a));
        }

        /// <summary>
        /// Перевірка узгодженості з еталонним обчисленням у типі long
        /// на межах діапазону int.
        /// </summary>
        [DataTestMethod]
        [DataRow(MAXINT, 1L, MAXINT - 1)]
        [DataRow(MININT, -1L, MININT + 1)]
        [DataRow(0L, MAXINT, -MAXINT)]
        [DataRow(-1L, MAXINT, MININT)]
        public void Sub_BoundaryValues_MatchesReferenceArithmetic(long a, long b, long expected)
        {
            Assert.AreEqual((int)expected, CalcClass.Sub(a, b));
        }

        /// <summary>
        /// Перевірка, що набір тестових даних дійсно завантажується з бази даних
        /// і містить усі три категорії випадків.
        /// </summary>
        [TestMethod]
        public void TestDatabase_ContainsAllCategories()
        {
            var all = TestDataRepository.GetSubTestCases().ToList();

            Assert.IsTrue(all.Count > 0, "База даних з тестовими даними порожня.");
            CollectionAssert.AreEquivalent(
                new[] { "Valid", "ArgOutOfRange", "ResultOverflow" },
                all.Select(c => c.Category).Distinct().ToList(),
                "У базі даних мають бути присутні всі три категорії тестових випадків.");
        }

        #endregion
    }
}
