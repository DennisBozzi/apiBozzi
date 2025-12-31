using System.Collections.Generic;

namespace apiBozzi.Models.Dtos;

public class ContractModelFillRequest
{
    public Dictionary<string, string> Values { get; set; } = new();
}