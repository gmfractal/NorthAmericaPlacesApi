using System.Text.Json.Serialization;

namespace NorthAmericaApi;

public class Country
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; } = "";

    [JsonPropertyName("states")]
    public StateProvince[]? StatesProvinces { get; set; }

}

public class StateProvince
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; } = "";
}

