using System.Text.Json.Serialization;

namespace YGOSharp.OCGWrapper
{
    // JSON schema matches serde serialization of ygopro_cdb_reader::Card.
    public sealed class CardRecord
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("desc")]
        public string Description { get; set; }
        [JsonPropertyName("hint")]
        public string[] Hint { get; set; }
        [JsonPropertyName("code")]
        public uint Code { get; set; }
        [JsonPropertyName("alias")]
        public uint Alias { get; set; }
        [JsonPropertyName("setcode")]
        public ushort[] Setcode { get; set; }
        [JsonPropertyName("card_type")]
        public uint CardType { get; set; }
        [JsonPropertyName("level")]
        public uint Level { get; set; }
        [JsonPropertyName("attribute")]
        public uint Attribute { get; set; }
        [JsonPropertyName("race")]
        public uint Race { get; set; }
        [JsonPropertyName("attack")]
        public int Attack { get; set; }
        [JsonPropertyName("defense")]
        public int Defense { get; set; }
        [JsonPropertyName("lscale")]
        public uint LScale { get; set; }
        [JsonPropertyName("rscale")]
        public uint RScale { get; set; }
        [JsonPropertyName("link_marker")]
        public uint LinkMarker { get; set; }
        [JsonPropertyName("ot")]
        public byte Ot { get; set; }
        [JsonPropertyName("category")]
        public uint Category { get; set; }
    }

    [JsonSerializable(typeof(CardRecord[]))]
    public partial class CardRecordJsonContext : JsonSerializerContext
    {
    }
}
