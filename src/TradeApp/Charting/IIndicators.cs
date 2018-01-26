namespace TradeApp.Charting
{
    public interface IIndicator<in TIn, out TOut>
    {
        TOut NextValue(TIn data);
    }
}
