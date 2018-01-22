using System;

namespace TradeApp.Oanda
{
    [Serializable]
    public class OandaApiException : Exception
    {
        private OandaApiException(ErrorInfo errorInfo) : base(String.Format(errorInfo.Message))
        {
            ErrorInfo = errorInfo;
        }

        public ErrorInfo ErrorInfo { get; }

        public static OandaApiException FromErrorInfo(ErrorInfo errorInfo)
        {
            return new OandaApiException(errorInfo);
        }
    }
}