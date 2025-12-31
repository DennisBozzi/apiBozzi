namespace apiBozzi.Models.Responses;

public class ContractModelResponse
{
    public File? File { get; set; }
    public List<string> Params { get; set; } = new();
}