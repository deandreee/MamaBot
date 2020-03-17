using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace MaMa.HFT.Console.GlobalShared
{
    public class Instance
    {
        public BinanceClient client { get; set; }
        public BinanceSocketClient socketClient = new BinanceSocketClient();
        private CancellationToken token;
        decimal CurrentCumulativeDelta = 0;

        protected readonly Logger Logger;
        public bool IsAllowedIntoRange = false;
        public string PairLink { get; set; }
        public string ListenerKey { get; set; }
        public Instance(string pair,string api,string apisec)
        {
            Logger = LogManager.GetCurrentClassLogger();

            PairLink = pair;


            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(api, apisec),
                LogVerbosity = LogVerbosity.Debug
            });

            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials(api, apisec),
                LogVerbosity = LogVerbosity.Debug
            });
            client = new BinanceClient();
            var accountInfo = client.GetAccountInfo();
            ListenerKey = client.StartUserStream().Data;
            OrderDataStream();
        }

        public void OrderDataStream()
        {
            socketClient.SubscribeToUserDataUpdates(ListenerKey, null, OrderStreamUpdate, OrderStream, BalanceStream, null);

        }

        public void StartMaker()
        {


        }

        public void ReceiveOrderBook()
        {

        }


        public void PlaceOrder(string PairLink, OrderSide side , OrderType type, decimal quantity, decimal price)
        {

            var orderResult = client.PlaceOrder(PairLink, side, type, quantity, null, null, price, TimeInForce.GoodTillCancel, null, null, null, null, token);
            if(orderResult.Error != null)
            {
                Logger.Error(string.Format("Order not executed due to : {0}", orderResult.Error.Message));

            }
            else
            {
                Logger.Info(string.Format("Order not executed due to : {0}", orderResult.Data.Status));

            }


        }

        public void ObSub()
        {
            socketClient.SubscribeToBookTickerUpdates(PairLink, HandleBookOffer);
            socketClient.SubscribeToKlineUpdates(PairLink, KlineInterval.OneMinute, KL1Min);
            socketClient.SubscribeToSymbolTickerUpdates(PairLink, TT5);
        }

        private void TT5(BinanceStreamTick obj)
        {
            Logger.Info(string.Format("CVD : {0}", CurrentCumulativeDelta));

            Logger.Info(string.Format("WAP : {0}", obj.WeightedAveragePrice));

            CurrentCumulativeDelta -= obj.AskQuantity;

            CurrentCumulativeDelta += obj.BidQuantity;

        }

    private void KL1Min(BinanceStreamKlineData obj)
        {
            //this.IsAllowedIntoRange = obj.Data.Open > obj.Data.Close;
            //CurrentCumulativeDelta = (obj.Data.Volume - obj.Data.TakerBuyQuoteAssetVolume);
            //Logger.Info(string.Format("CVD : {0}", CurrentCumulativeDelta));
            //Logger.Info(string.Format("VOL : {0}", obj.Data.Volume));
            if (obj.Data.Final) { CurrentCumulativeDelta = 0; }
        }

        private void HandleBookOffer(BinanceStreamBookPrice obj)
        {
             decimal PriceSpread = obj.BestAskPrice - obj.BestBidPrice;

             decimal? VolumeSpread =  Math.Abs(obj.BestAskQuantity - obj.BestBidQuantity);

             decimal? MediumPrice = (obj.BestAskPrice + obj.BestBidPrice) / 2;
            //Logger.Info(string.Format("Price Spread : {0} ", PriceSpread));
            //Logger.Info(string.Format("Volume Spread : {0} ",VolumeSpread));
            //Logger.Info(string.Format("Medium Price : {0} ", MediumPrice));

            //if (PriceSpread > 3 && this.IsAllowedIntoRange)
            //{
            //    this.RemoveAllDirectionOrder(Binance.Net.Objects.OrderSide.Buy);
            //    this.RemoveAllDirectionOrder(Binance.Net.Objects.OrderSide.Sell);
            //    //Quote quote = new Quote(decimal.Round(obj.BestAskPrice, 2), decimal.Round(.0022m, 4), OrderSide.Buy, decimal.Round(obj.BestBidPrice, 2));

            //    this.PlaceOrder("BTCUSDT", Binance.Net.Objects.OrderSide.Sell, Binance.Net.Objects.OrderType.Limit, decimal.Round(.0022m, 4), decimal.Round(MediumPrice.Value, 2));
            //    this.PlaceOrder("BTCUSDT", Binance.Net.Objects.OrderSide.Buy, Binance.Net.Objects.OrderType.Limit, decimal.Round(.0022m, 4), decimal.Round(obj.BestBidPrice, 2));

            //}

            //Logger.Info(obj.BestAskPrice);
            //Logger.Info(obj.BestBidPrice);
            //Logger.Info(obj.BestAskQuantity);
            //Logger.Info(obj.BestBidQuantity);

        }

        public void RemoveAllDirectionOrder(OrderSide direction)
        {

            var CurrentOrder = client.GetOpenOrders(PairLink);
            foreach(var order in CurrentOrder.Data)
            {
                if(order.Side == direction && order.Symbol == PairLink)
                {
                    client.CancelOrder(PairLink,order.OrderId);
                }
            }


        }
        private void BalanceStream(IEnumerable<BinanceStreamBalance> obj)
        {
            foreach(var value in obj)
            {

                //Logger.Info(string.Format("Account update received for : {0}",value.Asset));
                //Logger.Info(string.Format("Account update received for : {0}", value.Total));
                //Logger.Info(string.Format("Account update received for : {0}", value.Free));

            }
        }

        private void OrderStream(BinanceStreamOrderList obj)
        {


                //Logger.Info(string.Format("TransactionTime received for : {0}", obj.TransactionTime));
                //Logger.Info(string.Format("ListOrderStatus received for : {0}", obj.ListOrderStatus));


        }

        private void OrderStreamUpdate(BinanceStreamOrderUpdate obj)
        {
                //Logger.Info(string.Format("TransactionTime received for : {0}", obj.Status));
                //Logger.Info(string.Format("ListOrderStatus received for : {0}", obj.Quantity));

        }
    }
}
