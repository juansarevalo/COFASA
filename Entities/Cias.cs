using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities
{
    [Table(CC.CIAS, Schema = CC.SCHEMA)]
    public class Cias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(3)]
        [Column("COD_CIA")]
        public required string CodCia { get; set; }

        [MaxLength(60)]
        [Column("RAZON_SOCIAL")]
        public string? RazonSocial { get; set; }

        [MaxLength(60)]
        [Column("NOM_COMERCIAL")]
        public string? NomComercial { get; set; }

        [MaxLength(3)]
        [Column("COD_CIA_CORE")]
        public required string CodCiaCore { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<UserCia>? UserCia { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<Role>? Role { get; set; }

    }
}
