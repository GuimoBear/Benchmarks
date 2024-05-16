using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks.Linqs.Entities
{
    public class CardProductModel
    {
        public CardBrandEnum Brand { get; set; }
        public CardProductEnum Product { get; set; }
        public int Installment { get; set; }
        public double UnitPrice { get; set; }
        public double PercentagePrice { get; set; }
        public CardPricingTypeEnum PrincingType { get; set; }
    }

    public enum CardBrandEnum : byte
    {
        None = 0,
        Mastercard = 1,
        Visa = 2,
        Hipercard = 3,
        Amex = 4,
        Elo = 5,
        Unionpay = 6,
        Cabal = 7
    }

    public enum CardProductEnum : byte
    {
        Debit = 0,
        Credit = 1
    }

    public enum CardPricingTypeEnum : byte
    {
        Cet = 0,
        Mdr = 1
    }
}
