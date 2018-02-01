using System.Linq;

namespace TradeApp.Charting
{
    public interface ICandleStore
    {
        void AddCandle(Candle candle);
        Candle LatestCandle { get; }
        IQueryable<Candle> Candles { get; }
    }
}