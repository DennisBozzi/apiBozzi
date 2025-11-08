using System.Text.Json;
using apiBozzi.Context;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class DemoSeedService(IServiceProvider sp) : ServiceBase(sp)
{
    public async Task<object> SeedFromFileAsync(string jsonPath)
    {
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Arquivo não encontrado: {jsonPath}");

        await using var fs = File.OpenRead(jsonPath);
        var payload = await JsonSerializer.DeserializeAsync<DemoSeedPayload>(fs, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (payload == null)
            throw new InvalidOperationException("JSON inválido.");

        return await SeedAsync(payload);
    }

    public async Task<object> SeedAsync(DemoSeedPayload payload)
    {
        await using var trx = await Context.Database.BeginTransactionAsync();

        try
        {
            await Context.ApartmentsDemo.ExecuteDeleteAsync();
            await Context.TenantsDemo.ExecuteDeleteAsync();

            var byCpf = new Dictionary<string, TenantDemo>();
            foreach (var t in payload.Tenants)
            {
                var ent = new TenantDemo
                {
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    Cpf = t.Cpf,
                    Email = t.Email,
                    Phone = t.Phone,
                    Born = t.Born
                };
                await Context.TenantsDemo.AddAsync(ent);
                byCpf[t.Cpf] = ent;
            }

            await Context.SaveChangesAsync();

            foreach (var t in payload.Tenants)
            {
                if (!string.IsNullOrWhiteSpace(t.ResponsibleCpf) && byCpf.TryGetValue(t.Cpf, out var child) &&
                    byCpf.TryGetValue(t.ResponsibleCpf!, out var resp))
                    child.Responsible = resp;
            }

            await Context.SaveChangesAsync();

            foreach (var a in payload.Apartments)
            {
                byCpf.TryGetValue(a.ResponsibleCpf ?? string.Empty, out var resp);
                var ap = new ApartmentDemo
                {
                    Number = a.Number.ToUpper(),
                    Rent = a.Rent,
                    Floor = (apiBozzi.Models.Enums.FloorEnum)a.Floor,
                    Type = (apiBozzi.Models.Enums.ApartmentTypeEnum)a.Type,
                    Responsible = resp
                };
                await Context.ApartmentsDemo.AddAsync(ap);
            }

            await Context.SaveChangesAsync();

            await trx.CommitAsync();

            return new { Tenants = byCpf.Count, Apartments = payload.Apartments.Count };
        }
        catch
        {
            await trx.RollbackAsync();
            throw;
        }
    }
}