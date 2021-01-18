using System;
using System.Collections.Specialized;
using System.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Helpers;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentService
{
    public class MakePaymentTests
    {
        //test all payment types succeeds
        [Theory]
        [InlineData("Backup", "FasterPayments", "FasterPayments", 12345, 100)]
        [InlineData("Backup", "Bacs", "Bacs", 12345, 100)]
        [InlineData("Backup", "Chaps", "Chaps", 12345, 100)]
        [InlineData("Backup", "FasterPayments", "FasterPayments", 101, 100)]
        [InlineData("Backup", "Bacs", "Bacs", 101, 100)]
        [InlineData("Backup", "Chaps", "Chaps", 101, 100)]
        [InlineData("Backup", "FasterPayments", "FasterPayments", 100, 100)]
        [InlineData("Backup", "Bacs", "Bacs", 100, 100)]
        [InlineData("Backup", "Bacs", "Bacs", 99, 100)]//no account balance validation for Bacs
        [InlineData("Backup", "Bacs", "Bacs", 0, 100)]//no account balance validation for Bacs
        [InlineData("Backup", "Chaps", "Chaps", 100, 100)]
        [InlineData("SomethingElse", "FasterPayments", "FasterPayments", 12345, 100)]
        [InlineData("SomethingElse", "Bacs", "Bacs", 12345, 100)]
        [InlineData("SomethingElse", "Chaps", "Chaps", 12345, 100)]
        [InlineData("SomethingElse", "FasterPayments", "FasterPayments", 101, 100)]
        [InlineData("SomethingElse", "Bacs", "Bacs", 101, 100)]
        [InlineData("SomethingElse", "Chaps", "Chaps", 101, 100)]
        [InlineData("SomethingElse", "FasterPayments", "FasterPayments", 100, 100)]
        [InlineData("SomethingElse", "Bacs", "Bacs", 100, 100)]
        [InlineData("SomethingElse", "Bacs", "Bacs", 99, 100)]//no account balance validation for Bacs
        [InlineData("SomethingElse", "Bacs", "Bacs", 0, 100)]//no account balance validation for Bacs
        [InlineData("SomethingElse", "Chaps", "Chaps", 100, 100)]
        [InlineData("", "FasterPayments", "FasterPayments", 12345, 100)]
        [InlineData("", "Bacs", "Bacs", 12345, 100)]
        [InlineData("", "Chaps", "Chaps", 12345, 100)]
        [InlineData("", "FasterPayments", "FasterPayments", 101, 100)]
        [InlineData("", "Bacs", "Bacs", 101, 100)]
        [InlineData("", "Chaps", "Chaps", 101, 100)]
        [InlineData("", "FasterPayments", "FasterPayments", 100, 100)]
        [InlineData("", "Bacs", "Bacs", 100, 100)]
        [InlineData("", "Bacs", "Bacs", 99, 100)]//no account balance validation for Bacs
        [InlineData("", "Bacs", "Bacs", 0, 100)]//no account balance validation for Bacs
        [InlineData("", "Chaps", "Chaps", 100, 100)]
        [InlineData(null, "FasterPayments", "FasterPayments", 12345, 100)]
        [InlineData(null, "Bacs", "Bacs", 12345, 100)]
        [InlineData(null, "Chaps", "Chaps", 12345, 100)]
        [InlineData(null, "FasterPayments", "FasterPayments", 101, 100)]
        [InlineData(null, "Bacs", "Bacs", 101, 100)]
        [InlineData(null, "Chaps", "Chaps", 101, 100)]
        [InlineData(null, "FasterPayments", "FasterPayments", 100, 100)]
        [InlineData(null, "Bacs", "Bacs", 100, 100)]
        [InlineData(null, "Bacs", "Bacs", 99, 100)]//no account balance validation for Bacs
        [InlineData(null, "Bacs", "Bacs", 0, 100)]//no account balance validation for Bacs
        [InlineData(null, "Chaps", "Chaps", 100, 100)]
        public void Test_MakePayment_With_Correct_Parameters_Should_Succeed(string dataStoreType, string allowedPaymentSchemes, string paymentScheme, decimal accountBalance, decimal requestAmount)
        {
            //arrange
            var appConfig = new AppConfigProvider();
            appConfig.AppSettings.CustomValues.Add("DataStoreType", dataStoreType);
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();

            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = (AllowedPaymentSchemes)Enum.Parse(typeof(AllowedPaymentSchemes), allowedPaymentSchemes, true),
                Balance = accountBalance,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            var sut = new Services.PaymentService(appConfig, new Account(), mockBackupAccountDataStore.Object, mockAccountDataStore.Object);
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = requestAmount,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = (PaymentScheme)Enum.Parse(typeof(PaymentScheme), paymentScheme, true)
            };

            //act
            var result = sut.MakePayment(request);
            //assert
            result.Success.Should().Be(true);
        }
        //test chaps failures
        [Theory]
        [InlineData("Backup", "Disabled")]
        [InlineData("SomethingElse", "Disabled")]
        [InlineData("", "Disabled")]
        [InlineData(null, "Disabled")]
        [InlineData("Backup", "InboundPaymentsOnly")]
        [InlineData("SomethingElse", "InboundPaymentsOnly")]
        [InlineData("", "InboundPaymentsOnly")]
        public void Test_Make_Chaps_Payment_With_Account_Not_Live_Should_Fail(string dataStoreType, string accountStatus)
        {
            //arrange
            var appConfig = new AppConfigProvider();
            appConfig.AppSettings.CustomValues.Add("DataStoreType", dataStoreType);
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();

            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), accountStatus, true)
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            var sut = new Services.PaymentService(appConfig, new Account(), mockBackupAccountDataStore.Object, mockAccountDataStore.Object);
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 100,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.Chaps
            };

            //act
            var result = sut.MakePayment(request);
            //assert
            result.Success.Should().Be(false);
        }
        //test faster payments failures
        [Theory]
        [InlineData("Backup", 99.99, 100)]
        [InlineData("Backup", 1, 100)]
        [InlineData("Backup", 0, 100)]
        [InlineData("SomethingElse", 99.99, 100)]
        [InlineData("SomethingElse", 1, 100)]
        [InlineData("SomethingElse", 0, 100)]
        [InlineData("", 99.99, 100)]
        [InlineData("", 1, 100)]
        [InlineData("", 0, 100)]
        [InlineData(null, 99.9, 100)]
        [InlineData(null, 1, 100)]
        [InlineData(null, 0, 100)]
        public void Test_Make_FasterPayments_Payment_With_Low_Balance_Should_Fail(string dataStoreType, decimal accountBalance, decimal requestAmount)
        {
            //arrange
            var appConfig = new AppConfigProvider();
            appConfig.AppSettings.CustomValues.Add("DataStoreType", dataStoreType);
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();

            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = accountBalance,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            var sut = new Services.PaymentService(appConfig, new Account(), mockBackupAccountDataStore.Object, mockAccountDataStore.Object);
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = requestAmount,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            //act
            var result = sut.MakePayment(request);
            //assert
            result.Success.Should().Be(false);
        }
        [Theory]
        [InlineData("Bacs", "Backup")]
        [InlineData("Chaps", "Backup")]
        [InlineData("FasterPayments", "Backup")]
        [InlineData("Bacs", "SomethingElse")]
        [InlineData("Chaps", "SomethingElse")]
        [InlineData("FasterPayments", "SomethingElse")]
        public void Test_MakePayment_With_Null_AccountShould_Fail(string paymentScheme, string dataStoreType)
        {
            //arrange
            var appConfig = new AppConfigProvider();
            appConfig.AppSettings.CustomValues.Add("DataStoreType", dataStoreType);
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();

            Account returnAccount = null;

            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            var sut = new Services.PaymentService(appConfig, new Account(), mockBackupAccountDataStore.Object, mockAccountDataStore.Object);
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 123,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = (PaymentScheme)Enum.Parse(typeof(PaymentScheme), paymentScheme, true)
            };

            //act
            var result = sut.MakePayment(request);
            //assert
            result.Success.Should().Be(false);
        }
        [Theory]
        [InlineData("Bacs", "Chaps", "Backup")]
        [InlineData("Bacs", "FasterPayments", "Backup")]
        [InlineData("Chaps", "Bacs", "Backup")]
        [InlineData("Chaps", "FasterPayments", "Backup")]
        [InlineData("FasterPayments", "Bacs", "Backup")]
        [InlineData("FasterPayments", "Chaps", "Backup")]
        [InlineData("Bacs", "Chaps", "SomethingElse")]
        [InlineData("Bacs", "FasterPayments", "SomethingElse")]
        [InlineData("Chaps", "Bacs", "SomethingElse")]
        [InlineData("Chaps", "FasterPayments", "SomethingElse")]
        [InlineData("FasterPayments", "Bacs", "SomethingElse")]
        [InlineData("FasterPayments", "Chaps", "SomethingElse")]
        public void Test_MakePayment_With_MissMatched_PaymentSchemes_Should_Fail(string allowedPaymentSchemes, string paymentScheme, string dataStoreType)
        {
            //arrange
            var appConfig = new AppConfigProvider();
            appConfig.AppSettings.CustomValues.Add("DataStoreType", dataStoreType);
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();

            var returnAccount = new Account
            {
                AccountNumber = "12345",
                Balance = 12345,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = (AllowedPaymentSchemes)Enum.Parse(typeof(AllowedPaymentSchemes), allowedPaymentSchemes, true),
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            var sut = new Services.PaymentService(appConfig, new Account(), mockBackupAccountDataStore.Object, mockAccountDataStore.Object);
            var request = new MakePaymentRequest
            {
                CreditorAccountNumber = "12345",
                Amount = 123,
                DebtorAccountNumber = "23456",
                PaymentDate = DateTime.Now,
                PaymentScheme = (PaymentScheme)Enum.Parse(typeof(PaymentScheme), paymentScheme, true)
            };

            //act
            var result = sut.MakePayment(request);
            //assert
            result.Success.Should().Be(false);
        }
    }
}
