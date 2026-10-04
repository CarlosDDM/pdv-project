using System.Text.Json.Serialization;

namespace Pdv.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum CompanyType
{
    Matriz = 1,
    Filial = 2
}
