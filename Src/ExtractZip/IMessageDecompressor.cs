using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.PipelineComponents.ExtractZip
{
    public interface IMessageDecompressor
    {
        ICollection<KeyValuePair<string, Stream>> DecompressMessage(Stream inputStream); 
    }
}
