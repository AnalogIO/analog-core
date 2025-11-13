using Bogus;

namespace CoffeeCard.Tests.Common.Builders
{
    public abstract class BaseBuilder<T>
        where T : class
    {
        protected readonly Faker<T> Faker = new();

        /// <summary>
        /// Creates a new instance of type T,
        /// using the configuration set by using the builderMethods
        /// </summary>
        public T Build()
        {
            return Faker.StrictMode(true).Generate();
        }

        /// <summary>
        /// Creates a new list of type T,
        /// using the configuration set by using the builderMethods
        /// </summary>
        public List<T> Build(int count)
        {
            return Faker.StrictMode(true).Generate(count);
        }
    }

    public interface IBuilder<T>
        where T : class
    {
        /// <summary>
        /// Gives a standard configured builder,
        /// where lists of linked entities are empty
        /// and nullable enities are null
        /// </summary>
        public static abstract T Simple();

        /// <summary>
        /// Gives a standard configured builder,
        /// where all linked entities are populated
        /// </summary>
        public static abstract T Typical();
    }
}
