using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Helpers;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Helpers
{
    public class AccountHelperTests
    {
        [Fact]
        public void TestGetAccount_should_call_backup_datastore()
        {
            //arrange
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount).Verifiable();
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            //act
            var account = AccountHelper.GetAccount("Backup", "12345", mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            account.Should().NotBeNull();
            mockBackupAccountDataStore.Verify();
        }
        [Fact]
        public void TestGetAccount_should_call_account_datastore()
        {
            //arrange
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount).Verifiable();

            //act
            var account = AccountHelper.GetAccount("SomethingElse", "12345", mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            account.Should().NotBeNull();
            mockAccountDataStore.Verify();
        }
        [Fact]
        public void TestGetAccount_for_backup_datastore_should_handle_execption()
        {
            //arrange
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Throws(new Exception());
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);

            //act
            var account = AccountHelper.GetAccount("Backup", "12345", mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            account.Should().BeNull();
        }
        [Fact]
        public void TestGetAccount_for_datastore_should_handle_execption()
        {
            //arrange
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var returnAccount = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(returnAccount);
            mockAccountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Throws(new Exception());

            //act
            var account = AccountHelper.GetAccount("SomethingElse", "12345", mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            account.Should().BeNull();
        }
        [Fact]
        public void TestUpdateAccount_should_call_account_datastore()
        {
            //arrange
            var balance = 123;
            var amount = 10;
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>()));
            mockAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>())).Verifiable();

            //act
            var result = AccountHelper.UpdateAccount("SomethingElse", amount, account, mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            result.Should().BeTrue();
            mockAccountDataStore.Verify();
            account.Balance.Should().Be(balance - amount);
        }
        [Fact]
        public void TestUpdateAccount_should_call_backup_datastore()
        {
            //arrange
            var balance = 123;
            var amount = 10;
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>())).Verifiable();
            mockAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>()));

            //act
            var result = AccountHelper.UpdateAccount("Backup", amount, account, mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            result.Should().BeTrue();
            mockBackupAccountDataStore.Verify();
            account.Balance.Should().Be(balance - amount);
        }
        [Fact]
        public void TestUpdateAccount_for_backup_account_should_handle_exception()
        {
            //arrange
            var balance = 123;
            var amount = 10;
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>())).Throws(new Exception());
            mockAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>()));

            //act
            var result = AccountHelper.UpdateAccount("Backup", amount, account, mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            result.Should().BeFalse();
            account.Balance.Should().Be(balance);
        }
        [Fact]
        public void TestUpdateAccount_for_account_should_handle_exception()
        {
            //arrange
            var balance = 123;
            var amount = 10;
            var mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
            var mockAccountDataStore = new Mock<IAccountDataStore>();
            var account = new Account
            {
                AccountNumber = "12345",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 123,
                Status = AccountStatus.Live
            };
            mockBackupAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>()));
            mockAccountDataStore.Setup(x => x.UpdateAccount(It.IsAny<IAccount>())).Throws(new Exception());

            //act
            var result = AccountHelper.UpdateAccount("SomethingElse", amount, account, mockBackupAccountDataStore.Object,
                mockAccountDataStore.Object);
            //assert
            result.Should().BeFalse();
            account.Balance.Should().Be(balance);
        }
    }
}
