using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities;

[Keyless]
public class CofasaTipoMov {

    public required int idTipoMov { get; set; }
    [MaxLength(15)]
    public required string nombre { get; set; }
}