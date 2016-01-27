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
                var messages = _decompressionManager.DecompressAndSpliMessage(pInMsg, pContext);

                foreach(var msg in messages)
                {
                    _qOutMessages.Enqueue(msg);

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
