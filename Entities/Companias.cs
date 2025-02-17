using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities
{
    [Table(CC.COMPANIAS, Schema = CC.SCHEMA)]
    public class Companias
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

        [MaxLength(20)]
        [Column("NRC")]
        public string? NRC { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<UserCia>? UserCia { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<Role>? Role { get; set; }

    }
}
