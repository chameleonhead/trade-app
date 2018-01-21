using CommandLine;
using System;

namespace TradeApp
{
    public class TradeAppOption
    {
        [Option("server", HelpText = "OANDA�T�[�o�[��URL���w�肵�܂�", Required = true)]
        public Uri OandaServerBaseUri { get; set; }
        [Option("token", HelpText = "OANDA�T�[�o�[�̃A�N�Z�X�g�[�N�����w�肵�܂�", Required = true)]
        public string OandaAccessToken { get; set; }
        [Option("accountId", HelpText = "OANDA�̃A�J�E���g���w�肵�܂�", Required = true)]
        public int OandaAccountId { get; set; }
        [Option("gracefulshutdownfile", HelpText = "�V���b�g�_�E���p�̊Ď��t�@�C�����w�肵�܂�", Required = true)]
        public string ShutdownWatchFile { get; set; }
    }
}