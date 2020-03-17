using System;
using System.Collections.Generic;
using System.Text;

namespace MamaBot.GlobalShared
{
    public static class Vars
    {
        public static string BinanceApiKey = Environment.GetEnvironmentVariable("ApiKey", EnvironmentVariableTarget.User);
        public static string BinanceApiSecret = Environment.GetEnvironmentVariable("ApiSecret", EnvironmentVariableTarget.User);


    }
}
