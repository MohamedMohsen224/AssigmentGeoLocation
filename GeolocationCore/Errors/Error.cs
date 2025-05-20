using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Geolocation.Core.Enums;

namespace Geolocation.Core.Errors
{
    public sealed class Error : ValueObject
    {
        private const string Separator = "/";
        public ErrorCodes Code { get; }
        public string? Message { get; }

        internal Error(ErrorCodes code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Code;
        }

        public override string ToString()
        {
            return $"{Code}{Separator}{Message}";
        }
    }
    public static class Errors
    {
        public static Error GeneralMessage(ErrorCodes code, string message)
        {
            return new Error(code, message);
        }
    }

}
