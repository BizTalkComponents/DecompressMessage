using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.IO;
using Microsoft.BizTalk.Streaming;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.DecompressMessage
{
    [System.Runtime.InteropServices.Guid("A5CDE812-B7F8-4AFB-B36F-61E3AB0EE563")]
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    public partial class ZipDisassembler : IBaseComponent,
                            IDisassemblerComponent,
                            IComponentUI
                            
    {
        private readonly DecompressionManager _decompressionManager;
        private System.Collections.Queue _qOutMessages = new System.Collections.Queue();

        public ZipDisassembler()
        {
            _decompressionManager = new DecompressionManager(new ZipDecompressor.ZipDecompressor());
        }

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            if (bodyPart != null)
            {

                Stream inboundStream = bodyPart.GetOriginalDataStream();
                VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
                ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

                readOnlySeekableStream.Position = 0;
                readOnlySeekableStream.Seek(0, SeekOrigin.Begin);
                var messages = _decompressionManager.DecompressMessage(readOnlySeekableStream);
                IBaseMessage outMessage;

                foreach (var msg in messages)
                {
                    outMessage = pContext.GetMessageFactory().CreateMessage();
                    outMessage.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
                    outMessage.BodyPart.Data = msg.Value;
                    ContextExtensions.Promote(pInMsg.Context, new ContextProperty(FileProperties.ReceivedFileName), msg.Key);
                    outMessage.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
                    _qOutMessages.Enqueue(outMessage);
                }
            }
        }

        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (_qOutMessages.Count > 0)
                return (IBaseMessage)_qOutMessages.Dequeue();
            else
                return null;
        }
              
    }
}
