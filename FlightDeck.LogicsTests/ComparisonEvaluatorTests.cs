﻿using System.Collections.Generic;
using System.Linq;
using FlightDeck.Core;
using FlightDeck.Logics.Evaluators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightDeck.LogicsTests
{
    [TestClass]
    public class ComparisonEvaluatorTests
    {
        private void Workhorse(string currentValue, string comparisonValue, List<string> expectedTrueComparisons)
        {
            List<string> errors = new List<string>();

            var evaluator = new ComparisonEvaluator(new EnumConverter());

            //Act
            ComparisonEvaluator.AllowedComparisons.ForEach((string comparisonType) =>
            {
                bool status = evaluator.CompareValues(currentValue, comparisonValue, comparisonType);

                if (expectedTrueComparisons.Contains(comparisonType) && !status) errors.Add($"{comparisonType} was not {!status}, but should have been.");
                if (!expectedTrueComparisons.Contains(comparisonType) && status) errors.Add($"{comparisonType} was {status}, but should not have been.");
            });

            //Assert
            Assert.IsFalse(errors.Any(), string.Join(" | ", errors));
        }

        #region Numeric Comparison

        [TestMethod]
        public void CompareValuesTest_Same_Value_Numeric()
        {
            //Arrange
            string currentValue = "0";
            string comparisonValue = "0";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorEquals,
                ComparisonEvaluator.OperatorGreaterOrEquals,
                ComparisonEvaluator.OperatorLessOrEquals
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }

        [TestMethod]
        public void CompareValuesTest_Different_Value_Greater_Numeric()
        {
            //Arrange
            string currentValue = "1";
            string comparisonValue = "0";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorNotEquals,
                ComparisonEvaluator.OperatorGreaterOrEquals,
                ComparisonEvaluator.OperatorGreater
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }

        [TestMethod]
        public void CompareValuesTest_Different_Value_Less_Numeric()
        {
            //Arrange
            string currentValue = "-1";
            string comparisonValue = "0";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorNotEquals,
                ComparisonEvaluator.OperatorLessOrEquals,
                ComparisonEvaluator.OperatorLess
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }
        #endregion

        #region Alpha Comparison

        [TestMethod]
        public void CompareValuesTest_Same_Value_Alpha()
        {
            //Arrange
            string currentValue = "m";
            string comparisonValue = "m";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorEquals,
                ComparisonEvaluator.OperatorGreaterOrEquals,
                ComparisonEvaluator.OperatorLessOrEquals
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }

        [TestMethod]
        public void CompareValuesTest_Different_Value_Greater_Alpha()
        {
            //Arrange
            string currentValue = "n";
            string comparisonValue = "m";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorNotEquals,
                ComparisonEvaluator.OperatorGreaterOrEquals,
                ComparisonEvaluator.OperatorGreater
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }

        [TestMethod]
        public void CompareValuesTest_Different_Value_Less_Alpha()
        {
            //Arrange
            string currentValue = "l";
            string comparisonValue = "m";
            List<string> expectedTrueComparisons = new List<string> {
                ComparisonEvaluator.OperatorNotEquals,
                ComparisonEvaluator.OperatorLessOrEquals,
                ComparisonEvaluator.OperatorLess
            };

            //Act
            Workhorse(currentValue, comparisonValue, expectedTrueComparisons);
        }

        #endregion
    }
}