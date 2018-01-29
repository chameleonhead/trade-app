using System.Collections.Generic;

namespace TradeApp.Charting.Indicators
{
    public class SmaIndicator : IIndicator<SingleValue, SingleValue>
    {
        private Queue<decimal> samples;
        private decimal total;
        private int period;

        public SmaIndicator(int period)
        {
            this.samples = new Queue<decimal>();
            this.period = period;
        }

        public SingleValue Next(SingleValue data)
        {
            total += data.Value;
            samples.Enqueue(data.Value);

            if (samples.Count <= period)
            {
                return new SingleValue(data.Time, total / samples.Count);
            }

            total -= samples.Dequeue();
            return new SingleValue(data.Time, total / period);
        }
    }
}
