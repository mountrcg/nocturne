using System.Text.Json.Serialization;
using Nocturne.Core.Models.Attributes;

namespace Nocturne.Core.Models;

public class BodyWeight : ProcessableDocumentBase
{
    [JsonPropertyName("_id")]
    public override string? Id { get; set; }

    [JsonPropertyName("created_at")]
    public override string? CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

    [JsonPropertyName("mills")]
    public override long Mills { get; set; }

    [JsonPropertyName("utcOffset")]
    public override int? UtcOffset { get; set; }

    [JsonPropertyName("weightKg")]
    public decimal WeightKg { get; set; }

    [JsonPropertyName("bodyFatPercent")]
    public decimal? BodyFatPercent { get; set; }

    [JsonPropertyName("leanMassKg")]
    public decimal? LeanMassKg { get; set; }

    [JsonPropertyName("device")]
    [Sanitizable]
    public string? Device { get; set; }

    [JsonPropertyName("enteredBy")]
    [Sanitizable]
    public string? EnteredBy { get; set; }

    [JsonPropertyName("data_source")]
    [NocturneOnly]
    public string? DataSource { get; set; }
}
