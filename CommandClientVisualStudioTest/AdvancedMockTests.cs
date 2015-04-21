using System;
using System.Net;
using System.Reflection;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proshot.CommandClient;
using Rhino.Mocks;
using System.Linq;

namespace CommandClientVisualStudioTest
{
    [TestClass]
    public class AdvancedMockTests
    {
        private MockRepository mocks;

        [TestMethod]
        public void VerySimpleTest()
        {
            CMDClient client = new CMDClient(null, "Bogus network name");
            Assert.AreEqual("Bogus network name", client.NetworkName);
        }

        [TestInitialize()]
        public void Initialize()
        {
            mocks = new MockRepository();
        }

        [TestMethod]
        public void TestUserExitCommand()
        {
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            Command command = new Command(CommandType.UserExit, ipaddress, null);
            System.IO.Stream fakeStream = mocks.DynamicMock<System.IO.Stream>();
            byte[] commandBytes = { 0, 0, 0, 0 };
            byte[] ipLength = { 9, 0, 0, 0 };
            byte[] ip = { 49, 50, 55, 46, 48, 46, 48, 46, 49 };
            byte[] metaDataLength = { 2, 0, 0, 0 };
            byte[] metaData = { 10, 0 };

            using (mocks.Ordered())
            {
                fakeStream.Write(commandBytes, 0, 4);
                fakeStream.Flush();
                fakeStream.Write(ipLength, 0, 4);
                fakeStream.Flush();
                fakeStream.Write(ip, 0, 9);
                fakeStream.Flush();
                fakeStream.Write(metaDataLength, 0, 4);
                fakeStream.Flush();
                fakeStream.Write(metaData, 0, 2);
                fakeStream.Flush();
            }
            mocks.ReplayAll();
            CMDClient client = new CMDClient(null, "Bogus network name");
            typeof(CMDClient).GetField("networkStream", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(client, fakeStream);
            // we need to set the private variable here

            client.SendCommandToServerUnthreaded(command);
            mocks.VerifyAll();
            
        }

        [TestMethod]
        public void TestUserExitCommandWithoutMocks()
        {
            Console.WriteLine("HELLO");
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            Command command = new Command(CommandType.UserExit, ipaddress, null);
            MemoryStream fakeStream = new MemoryStream(23);
            byte[] commandBytes = { 0, 0, 0, 0 };
            byte[] ipLength = { 9, 0, 0, 0 };
            byte[] ip = { 49, 50, 55, 46, 48, 46, 48, 46, 49 };
            byte[] metaDataLength = { 2, 0, 0, 0 };
            byte[] metaData = { 10, 0 };

            CMDClient client = new CMDClient(null, "Bogus network name");
            typeof(CMDClient).GetField("networkStream", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(client, fakeStream);
            // we need to set the private variable here
            client.SendCommandToServerUnthreaded(command);
            //byte[] writtenData = fakeStream.ToArray();
            //fakeStream.Position = 0;
            //string str = new StreamReader(fakeStream).ReadToEnd();
            string data = System.Text.Encoding.Default.GetString(fakeStream.ToArray());
            Assert.IsTrue(data.Contains(System.Text.Encoding.Default.GetString(commandBytes)));
            Assert.IsTrue(data.Contains(System.Text.Encoding.Default.GetString(ipLength)));
            Assert.IsTrue(data.Contains(System.Text.Encoding.Default.GetString(ip)));
            Assert.IsTrue(data.Contains(System.Text.Encoding.Default.GetString(metaDataLength)));
            Assert.IsTrue(data.Contains(System.Text.Encoding.Default.GetString(metaData)));
        }

        [TestMethod]
        public void TestSemaphoreReleaseOnNormalOperation()
        {
            Assert.Fail("Not yet implemented");
        }

        [TestMethod]
        public void TestSemaphoreReleaseOnExceptionalOperation()
        {
            Assert.Fail("Not yet implemented");

        }
    }
}
