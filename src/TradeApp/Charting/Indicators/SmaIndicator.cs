using System.Collections.Generic;

namespace TradeApp.Charting.Indicators
{
    public class SmaIndicator : IIndicator<decimal, decimal>
    {
        private Queue<decimal> samples;
        private decimal total;
        private int period;

        public SmaIndicator(int period)
        {
            this.samples = new Queue<decimal>();
            this.period = period;
        }

        public decimal NextValue(decimal data)
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
}
