using System.Collections.Generic;

namespace TradeApp.Charting.Indicators
{
    public class SmaIndicator : IChartIndicator<SingleValue>
    {
        public class SmaImpl : IIndicator<decimal, decimal>
        {
            private Queue<decimal> samples;
            private decimal total;
            private int period;

            public SmaImpl(int period)
            {
                this.samples = new Queue<decimal>();
                this.period = period;
            }

            public decimal Next(decimal data)
            {
                total += data;
                samples.Enqueue(data);

                if (samples.Count <= period)
                {
                    return total / samples.Count;
                }

                total -= samples.Dequeue();
                return total / period;
            }
        }

        private SmaImpl _decimalIndicator;

        public SmaIndicator(int period)
        {
            _decimalIndicator = new SmaImpl(period);
        }

        public SingleValue Next(Candle data)
        {
            return new SingleValue(data.Time, _decimalIndicator.Next(data.Close));
        }
    }
}
