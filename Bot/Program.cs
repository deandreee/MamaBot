using System.Threading.Tasks;
using MaMa.HFT.Console.GlobalShared;
using MamaBot.GlobalShared;

namespace MaMa.HFT.Console
{
    public class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public static async Task Main(string[] args)
        {
            // set bot settings
            const string token = "BTCUSDT";

            Instance test = new Instance("BTCUSDT", Vars.BinanceApiKey, Vars.BinanceApiSecret);
            //var strategyConfig = new MarketStrategyConfiguration
            //{
            //    MinOrderVolume = 1m,
            //    MaxOrderVolume = 1m,
            //    TradeWhenSpreadGreaterThan = -.05M
            //};

            test.ObSub();

            //CandleService LoadData = new CandleService("BTCUSDT");



            try
            {
                //await bot.RunAsync();

                System.Console.WriteLine("Press Enter to stop bot...");
                System.Console.ReadLine();
            }
            finally
            {
                //bot.Stop();
            }
           
            System.Console.WriteLine("Press Enter to exit...");
            System.Console.ReadLine();
        }   
    }
}
