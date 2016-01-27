using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using System.IO;

namespace BizTalkComponents.PipelineComponents.DecompressMessage.Tests.UnitTests
{
    [TestClass]
    public class ZipDisassemblerTests
    {
        [TestMethod]
        public void TestExtractNoOfFiles()
        {
            int expectedNoOfFilesInZip = 3;
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new ZipDisassembler();
            string zipPath = @"TestData\test.zip";
            pipeline.AddComponent(component, PipelineStage.Disassemble);

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
    }
}
