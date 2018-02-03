namespace TradeApp.Charting
{
    public interface IIndicator<in TIn, out TOut>
    {
        TOut Next(TIn data);
    }

    public interface IChartIndicator { }

    public interface IChartIndicator<out T> : IChartIndicator, IIndicator<Candle, T>
        where T: IPlotType
    {
    }
}
