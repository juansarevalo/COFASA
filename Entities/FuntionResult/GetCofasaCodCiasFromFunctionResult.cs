using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities
{
    [Keyless]
    public class GetCofasaCodCiasFromFunctionResult {
        public string COD_CIA { get; set; }
        public string NOM_COMERCIAL { get; set; }
    }
}
