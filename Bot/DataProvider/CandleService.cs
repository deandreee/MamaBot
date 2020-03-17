using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core;

namespace MaMa.HFT.Console
{
    public class HistoGram
    {
        public List<AnalyzableTick<decimal?>> Data { get; set; }
        public AnalyzableTick<decimal?> Last { get; set; }
        public KlineInterval Interval { get; set; }
        public HistoGram(List<Candle> Source, KlineInterval period)
        {
            this.Last = Source.MacdHist(12, 26, 9)[Source.Count() - 1];
            this.Data = Source.MacdHist(12, 26, 9).ToList();
            this.Interval = period;
        }
    }

    public class CandleService
    {
        public BinanceClient client { get; set; } = new BinanceClient();
        public Dictionary<KlineInterval, HistoGram> HistoGramData = new Dictionary<KlineInterval, HistoGram>();
        public string KeyPair { get; set; } = string.Empty;
        public Dictionary<KlineInterval, List<Candle>> RawCandles = new Dictionary<KlineInterval, List<Candle>>();
        public DateTime StartTime { get; set; } = DateTime.Now;
        public CandleService(string keypair)
        {
            this.KeyPair = keypair;
            LoadKLineData();
        }
        internal void LoadKLineData()
        {
            this.RawCandles.Add(KlineInterval.OneHour, TransformCandle(client.GetKlines(KeyPair, KlineInterval.OneHour, startTime: DateTime.UtcNow.AddHours(-24), endTime: DateTime.UtcNow, limit: 1000).Data));
            this.RawCandles.Add(KlineInterval.FourHour, TransformCandle(client.GetKlines(KeyPair, KlineInterval.FourHour, startTime: DateTime.UtcNow.AddHours(-24), endTime: DateTime.UtcNow, limit: 1000).Data));
            this.RawCandles.Add(KlineInterval.ThirtyMinutes, TransformCandle(client.GetKlines(KeyPair, KlineInterval.ThirtyMinutes, startTime: DateTime.UtcNow.AddHours(-24), endTime: DateTime.UtcNow, limit: 1000).Data));

            //Interpret

            try
            {
                Parallel.ForEach(this.RawCandles, (source) =>
                {
                    this.HistoGramData.Add(source.Key, new HistoGram(source.Value, source.Key));
                });

            }
            catch
            {

            }
        }
        internal List<Candle> TransformCandle(IEnumerable<BinanceKline> data)
        {
            List<Candle> OutputList = new List<Candle>();
            Parallel.ForEach(data, (source) =>
            {
                OutputList.Add(new Candle(source.CloseTime, source.Open, source.High, source.Low, source.Close, source.Volume));
            });
            return OutputList;
        }
    }
}
