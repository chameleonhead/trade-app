using System.Collections.Generic;

namespace TradeApp
{
    public class TradingSymbol
    {
        public TradingSymbol(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }

        public override bool Equals(object obj)
        {
            var symbol = obj as TradingSymbol;
            return symbol != null &&
                   Symbol == symbol.Symbol;
        }

        public override int GetHashCode()
        {
            return -1758840423 + EqualityComparer<string>.Default.GetHashCode(Symbol);
        }
    }
}
