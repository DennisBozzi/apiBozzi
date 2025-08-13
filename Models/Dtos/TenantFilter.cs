using Microsoft.AspNetCore.Mvc.RazorPages;

namespace apiBozzi.Models.Dtos;

public class TenantFilter : PagedFilter
{
    public string? Name { get; set; }
}