using AutoBogus;
using AutoBogus.Conventions;

namespace CoffeeCard.Tests.Common.Builders
{
    public abstract class BaseBuilder<T> where T : class
    {
        protected readonly AutoFaker<T> Faker = new();
        protected BaseBuilder()
        {
            Faker.Configure(builder => builder.WithConventions());
        }

        /// <summary>
        /// Gives a standard configured builder,
        /// where lists of linked entities are empty
        /// and nullable enities are null
        /// </summary>
        public abstract BaseBuilder<T> Simple();

        /// <summary>
        /// Gives a standard configured builder,
        /// where all linked entities are populated
        /// </summary>
        public abstract BaseBuilder<T> Typical();

        /// <summary>
        /// Creates a new instance of type T, 
        /// using the configuration set by 
        /// </summary>
        public T Build(){
            return Faker.Generate();
        }
    }
}