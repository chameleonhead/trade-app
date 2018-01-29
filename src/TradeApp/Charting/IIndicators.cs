namespace TradeApp.Charting
{
    public interface IIndicator<in TIn, out TOut>
    {
        TOut Last { get; }
        TOut Next(TIn data);
    }
}
