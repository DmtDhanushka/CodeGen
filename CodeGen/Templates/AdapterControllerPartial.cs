using CodeGen.Models;

namespace CodeGen.Templates
{
    public partial class AdapterController
    {
        public required ControllerModel ControllerMetaData { get; set; }
        public required string EndpointsCode { get; set; }
    }
}
