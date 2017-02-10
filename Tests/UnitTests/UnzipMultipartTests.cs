using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using System.IO;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.DecompressMessage.Tests.UnitTests
{
    [TestClass]
    public class UnzipMultipartTests
    {
        [TestMethod]
        public void TestExtractNoOfFilesSingleMessage()
        {
            int expectedNoOfFilesInZip = 1;
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new UnzipMultipart();
            string zipPath = @"TestData\testMultipart.zip";
            pipeline.AddComponent(component, PipelineStage.Decode);

            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    var message = MessageHelper.CreateFromStream(sr.BaseStream);
                    var output = pipeline.Execute(message);

                    Assert.AreEqual(expectedNoOfFilesInZip, output.Count);
                }
            }
        }

        [TestMethod]
        public void TestExtractionAndCreationOfSingleMessage()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new UnzipMultipart();
            string zipPath = @"TestData\testMultiPart.zip";
            pipeline.AddComponent(component, PipelineStage.Decode);
            MessageCollection output;
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    var message = MessageHelper.CreateFromStream(sr.BaseStream);
                    output = pipeline.Execute(message);
                }
            }

            Assert.AreEqual(2, output[0].PartCount);
            string nametwo;
            Assert.AreEqual("testfile1.xml", output[0].Context.Read(new ContextProperty(FileProperties.ReceivedFileName)));
            var splitter = FileProperties.ReceivedFileName.Split('#');
            Assert.AreEqual("testfile2.pdf", output[0].GetPartByIndex(1, out nametwo).PartProperties.Read(splitter[1], splitter[0]));
            Assert.AreEqual("appendix1", nametwo);
        }

        [TestMethod]
        public void TestMessageTypeOfBodyPartMessage()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new UnzipMultipart();
            string zipPath = @"TestData\testMultiPart.zip";
            pipeline.AddComponent(component, PipelineStage.Decode);
            MessageCollection output;
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    var message = MessageHelper.CreateFromStream(sr.BaseStream);
                    output = pipeline.Execute(message);
                }
            }
            var messageType = output[0].Context.Read(new ContextProperty(SystemProperties.MessageType));
            Assert.AreEqual("urn:nordicebuilding:Invoice:1.2.2#Invoic", messageType);
        }
    }
}
