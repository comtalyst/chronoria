using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Xunit;
using Chronoria_PersistentWorkers.Schedulers;
using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Tests.Models.Mocks.DbContexts;

namespace Chronoria_PersistentWorkers.Tests.Schedulers
{
    public class ExpireClearSchedulerTest : IDisposable
    {
        private class MockExpireClearProducer : IExpireClearProducer
        {
            public List<ExpireClearMessage> expireClearMessages;
            
            public MockExpireClearProducer()
            {
                expireClearMessages = new List<ExpireClearMessage>();
            }
            public async Task Produce(ExpireClearMessage message)
            {
                expireClearMessages.Add(message);
                await Task.CompletedTask;
            }

            public Task Produce(IEnumerable<ExpireClearMessage> messages)
            {
                throw new NotImplementedException();
            }
        }
        private const long fetchTime = 600;
        private const long epsilon = 50;
        private readonly IScheduler scheduler;
        private readonly MockExpireClearProducer producer;
        private readonly PendingContext contextMock;
        private readonly ICapsuleRepository<PendingContext> capsuleRepository;         // assume this passed the test and correct

        private static bool approxEqual(long t1, long t2)
        {
            var diff = t1 - t2;
            if(diff < 0)
            {
                diff = -diff;
            }
            return diff < epsilon;
        }

        public ExpireClearSchedulerTest()
        {
            producer = new MockExpireClearProducer();
            var contextMocker = new InmemMockContext<PendingContext>(op => new PendingContext(op));
            contextMock = contextMocker.context;
            capsuleRepository = new CapsuleRepository<PendingContext>(contextMock);
            scheduler = new ExpireClearScheduler(fetchTime, producer, capsuleRepository);
        }
        public void Dispose()
        {
            contextMock.Database.EnsureDeleted();
        }
    }
}
