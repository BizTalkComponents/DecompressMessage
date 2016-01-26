using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.PipelineComponents.DecompressMessage
{
    public class DecompressionManager
    {
        private readonly IMessageDecompressor _decompressor;

        public DecompressionManager(IMessageDecompressor decompressor)
        {
            if(decompressor == null)
            {
                throw new ArgumentNullException("decompressor");
            }

            _decompressor = decompressor;
        }

        public ICollection<KeyValuePair<string,Stream>> DecompressMessage(Stream msg)
        {
            return _decompressor.DecompressMessage(msg);
        }
    }
}
