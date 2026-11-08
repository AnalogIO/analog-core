using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Tests.ApiClient.v2.Generated;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers.v2
{
    public class GetProgrammesTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task GetProgrammes_returns_active_programmes_sorted_by_sort_priority()
        {
            RemoveRequestHeaders();

            var laterProgramme = ProgrammeBuilder
                .Simple()
                .WithId(1)
                .WithSortPriority(20)
                .WithShortName("LATER")
                .WithFullName("Later programme")
                .WithIsActive(true)
                .Build();
            var earlierProgramme = ProgrammeBuilder
                .Simple()
                .WithId(2)
                .WithSortPriority(10)
                .WithShortName("EARLY")
                .WithFullName("Earlier programme")
                .WithIsActive(true)
                .Build();
            var inactiveProgramme = ProgrammeBuilder
                .Simple()
                .WithId(3)
                .WithSortPriority(5)
                .WithShortName("INACTIVE")
                .WithFullName("Inactive programme")
                .WithIsActive(false)
                .Build();

            Context.Programmes.AddRange(laterProgramme, earlierProgramme, inactiveProgramme);
            await Context.SaveChangesAsync();

            var programmes = await CoffeeCardClientV2.Programmes_GetAsync();

            Assert.Equal(
                new List<int> { earlierProgramme.Id, laterProgramme.Id },
                programmes.Select(programme => programme.Id).ToList()
            );
            Assert.Equal(
                "EARLY",
                programmes.First(programme => programme.Id == earlierProgramme.Id).ShortName
            );
            Assert.Equal(
                "Earlier programme",
                programmes.First(programme => programme.Id == earlierProgramme.Id).FullName
            );
        }
    }
}
