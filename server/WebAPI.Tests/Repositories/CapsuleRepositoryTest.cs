using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Tests.Models.Mocks.DbContexts;

namespace Chronoria_WebAPI.Tests.Repositories
{
    public class CapsuleRepositoryTest
    {
        private readonly ICapsuleRepository<BaseContext> capsuleRepository;
        private readonly BaseContext contextMock;

        public CapsuleRepositoryTest()
        {
            var contextMocker = new InmemMockContext<BaseContext>(op => new BaseContext(op));
            contextMock = contextMocker.context;
            capsuleRepository = new CapsuleRepository<BaseContext>(contextMock);
        }

        [Fact]
        public async void Create_Successfully()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                DateTime.Now,
                DateTime.Now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );
            Capsule cap2 = await capsuleRepository.Create(capsule);
            Assert.Equal(capsule, cap2);
            Assert.True(contextMock.Capsules.Any());
            Capsule found = await contextMock.Capsules.FindAsync("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.NotNull(found);
        }
    }
}
