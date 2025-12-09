namespace apiBozzi.Models.Dtos;

public class DemoSeedPayload
{
    public List<DemoTenantDto> Tenants { get; set; } = new();
    public List<DemoApartmentDto> Apartments { get; set; } = new();
}

public class DemoTenantDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? Born { get; set; }
    public string? ResponsibleCpf { get; set; }
}

public class DemoApartmentDto
{
    public string Number { get; set; } = string.Empty;
    public decimal Rent { get; set; }
    public int Floor { get; set; }
    public int Type { get; set; }
    public string? ResponsibleCpf { get; set; }
}
