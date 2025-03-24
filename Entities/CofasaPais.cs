using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities;

[Keyless]
public class CofasaPais {

    public required int idPais { get; set; }
    [MaxLength(50)]
    public required string nombre { get; set; }
}