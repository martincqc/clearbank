using System;
using System.Collections.Generic;
using System.Text;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Helpers
{
    public static class ValidatorFactory
    {
        private static readonly Dictionary<string, Func<IValidator>> ValidatorMap = new Dictionary<string, Func<IValidator>>()
        {
            {"Bacs", () => { return new BacsValidator(); }},
            {"FasterPayments", () => { return new FasterPaymentsValidator(); }},
            {"Chaps", () => { return new ChapsValidator(); }}
        };

        public static IValidator CreateValidator(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || (!ValidatorMap.ContainsKey(name)))
            {
                return null;
            }
            return ValidatorMap[name]();
        }
    }

    public class BacsValidator : IValidator
    {
        public bool Validate(IAccount account, MakePaymentRequest request)
        {
            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
    public class FasterPaymentsValidator : IValidator
    {
        public bool Validate(IAccount account, MakePaymentRequest request)
        {
            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
                   (account.Balance >= request.Amount);
        }
    }
    public class ChapsValidator : IValidator
    {
        public bool Validate(IAccount account, MakePaymentRequest request)
        {
            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
                   (account.Status == AccountStatus.Live);
        }
    }
}
