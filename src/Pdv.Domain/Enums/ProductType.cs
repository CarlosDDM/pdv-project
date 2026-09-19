using System.Text.Json.Serialization;

namespace Pdv.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductType
{
    Unit = 1,
    Kg = 2,
    Lt = 3,
}
