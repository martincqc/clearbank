using System;
using System.Collections.Generic;
using System.Text;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Helpers;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Helpers
{
    public class ValidatorTests
    {
        [Fact]
        public void TestCreateValidator_should_get_Bacs_type()
        {
            //act
            var result = ValidatorFactory.CreateValidator("Bacs");
            //assert
            result.Should().BeOfType<BacsValidator>();
        }
        [Fact]
        public void TestCreateValidator_should_get_fasterpayments_type()
        {
            //act
            var result = ValidatorFactory.CreateValidator("FasterPayments");
            //assert
            result.Should().BeOfType<FasterPaymentsValidator>();
        }
        [Fact]
        public void TestCreateValidator_should_get_chaps_type()
        {
            //act
            var result = ValidatorFactory.CreateValidator("Chaps");
            //assert
            result.Should().BeOfType<ChapsValidator>();
        }
        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        [InlineData("xxxxxxxxxxxxxxxx")]
        public void TestCreateValidator_should_get_null_type(string key)
        {
            //act
            var result = ValidatorFactory.CreateValidator(key);
            //assert
            result.Should().BeNull();
        }
        [Theory]
        [InlineData("Bacs")]
        [InlineData("FasterPayments")]
        [InlineData("Chaps")]
        public void TestValidate_should_pass(string paymentScheme)
        {
            //arrange
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = (AllowedPaymentSchemes)Enum.Parse(typeof(AllowedPaymentSchemes), paymentScheme, true),
                Balance = 123,
                Status = AccountStatus.Live
            };
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 100,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = (PaymentScheme)Enum.Parse(typeof(PaymentScheme), paymentScheme, true)
            };

            //act
            var sut = ValidatorFactory.CreateValidator(paymentScheme);
            var result = sut.Validate(account, request);
            //assert
            result.Should().BeTrue();
        }
        [Theory]
        [InlineData("Bacs", "FasterPayments")]
        [InlineData("Bacs", "Chaps")]
        [InlineData("FasterPayments", "Bacs")]
        [InlineData("FasterPayments", "Chaps")]
        [InlineData("Chaps", "FasterPayments")]
        [InlineData("Chaps", "Bacs")]
        public void TestValidate_should_fail(string paymentScheme, string allowedPaymentScheme)
        {
            //arrange
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = (AllowedPaymentSchemes)Enum.Parse(typeof(AllowedPaymentSchemes), allowedPaymentScheme, true),
                Balance = 123,
                Status = AccountStatus.Live
            };
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 100,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = (PaymentScheme)Enum.Parse(typeof(PaymentScheme), paymentScheme, true)
            };

            //act
            var sut = ValidatorFactory.CreateValidator(paymentScheme);
            var result = sut.Validate(account, request);
            //assert
            result.Should().BeFalse();
        }
        [Theory]
        [InlineData(0, 100)]
        [InlineData(99, 100)]
        public void TestValidate_for_fasterpayments_should_fail_with_insufficient_funds(decimal balance, decimal amount)
        {
            //arrange
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = balance,
                Status = AccountStatus.Live
            };
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = amount,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            //act
            var sut = ValidatorFactory.CreateValidator("FasterPayments");
            var result = sut.Validate(account, request);
            //assert
            result.Should().BeFalse();
        }
        [Theory]
        [InlineData("Disabled")]
        [InlineData("InboundPaymentsOnly")]
        public void TestValidate_for_chaps_should_fail_with_account_not_live(string accountStatus)
        {
            //arrange
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), accountStatus, true)
            };
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 100,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.Chaps
            };

            //act
            var sut = ValidatorFactory.CreateValidator("Chaps");
            var result = sut.Validate(account, request);
            //assert
            result.Should().BeFalse();
        }
    }
}
