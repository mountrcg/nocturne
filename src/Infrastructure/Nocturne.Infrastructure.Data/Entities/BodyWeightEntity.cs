using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nocturne.Infrastructure.Data.Entities;

[Table("body_weights")]
public class BodyWeightEntity : ITenantScoped
{
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [Key]
    public Guid Id { get; set; }

    [Column("original_id")]
    [MaxLength(24)]
    public string? OriginalId { get; set; }

    [Column("mills")]
    public long Mills { get; set; }

    [Column("weight_kg")]
    public decimal WeightKg { get; set; }

    [Column("body_fat_percent")]
    public decimal? BodyFatPercent { get; set; }

    [Column("lean_mass_kg")]
    public decimal? LeanMassKg { get; set; }

    [Column("device")]
    [MaxLength(255)]
    public string? Device { get; set; }

    [Column("entered_by")]
    [MaxLength(255)]
    public string? EnteredBy { get; set; }

    [Column("created_at")]
    [MaxLength(50)]
    public string? CreatedAt { get; set; }

    [Column("utc_offset")]
    public int? UtcOffset { get; set; }

    [Column("sys_created_at")]
    public DateTime SysCreatedAt { get; set; } = DateTime.UtcNow;

    [Column("sys_updated_at")]
    public DateTime SysUpdatedAt { get; set; } = DateTime.UtcNow;
}
