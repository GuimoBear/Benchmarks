using BenchmarkDotNet.Attributes;
using Benchmarks.Linqs.Entities;
using Bogus;
using System.Runtime.InteropServices;

namespace Benchmarks.Linqs
{
    [MemoryDiagnoser(true)]
    public class ListValidationBenchmarks
    {
        private static readonly Faker<CardProductModel> Faker;

        static ListValidationBenchmarks()
        {
            Randomizer.Seed = new Random(643238);
            Faker = new Faker<CardProductModel>()
                .RuleFor(c => c.Brand, f => f.PickRandom<CardBrandEnum>())
                .RuleFor(c => c.Product, f => f.PickRandom<CardProductEnum>())
                .RuleFor(c => c.Installment, f => f.Random.Int(1, 18))
                .RuleFor(c => c.UnitPrice, f => f.Random.Double(0.2) * 5)
                .RuleFor(c => c.PercentagePrice, f => f.Random.Double(0.02, 0.05) * 100.0)
                .RuleFor(c => c.PrincingType, f => f .PickRandom<CardPricingTypeEnum>());
        }
        private int numberOfCards;

        public List<CardProductModel> Cards { get; private set; }

        [Params(5, 12, 18)]
        public int NumberOfCards
        {
            get => this.numberOfCards;
            set
            {
                this.numberOfCards = value;
                this.Cards = Faker.GenerateLazy(value).ToList();
            }
        }

        [Benchmark(Description = "Current", Baseline = true)]
        public bool CurrentValidation()
        {
            bool returnValue = true;
            var cards = this.Cards;

            cards.ForEach(item =>
            {
                var brand = item.Brand;
                var product = item.Product;
                var installment = item.Installment;
                var cardsDuplicated = cards.Where(card => !ReferenceEquals(card, item) && card.Brand == brand && card.Product == product && card.Installment == installment).ToList();
                if (cardsDuplicated.Count > 0)
                {
                    returnValue = false;
                    return;
                }
            });
            return returnValue;
        }

        [Benchmark(Description = "New")]
        public bool NewValidation()
        {
            var cards = this.Cards;

            return !cards
                .Exists(item => cards.Exists(card => !ReferenceEquals(card, item) && card.Brand == item.Brand && card.Product == item.Product && card.Installment == item.Installment));
        }

        [Benchmark(Description = "Using HashSet")]
        public bool HashSetValidation()
        {
            var cards = this.Cards;
            var hashSet = new HashSet<(CardBrandEnum, CardProductEnum, int)>(cards.Count);
            for (int currentCardIndex = 0; currentCardIndex < cards.Count; currentCardIndex++)
            {
                var current = cards[currentCardIndex];
                if (!hashSet.Add((current.Brand, current.Product, current.Installment)))
                    return false;
            }
            return true;
        }

        [Benchmark(Description = "Best")]
        public bool BestValidation()
        {
            var cards = this.Cards;
            var last = cards.Count - 1;
            for (int currentCardIndex = 0; currentCardIndex < last; currentCardIndex++)
            {
                var current = cards[currentCardIndex];
                for (int nextCardIndex = currentCardIndex + 1; nextCardIndex < cards.Count; nextCardIndex++)
                {
                    var next = cards[nextCardIndex];
                    if (current.Brand == next.Brand && current.Product == next.Product && current.Installment == next.Installment)
                        return false;
                }
            }
            return true;
        }

        [Benchmark(Description = "Bestest")]
        public bool BestestValidation()
        {
            var cards = CollectionsMarshal.AsSpan(this.Cards);
            var last = cards.Length - 1;
            for (int currentCardIndex = 0; currentCardIndex < last; currentCardIndex++)
            {
                var current = cards[currentCardIndex];
                for (int nextCardIndex = currentCardIndex + 1; nextCardIndex < cards.Length; nextCardIndex++)
                {
                    var next = cards[nextCardIndex];
                    if (current.Brand == next.Brand && current.Product == next.Product && current.Installment == next.Installment)
                        return false;
                }
            }
            return true;
        }
    }
}
