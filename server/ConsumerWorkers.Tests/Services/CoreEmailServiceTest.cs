using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Xunit;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Tests.Services
{
    public class CoreEmailServiceTest : IDisposable
    {
        private readonly ICoreEmailService coreEmailService;

        // this runs before every test
        public CoreEmailServiceTest()
        {
            coreEmailService = new CoreEmailService("TODO", "timelette@timelette.app", "Timelette App");
        }
        // this runs after every test
        public void Dispose() { }

        [Fact]
        public async void SendHtml()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = "../../../..";
            string fullPath = Path.Combine(currentDirectory, path, "Email.html");
            using (StreamReader r = new StreamReader(fullPath))
            {
                string html = r.ReadToEnd();
                await coreEmailService.SendHtml("TODO:email", "TODO:name", "Hello HTML", html);
            }
        }
    }
}
