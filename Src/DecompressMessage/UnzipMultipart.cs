using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.IO;
using Microsoft.BizTalk.Streaming;
using BizTalkComponents.Utils;


namespace BizTalkComponents.PipelineComponents.DecompressMessage
{
    [System.Runtime.InteropServices.Guid("3284E018-4F4E-40D8-81AA-CF24ED4519DA")]
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public partial class UnzipMultipart : IBaseComponent,
                            IComponentUI,
                            IComponent
    {
        private readonly DecompressionManager _decompressionManager;

        public UnzipMultipart()
        {
            _decompressionManager = new DecompressionManager(new ZipDecompressor.ZipDecompressor());
        }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            
            if (pInMsg != null)
            {
               return _decompressionManager.DecompressAndCombineMessage(pInMsg, pContext);
            }
            else
            {
                throw new System.ArgumentException();
            }

            
        }

    }
}