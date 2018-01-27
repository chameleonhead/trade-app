using System;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    public class AtrIndicator : IIndicator<Candle, decimal?>
    {
        private Candle previous;
        private decimal? averageTrueRange;
        private int period;
        private SmaIndicator smaIndicator;
        private int currentIndex;

        public AtrIndicator(int period)
        {
            this.period = period;
            this.smaIndicator = new SmaIndicator(period);
        }

        public decimal? NextValue(Candle data)
        {
            currentIndex++;
            var trueRange = previous == null
                ? data.High - data.Low
                : new[]{
                    data.High - data.Low,
                    Math.Abs(data.High - previous.Close),
                    Math.Abs(data.Low - previous.Close)
                }.Max();
            previous = data;

            if (currentIndex <= period)
            {
                averageTrueRange = smaIndicator.NextValue(trueRange);
                return null;
            }

            averageTrueRange = (averageTrueRange * (period - 1) + trueRange) / period;
            return averageTrueRange;
        }
    }
}