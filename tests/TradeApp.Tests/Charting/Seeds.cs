using System;

namespace TradeApp.Charting
{
    static class Seeds
    {
        private static DateTime date = DateTime.Now;
        public static Tuple<Candle[], SingleValue[]> ATR14_CANDLES = Tuple.Create(
            new[]
            {
                new Candle(date.AddDays(0),  0, 48.70m, 47.79m, 48.16m, 0),
                new Candle(date.AddDays(1),  0, 48.72m, 48.14m, 48.61m, 0),
                new Candle(date.AddDays(2),  0, 48.90m, 48.39m, 48.75m, 0),
                new Candle(date.AddDays(3),  0, 48.87m, 48.37m, 48.63m, 0),
                new Candle(date.AddDays(4),  0, 48.82m, 48.24m, 48.74m, 0),
                new Candle(date.AddDays(5),  0, 49.05m, 48.64m, 49.03m, 0),
                new Candle(date.AddDays(6),  0, 49.20m, 48.94m, 49.07m, 0),
                new Candle(date.AddDays(7),  0, 49.35m, 48.86m, 49.32m, 0),
                new Candle(date.AddDays(8),  0, 49.92m, 49.50m, 49.91m, 0),
                new Candle(date.AddDays(9),  0, 50.19m, 49.87m, 50.13m, 0),
                new Candle(date.AddDays(10),  0, 50.12m, 49.20m, 49.53m, 0),
                new Candle(date.AddDays(11),  0, 49.66m, 48.90m, 49.50m, 0),
                new Candle(date.AddDays(12),  0, 49.88m, 49.43m, 49.75m, 0),
                new Candle(date.AddDays(13),  0, 50.19m, 49.73m, 50.03m, 0),
                new Candle(date.AddDays(14),  0, 50.36m, 49.26m, 50.31m, 0),
                new Candle(date.AddDays(15),  0, 50.57m, 50.09m, 50.52m, 0),
                new Candle(date.AddDays(16),  0, 50.65m, 50.30m, 50.41m, 0),
                new Candle(date.AddDays(17),  0, 50.43m, 49.21m, 49.34m, 0),
                new Candle(date.AddDays(18),  0, 49.63m, 48.98m, 49.37m, 0),
                new Candle(date.AddDays(19),  0, 50.33m, 49.61m, 50.23m, 0),
                new Candle(date.AddDays(20),  0, 50.29m, 49.20m, 49.24m, 0),
                new Candle(date.AddDays(21),  0, 50.17m, 49.43m, 49.93m, 0),
                new Candle(date.AddDays(22),  0, 49.32m, 48.08m, 48.43m, 0),
                new Candle(date.AddDays(23),  0, 48.50m, 47.64m, 48.18m, 0),
                new Candle(date.AddDays(24),  0, 48.32m, 41.55m, 46.57m, 0),
                new Candle(date.AddDays(25),  0, 46.80m, 44.28m, 45.41m, 0),
                new Candle(date.AddDays(26),  0, 47.80m, 47.31m, 47.77m, 0),
                new Candle(date.AddDays(27),  0, 48.39m, 47.20m, 47.72m, 0),
                new Candle(date.AddDays(28),  0, 48.66m, 47.90m, 48.62m, 0),
                new Candle(date.AddDays(29),  0, 48.79m, 47.73m, 47.85m, 0),
            },
            new[] {
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                default(SingleValue),
                new SingleValue(date.AddDays(14), 0.5933m),
                new SingleValue(date.AddDays(15), 0.5852m),
                new SingleValue(date.AddDays(16), 0.5684m),
                new SingleValue(date.AddDays(17), 0.6149m),
                new SingleValue(date.AddDays(18), 0.6174m),
                new SingleValue(date.AddDays(19), 0.6419m),
                new SingleValue(date.AddDays(20), 0.6739m),
                new SingleValue(date.AddDays(21), 0.6922m),
                new SingleValue(date.AddDays(22), 0.7749m),
                new SingleValue(date.AddDays(23), 0.7810m),
                new SingleValue(date.AddDays(24), 1.2088m),
                new SingleValue(date.AddDays(25), 1.3024m),
                new SingleValue(date.AddDays(26), 1.3801m),
                new SingleValue(date.AddDays(27), 1.3665m),
                new SingleValue(date.AddDays(28), 1.3361m),
                new SingleValue(date.AddDays(29), 1.3163m),
            }
        );
    }
}
