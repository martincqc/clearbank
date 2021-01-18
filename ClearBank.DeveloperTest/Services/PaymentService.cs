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

            _account = dataStoreType == "Backup" ? _backupAccountDataStore.GetAccount(request.DebtorAccountNumber) : _accountDataStore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult {Success = Validate(request)};

            if (result.Success)
            {
                _account.Balance -= request.Amount;

                if (dataStoreType == "Backup")
                {
                    _backupAccountDataStore.UpdateAccount(_account);
                }
                else
                {
                    _accountDataStore.UpdateAccount(_account);
                }
            }

            return result;
        }

        private bool Validate(MakePaymentRequest request)
        {
            var isValid = false;

            if (_account != null)
            {
                switch (request.PaymentScheme)
                {
                    case PaymentScheme.Bacs:
                        isValid = _account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
                        break;

                    case PaymentScheme.FasterPayments:

                        isValid = (_account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
                                      (_account.Balance >= request.Amount));
                        break;

                    case PaymentScheme.Chaps:
                        isValid = (_account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
                                   (_account.Status == AccountStatus.Live));
                        break;
                }

            }

            return isValid;
        }
    }
}
