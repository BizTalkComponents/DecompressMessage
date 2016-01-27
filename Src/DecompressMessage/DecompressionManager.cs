using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections;
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

        public Queue DecompressAndSpliMessage(IBaseMessage inMsg, IPipelineContext pctx)
        {
            var readOnlySeekableStream = GetSeekableStream(inMsg);

            var messages = _decompressor.DecompressMessage(readOnlySeekableStream);

            var outMsgs = new Queue();
            IBaseMessage outMessage;
            foreach (var msg in messages)
            {
                outMessage = pctx.GetMessageFactory().CreateMessage();
                outMessage.AddPart("Body", pctx.GetMessageFactory().CreateMessagePart(), true);
                outMessage.BodyPart.Data = msg.Value;
                outMessage.Context = PipelineUtil.CloneMessageContext(inMsg.Context);
                ContextExtensions.Promote(outMessage.Context, new ContextProperty(FileProperties.ReceivedFileName), msg.Key);

                outMsgs.Enqueue(outMessage);
            }
            return outMsgs;
        }

        private ReadOnlySeekableStream GetSeekableStream(IBaseMessage msg)
        {
            Stream inboundStream = msg.BodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            readOnlySeekableStream.Position = 0;
            readOnlySeekableStream.Seek(0, SeekOrigin.Begin);

            return readOnlySeekableStream;
        }
    }
}
