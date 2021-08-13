﻿namespace Plastic.UnitTests
{
    using Xunit;
    using FluentAssertions;
    using AutoFixture.Xunit2;

    public class ExecutionResultTests
    {
        [Theory]
        [InlineData(true, default(string))]
        [InlineData(false, default(string))]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, "Seotaiji")]
        [InlineData(false, "Damien rice")]
        [AutoData]
        public void Constructor_does_construct_ExecutionResult_with_given_parameters(
            bool expectedSuccess, string? expectedMessage)
        {
            // Arrange
            // Act
            var sut = new ExecutionResult(expectedSuccess, expectedMessage);

            // Assert
            sut.Result.Should().Be(expectedSuccess);
            sut.Message.Should().Be(expectedMessage);
        }

        [Theory]
        [InlineData(true, default(string))]
        [InlineData(false, default(string))]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, "Seotaiji")]
        [InlineData(false, "Damien rice")]
        [AutoData]
        public void Constructor_does_construct_ExecutionResult_with_given_Response(
            bool expectedSuccess, string? expectedMessage)
        {
            // Arrange
            var expectedState = new Response(expectedSuccess, expectedMessage);

            // Act
            var sut = new ExecutionResult(expectedState);

            // Assert
            sut.Result.Should().Be(expectedState.Result);
            sut.Message.Should().Be(expectedState.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasSucceed_does_return_correctly(bool expectedSuccess)
        {
            // Arrange
            var sut = new ExecutionResult(expectedSuccess, string.Empty);

            // Act
            bool succeed = sut.HasSucceeded();

            // Assert
            succeed.Should().Be(expectedSuccess);
        }
    }
}
