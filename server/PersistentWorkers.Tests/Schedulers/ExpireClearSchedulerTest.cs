using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Xunit;
using Xunit.Abstractions;
using Chronoria_PersistentWorkers.Schedulers;
using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Tests.Models.Mocks.DbContexts;
using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Tests.Schedulers
{
    public class ExpireClearSchedulerTest : IDisposable
    {
        private class MockExpireClearProducer : IExpireClearProducer
        {
            public class ExpireClearLog
            {
                public long TimeL { get; set; }
                public long TimeR { get; set; }
                public long TimeLog { get; set; }
                public ExpireClearLog(ExpireClearMessage message)
                {
                    TimeLog = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                    TimeL = message.TimeL;
                    TimeR = message.TimeR;
                }
            }
            public List<ExpireClearLog> expireClearLogs;
            
            public MockExpireClearProducer()
            {
                expireClearLogs = new List<ExpireClearLog>();
            }
            public async Task Produce(ExpireClearMessage message)
            {
                expireClearLogs.Add(new ExpireClearLog(message));
                await Task.CompletedTask;
            }

            public Task Produce(IEnumerable<ExpireClearMessage> messages)
            {
                throw new NotImplementedException();
            }
        }
        private readonly ITestOutputHelper output;
        private const long fetchTime = 50000;
        private const long epsilon = 2000;
        private readonly IScheduler scheduler;
        private readonly MockExpireClearProducer producer;
        private readonly PendingContext contextMock;
        private readonly ICapsuleRepository<PendingContext> capsuleRepository;         // assume this passed the test and correct
        private readonly List<MockExpireClearProducer.ExpireClearLog> produced;

        private static bool ApproxEqual(long t1, long t2)
        {
            var diff = t1 - t2;
            if(diff < 0)
            {
                diff = -diff;
            }
            return diff < epsilon;
        }

        public ExpireClearSchedulerTest(ITestOutputHelper output)
        {
            this.output = output;
            producer = new MockExpireClearProducer();
            produced = producer.expireClearLogs;
            var contextMocker = new InmemMockContext<PendingContext>(op => new PendingContext(op));
            contextMock = contextMocker.context;
            capsuleRepository = new CapsuleRepository<PendingContext>(contextMock);
            scheduler = new ExpireClearScheduler(fetchTime, producer, capsuleRepository);
        }
        public void Dispose()
        {
            contextMock.Database.EnsureDeleted();
        }
        [Fact]
        public async void Suspend_Efficiently()
        {
            var scheduler2 = new ExpireClearScheduler(1000000, producer, capsuleRepository);
            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler2.Start();
            await Task.Delay(1000);
            await scheduler2.Suspend();
            var stopTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            Assert.NotEmpty(produced);
            Assert.True(ApproxEqual(stopTime, startTime + 1000));
        }

        [Fact]
        public async void Trigger_First_Time_Correctly()
        {
            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler.Start();
            await Task.Delay(1000);
            await scheduler.Suspend();
            Assert.NotEmpty(produced);
            Assert.True(ApproxEqual(produced[0].TimeR, startTime - fetchTime));
            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
        }
        [Fact]
        public async void Trigger_Correctly_On_Empty_Db()
        {
            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler.Start();
            await Task.Delay((int)(fetchTime + epsilon) * 4);
            await scheduler.Suspend();
            Assert.NotEmpty(produced);

            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
            for (int i = 1; i < produced.Count; i++)
            {
                //output.WriteLine(produced[i].TimeL.ToString() + " -- " + produced[i].TimeR.ToString());
                Assert.Equal(produced[i].TimeL-1, produced[i - 1].TimeR);
                Assert.True(ApproxEqual(produced[i].TimeR - produced[i].TimeL, fetchTime));
                Assert.True(ApproxEqual(produced[i].TimeLog, startTime + fetchTime*i));
            }
        }
        [Fact]
        public async void Trigger_Correctly_On_Prefilled_Db()
        {
            var now = TimeUtils.now();
            output.WriteLine(TimeUtils.DateTimeToEpochMs(now).ToString());
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b00", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b01", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 7 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 7 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b02", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 11 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 11 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b03", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 14 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 14 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            Assert.Equal("0cade597-d092-40db-9a8d-589166f76b00", (await capsuleRepository.GetNextByCreateTime(now.AddMilliseconds(-fetchTime))).Id);

            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler.Start();
            await Task.Delay((int)(fetchTime + epsilon) * 4);
            await scheduler.Suspend();
            
            for (int i = 0; i < produced.Count; i++)
            {
                output.WriteLine(produced[i].TimeLog.ToString() + " : " + produced[i].TimeR.ToString() + " - " + produced[i].TimeL.ToString());
            }

            Assert.NotEmpty(produced);
            Assert.True(produced.Count > 5);

            List<long> waits = new List<long>();
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime * 3 / 6);
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime * 3 / 6);
            waits.Add(fetchTime);

            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
            long sumWaits = 0;
            for (int i = 1; i <= 5; i++)
            {
                // connected to last message
                Assert.Equal(produced[i].TimeL - 1, produced[i - 1].TimeR);

                sumWaits += waits[i - 1];
                // produce a message at the correct time
                Assert.True(ApproxEqual(produced[i].TimeLog - produced[i - 1].TimeLog, waits[i - 1]));          // exact time may diverge by epsilon acculmulation
                // message is correct
                Assert.True(ApproxEqual(produced[i].TimeR, produced[i].TimeLog - fetchTime));
                Assert.True(ApproxEqual(produced[i].TimeR - produced[i].TimeL, waits[i - 1]));
            }
        }
        [Fact]
        public async void Trigger_Correctly_On_Prefilled_Db_Dispersed()
        {
            var now = TimeUtils.now();
            output.WriteLine(TimeUtils.DateTimeToEpochMs(now).ToString());
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b00", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b01", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 7 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 7 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b02", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 22 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 22 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            Assert.Equal("0cade597-d092-40db-9a8d-589166f76b00", (await capsuleRepository.GetNextByCreateTime(now.AddMilliseconds(-fetchTime))).Id);

            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler.Start();
            await Task.Delay((int)(fetchTime + epsilon) * 5);
            await scheduler.Suspend();

            for (int i = 0; i < produced.Count; i++)
            {
                output.WriteLine(produced[i].TimeLog.ToString() + " : " + produced[i].TimeR.ToString() + " - " + produced[i].TimeL.ToString());
            }

            Assert.NotEmpty(produced);
            Assert.True(produced.Count > 6);

            List<long> waits = new List<long>();
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime * 3 / 6);
            waits.Add(fetchTime);
            waits.Add(fetchTime);
            waits.Add(fetchTime * 3 / 6);
            waits.Add(fetchTime);

            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
            long sumWaits = 0;
            for (int i = 1; i <= 5; i++)
            {
                // connected to last message
                Assert.Equal(produced[i].TimeL - 1, produced[i - 1].TimeR);

                sumWaits += waits[i - 1];
                // produce a message at the correct time
                Assert.True(ApproxEqual(produced[i].TimeLog - produced[i - 1].TimeLog, waits[i - 1]));          // exact time may diverge by epsilon acculmulation
                // message is correct
                Assert.True(ApproxEqual(produced[i].TimeR, produced[i].TimeLog - fetchTime));
                Assert.True(ApproxEqual(produced[i].TimeR - produced[i].TimeL, waits[i - 1]));
            }
        }
        [Fact]
        public async void Trigger_Correctly_On_Prefilled_Db_Stacked()
        {
            var now = TimeUtils.now();
            output.WriteLine(TimeUtils.DateTimeToEpochMs(now).ToString());
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b00", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 1 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 1 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b01", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 2 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 2 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b02", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 3 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 3 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b03", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 4 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b04", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 5 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 5 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b05", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 6 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 6 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b06", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 10 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 10 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b07", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 22 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 22 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b08", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 23 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 23 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b09", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 24 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 24 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b10", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                now.AddMilliseconds(fetchTime * 31 / 6 - fetchTime),
                now.AddMilliseconds(fetchTime * 31 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            Assert.Equal("0cade597-d092-40db-9a8d-589166f76b00", (await capsuleRepository.GetNextByCreateTime(now.AddMilliseconds(-fetchTime))).Id);

            var startTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
            await scheduler.Start();
            await Task.Delay((int)(fetchTime + epsilon) * 37 / 6);
            await scheduler.Suspend();

            for (int i = 0; i < produced.Count; i++)
            {
                output.WriteLine(produced[i].TimeLog.ToString() + " : " + produced[i].TimeR.ToString() + " - " + produced[i].TimeL.ToString());
            }

            Assert.NotEmpty(produced);
            Assert.True(produced.Count > 14);

            List<long> waits = new List<long>();
            waits.Add(fetchTime * 1 / 6); // because this test is heavy
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime);
            waits.Add(fetchTime);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime);

            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
            long sumWaits = 0;
            for (int i = 1; i <= 14; i++)
            {
                // connected to last message
                Assert.Equal(produced[i].TimeL - 1, produced[i - 1].TimeR);

                sumWaits += waits[i - 1];
                // produce a message at the correct time
                Assert.True(ApproxEqual(produced[i].TimeLog - produced[i - 1].TimeLog, waits[i - 1]));          // exact time may diverge by epsilon acculmulation
                // message is correct
                Assert.True(ApproxEqual(produced[i].TimeR, produced[i].TimeLog - fetchTime));
                Assert.True(ApproxEqual(produced[i].TimeR - produced[i].TimeL, waits[i - 1]));
            }
        }
        [Fact]
        public async void Trigger_Correctly_On_Filling_Up_Db_Dispersed()
        {
            var start = TimeUtils.now();
            var startTime = TimeUtils.DateTimeToEpochMs(start);
            await scheduler.Start();

            await Task.Delay((int)(fetchTime * 3 / 6));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b00", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                start.AddMilliseconds(fetchTime * 9 / 6 - fetchTime),
                start.AddMilliseconds(fetchTime * 9 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));

            await Task.Delay((int)(fetchTime * 7 / 6));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b01", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                start.AddMilliseconds(fetchTime * 16 / 6 - fetchTime),
                start.AddMilliseconds(fetchTime * 16 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b02", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                start.AddMilliseconds(fetchTime * 20 / 6 - fetchTime),
                start.AddMilliseconds(fetchTime * 20 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b03", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                start.AddMilliseconds(fetchTime * 31 / 6 - fetchTime),
                start.AddMilliseconds(fetchTime * 31 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));

            await Task.Delay((int)(fetchTime * 8 / 6));
            await capsuleRepository.Create(new Capsule(
                "0cade597-d092-40db-9a8d-589166f76b04", "sender@email.com", "Mr. Sender", "recipient@email.com", "Mr. Recipient", (ContentType)Enum.Parse(typeof(ContentType), "File"),
                start.AddMilliseconds(fetchTime * 24 / 6 - fetchTime),
                start.AddMilliseconds(fetchTime * 24 / 6 - fetchTime),
                (Status)Enum.Parse(typeof(Status), "Pending")
                ));

            await Task.Delay((int)(fetchTime * 19 / 6));
            await scheduler.Suspend();

            for (int i = 0; i < produced.Count; i++)
            {
                output.WriteLine(produced[i].TimeLog.ToString() + " : " + produced[i].TimeR.ToString() + " - " + produced[i].TimeL.ToString());
            }

            Assert.NotEmpty(produced);
            Assert.True(produced.Count > 9);

            List<long> waits = new List<long>();
            waits.Add(fetchTime);
            waits.Add(fetchTime * 3 / 6);
            waits.Add(fetchTime);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime * 4 / 6);
            waits.Add(fetchTime);
            waits.Add(fetchTime * 1 / 6);
            waits.Add(fetchTime);

            Assert.True(ApproxEqual(produced[0].TimeLog, startTime));
            long sumWaits = 0;
            for (int i = 1; i <= 9; i++)
            {
                // connected to last message
                Assert.Equal(produced[i].TimeL - 1, produced[i - 1].TimeR);

                sumWaits += waits[i - 1];
                // produce a message at the correct time
                Assert.True(ApproxEqual(produced[i].TimeLog - produced[i - 1].TimeLog, waits[i - 1]));          // exact time may diverge by epsilon acculmulation
                // message is correct
                Assert.True(ApproxEqual(produced[i].TimeR, produced[i].TimeLog - fetchTime));
                Assert.True(ApproxEqual(produced[i].TimeR - produced[i].TimeL, waits[i - 1]));
            }
        }
    }
}
