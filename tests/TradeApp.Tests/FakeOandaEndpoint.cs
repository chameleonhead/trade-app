using System;

namespace TradeApp
{
    class FakeOandaServer
    {
        private string accessToken;

        public Uri BaseUri { get; internal set; }

        public FakeOandaServer(string accessToken)
        {
            this.accessToken = accessToken;
        }

        internal void Stop()
        {
            throw new NotImplementedException();
        }

        internal void HasReceivedAccountRequest()
        {
            throw new NotImplementedException();
        }
    }
}