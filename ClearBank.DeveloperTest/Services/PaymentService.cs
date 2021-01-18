using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;
using ClearBank.DeveloperTest.Helpers;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private IAccount _account;
        private readonly IAppConfigProvider _appConfigProvider;
        private readonly IBackupAccountDataStore _backupAccountDataStore;
        private readonly IAccountDataStore _accountDataStore;

        public PaymentService(IAppConfigProvider appConfigProvider, IAccount account, IBackupAccountDataStore backupAccountDataStore, IAccountDataStore accountDataStore)
        {
            _appConfigProvider = appConfigProvider;
            _account = account;
            _backupAccountDataStore = backupAccountDataStore;
            _accountDataStore = accountDataStore;
        }
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = _appConfigProvider.AppSettings["DataStoreType"];

            _account = AccountHelper.GetAccount(dataStoreType, request.DebtorAccountNumber, _backupAccountDataStore, _accountDataStore);

            var result = new MakePaymentResult {Success = Validate(request)};

            if (result.Success)
            {
                result.Success = AccountHelper.UpdateAccount(dataStoreType, request.Amount, _account, _backupAccountDataStore, _accountDataStore);
            }

            return result;
        }

        private bool Validate(MakePaymentRequest request)
        {
            var isValid = false;

            if (_account != null)
            {
                var validator = ValidatorFactory.CreateValidator(Enum.GetName(typeof(PaymentScheme), request.PaymentScheme));
                isValid = validator.Validate(_account, request);
            }

            return isValid;
        }
    }
}
