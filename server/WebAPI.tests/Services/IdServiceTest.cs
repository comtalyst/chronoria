using Xunit;
using Chronoria_WebAPI.Services;

namespace Tests.Services
{
    public class IdServiceTest
    {
        private readonly IIdService idService;
        public IdServiceTest()
        {
            idService = new IdService();
        }

        [Fact]
        public void Generate_And_Validate_Correctly()
        {
            var id = idService.generate();
            Assert.True(idService.validate(id));
        }
        [Fact]
        public void Validate_Correctly()
        {
            var id = "0241cccc-d492-4eae-83e8-83914afe8c56";
            Assert.True(idService.validate(id));
        }
        [Fact]
        public void Invalidate_TooShort()
        {
            var id = "0241ccc-d492-4eae-83e8-83914afe8c56";
            Assert.False(idService.validate(id));
        }
        [Fact]
        public void Invalidate_Illegal1()
        {
            var id = "0241c-cc-d492-4eae-83e8-83914afe8c56";
            Assert.False(idService.validate(id));
        }
        [Fact]
        public void Invalidate_Illegal2()
        {
            var id = "0241cccc-d492-4eae-83e8-839*4afe8c56";
            Assert.False(idService.validate(id));
        }
    }
}
