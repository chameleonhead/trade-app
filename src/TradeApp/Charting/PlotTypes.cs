using System;

namespace TradeApp.Charting
{
    public class Candle
    {
        public Candle(DateTime time, decimal open, decimal high, decimal low, decimal close, int volume)
        {
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public DateTime Time { get; }
        public decimal Open { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Close { get; }
        public int Volume { get; }
    }

    public class SingleValue
    {
        public SingleValue(DateTime time, decimal value)
        {
            Time = time;
            Value = value;
        }

        public DateTime Time { get; }
        public decimal Value { get; }
    }
}
