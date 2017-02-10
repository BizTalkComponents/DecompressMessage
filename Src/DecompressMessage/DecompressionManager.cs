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
            if (decompressor == null)
            {
                throw new ArgumentNullException("decompressor");
            }

            _decompressor = decompressor;
        }

        public Queue DecompressAndSplitMessage(IBaseMessage inMsg, IPipelineContext pctx)
        {
            var marketableleStream = GetSeekableStream(inMsg);

            var messages = _decompressor.DecompressMessage(marketableleStream);

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

        public IBaseMessage DecompressAndCombineMessage(IBaseMessage inMsg, IPipelineContext pctx)
        {
            var marketableStream = GetSeekableStream(inMsg);

            var messages = _decompressor.DecompressMessage(marketableStream).OrderByDescending(o => o.Key.EndsWith(".xml")).ToArray();

            IBaseMessage outMessage;
            outMessage = pctx.GetMessageFactory().CreateMessage();

            for (int i = 0; i < messages.Length; i++)
            {
                var msg = messages[i];

                if (i == 0)
                {
                    outMessage.AddPart("Body", pctx.GetMessageFactory().CreateMessagePart(), true);
                    outMessage.BodyPart.Data = msg.Value;
                    outMessage.Context = PipelineUtil.CloneMessageContext(inMsg.Context);
                    ContextExtensions.Promote(outMessage.Context, new ContextProperty(FileProperties.ReceivedFileName), msg.Key);
                    var docType = Microsoft.BizTalk.Streaming.Utils.GetDocType(GetMarketableStream(msg.Value));
                    ContextExtensions.Promote(outMessage.Context, new ContextProperty(SystemProperties.MessageType), docType);
                }
                else
                {
                    var part = pctx.GetMessageFactory().CreateMessagePart();
                    var appendix = "appendix" + i.ToString();
                    outMessage.AddPart(appendix, part, false);
                    part.Data = msg.Value;
                    part.PartProperties.Write(FileProperties.ReceivedFileName.Split('#')[1], FileProperties.ReceivedFileName.Split('#')[0], msg.Key);

                }

            }
            return outMessage;
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

        private MarkableForwardOnlyEventingReadStream GetMarketableStream(Stream inboundStream)
        {
            return inboundStream as MarkableForwardOnlyEventingReadStream ?? new MarkableForwardOnlyEventingReadStream(inboundStream);
        }
    }
}
