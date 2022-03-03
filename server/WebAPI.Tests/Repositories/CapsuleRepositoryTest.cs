using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Xunit;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Tests.Models.Mocks.DbContexts;

namespace Chronoria_WebAPI.Tests.Repositories
{
    public class CapsuleRepositoryTest : IDisposable
    {
        private readonly ICapsuleRepository<BaseContext> capsuleRepository;
        private readonly BaseContext contextMock;
        private readonly DateTime now;

        // this runs before every test
        public CapsuleRepositoryTest()
        {
            now = DateTime.Now;

            var contextMocker = new InmemMockContext<BaseContext>(op => new BaseContext(op));
            contextMock = contextMocker.context;
            capsuleRepository = new CapsuleRepository<BaseContext>(contextMock);
        }
        // this runs after every test
        public void Dispose()
        {
            contextMock.Database.EnsureDeleted();
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
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            Capsule ret = await capsuleRepository.Create(capsule);
            Assert.Equal(capsule, ret);

            Assert.True(contextMock.Capsules.Any());

            Capsule? found = await contextMock.Capsules.FindAsync("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.NotNull(found);
            Assert.Equal(capsule, found);
        }
        [Fact]
        public async void Create_Only_One()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.Single(contextMock.Capsules.ToList());
        }
        [Fact]
        public async void Create_And_Delete_Correctly()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.True(contextMock.Capsules.Any());
            await capsuleRepository.Delete("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.False(contextMock.Capsules.Any());
        }
        [Fact]
        public async void Not_Delete_Nonexistent_Entry()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.True(contextMock.Capsules.Any());
            await capsuleRepository.Delete("0cade597-d092-40db-9a8d-589166f76b28");
            Assert.True(contextMock.Capsules.Any());
        }
        public async void Create_And_Retrieve_Correctly()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.True(contextMock.Capsules.Any());
            Capsule? ret = await capsuleRepository.Retrieve("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.NotNull(ret);
            Assert.Equal(capsule, ret);
            Assert.False(contextMock.Capsules.Any());
        }
        [Fact]
        public async void Not_Retrieve_Nonexistent_Entry()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.True(contextMock.Capsules.Any());
            Capsule? ret = await capsuleRepository.Retrieve("0cade597-d092-40db-9a8d-589166f76b28");
            Assert.Null(ret);
            Assert.True(contextMock.Capsules.Any());
        }
        [Fact]
        public async void Not_Retrieve_Just_Retrieved_Entry()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(capsule);
            Assert.True(contextMock.Capsules.Any());
            Capsule? ret = await capsuleRepository.Retrieve("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.NotNull(ret);
            Capsule? ret2 = await capsuleRepository.Retrieve("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.Null(ret2);
        }
        [Fact]
        public async void Get_Correctly()
        {
            Capsule capsule = new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b29",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                );

            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b28",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(capsule);
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b30",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));

            Capsule? ret = await capsuleRepository.Get("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.NotNull(ret);
            Assert.Equal(capsule, ret);
        }
        [Fact]
        public async void Get_Null_If_Not_Exist()
        {
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b28",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b30",
                "sender@email.com",
                "Mr. Sender",
                "recipient@email.com",
                "Mr. Recipient",
                (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now,
                now,
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));

            Capsule? ret = await capsuleRepository.Get("0cade597-d092-40db-9a8d-589166f76b29");
            Assert.Null(ret);
        }
    }
}
