namespace TradeApp.Charting
{
    public interface IIndicator { }

    public interface IIndicator<in TIn, out TOut> : IIndicator
    {
        TOut Next(TIn data);
    }
}
