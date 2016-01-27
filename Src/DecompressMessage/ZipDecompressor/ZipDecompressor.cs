using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BizTalkComponents.PipelineComponents.DecompressMessage.ZipDecompressor
{
    public class ZipDecompressor : IMessageDecompressor
    {
        public ICollection<KeyValuePair<string, Stream>> DecompressMessage(Stream inputStream)
        {
            var messageParts = new List<KeyValuePair<string, Stream>>();

            using (ZipArchive zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    MemoryStream entryStream = new MemoryStream();
                    byte[] entrybuffer = new Byte[1024];
                    int entryBytesRead = 1024;
                    Stream zipArchiveEntryStream = entry.Open();
                    while (entryBytesRead != 0)
                    {
                        entryBytesRead = zipArchiveEntryStream.Read(entrybuffer, 0, entrybuffer.Length);
                        entryStream.Write(entrybuffer, 0, entryBytesRead);
                    }

                    entryStream.Position = 0;
                    messageParts.Add(new KeyValuePair<string, Stream>(entry.Name, entryStream));
                }
            }

            return messageParts;
        }
    }
}
