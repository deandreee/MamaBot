using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.DataProvider
{
    class BookEntry
    {
            public BookEntry(decimal ask, decimal bid, long updateTime)
            {
                if (updateTime <= 0)
                    throw new ArgumentOutOfRangeException(nameof(updateTime));

                Ask = ask;
                Bid = bid;
                UpdateTime = updateTime;
            }


            public decimal Ask { get; }

            public decimal Bid { get; }

            public long UpdateTime { get; }


            public bool IsFullPair => Ask != null && Bid != null;

            public decimal? PriceSpread => IsFullPair ? Ask - Bid : default(decimal?);
            public decimal? NegativeSpread => IsFullPair ? Bid - Ask : default(decimal?);

            public decimal? MediumPrice => IsFullPair ? (Ask + Bid) / 2 : default(decimal?);

    }
}
