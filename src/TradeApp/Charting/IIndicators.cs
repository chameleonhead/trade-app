namespace TradeApp.Charting
{
    public interface IIndicator<in TIn, out TOut>
    {
        TOut Next(TIn data);
    }

    public interface IChartIndicator<T> : IIndicator<Candle, T>
    {
    }
}
