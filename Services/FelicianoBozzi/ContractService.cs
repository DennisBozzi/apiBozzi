using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Services.Firebase;
using Microsoft.EntityFrameworkCore;
using Xceed.Words.NET;
using File = apiBozzi.Models.File;
using NumerosExtensos;
using NumerosExtensos.Enums;

namespace apiBozzi.Services.FelicianoBozzi;

public class ContractService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<ContractResponse> GetContractById(int id)
    {
        var contract = await Context.Contracts.FindAsync(id);

        if (contract is null)
            throw new ValidationException("Contract not found.");

        return new ContractResponse(contract);
    }

    public async Task<ContractResponse> GetContractByUnit(int id)
    {
        var contract = await GetValidContractByUnit(id);

        if (contract is not null)
            return new ContractResponse(contract);

        var res = new ContractResponse()
        {
            Unit = new UnitResponse(await Context.Units.FindAsync(id))
        };

        return res;
    }

    public async Task<List<ContractResponse>> GetContractsByTenant(int id, bool isActive = true)
    {
        var contracts = isActive
            ? await Context.Contracts
                .Where(x => x.Tenant.Id == id && x.Status == StatusContract.Active)
                .ToListAsync()
            : await Context.Contracts
                .Where(x => x.Tenant.Id == id).ToListAsync();

        var res = contracts.Select(x => new ContractResponse(x)).ToList();

        return res;
    }

    public async Task<List<UnitResponse>> FillContracts(List<UnitResponse> units)
    {
        var today = DateTime.Today.ToUniversalTime();
        foreach (var unit in units)
        {
            var contract = await Context.Contracts
                .FirstOrDefaultAsync(x => x.Unit.Id == unit.Id &&
                                          x.Status == StatusContract.Active &&
                                          x.ValidUntil > today);

            if (contract is not null)
                unit.FillContract(contract);
        }

        return units;
    }

    public async Task<ContractResponse> NewContract(NewContract dto)
    {
        dto.Tenant = await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.TenantId);
        dto.Unit = await Context.Units.FirstOrDefaultAsync(x => x.Id == dto.UnitId);
        dto.Contract = await Context.Contracts
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.Unit == dto.Unit);

        ValidateContract(dto);

        var contract = new Contract(dto);

        contract.Status = StatusContract.Active;
        contract.File = await FillContractFile(dto);

        Context.Contracts.Add(contract);

        return new ContractResponse(contract);
    }

    public async Task<ContractModelResponse> NewModel(IFormFile file)
    {
        ValidateNewModel(file);

        var res = new ContractModelResponse();

        res.File = await FileService
            .UploadContractAndSaveAsync(file.OpenReadStream(), file.FileName, FileType.ModelContract);

        if (res.File is null)
            return res;

        res.Params = await ExtractPlaceholdersAsync(res.File.IdStorage);

        return res;
    }

    public async Task<ContractModelResponse> GetModel()
    {
        var res = new ContractModelResponse();

        var modelFile = await Context.Files
            .Where(x => x.Type == FileType.ModelContract)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (modelFile is null)
            return res;

        res.File = modelFile;
        res.Params = await ExtractPlaceholdersAsync(modelFile.IdStorage);
        return res;
    }

    public async Task<(string FileName, MemoryStream Stream)> FillModelAsync(ContractModelFillRequest request)
    {
        if (request.Values == null || request.Values.Count == 0)
            throw new ValidationException("At least one placeholder value must be provided.");

        var modelFile = await Context.Files
            .Where(x => x.Type == FileType.ModelContract)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (modelFile is null)
            throw new ValidationException("No contract model is available.");

        foreach (var placeholder in request.Values.Keys)
        {
            if (!placeholder.StartsWith("{") || !placeholder.EndsWith("}"))
                throw new ValidationException("Placeholders must be wrapped in braces, e.g., {name}.");
        }

        var storageService = ServiceProvider.GetRequiredService<StorageService>();
        using var sourceStream = await storageService.GetFileStreamAsync(modelFile.IdStorage);
        using var document = DocX.Load(sourceStream);

        foreach (var (placeholder, replacement) in request.Values)
            document.ReplaceText(placeholder, replacement ?? string.Empty);

        var outputStream = new MemoryStream();
        document.SaveAs(outputStream);
        outputStream.Position = 0;

        var filledFileName = modelFile.FileName;
        if (!filledFileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            filledFileName += ".docx";

        return (filledFileName, outputStream);
    }

    private async Task<Contract?> GetValidContractByUnit(int id)
    {
        var today = DateTime.Today.ToUniversalTime();

        return await Context.Contracts
            .FirstOrDefaultAsync(x =>
                x.Unit.Id == id &&
                x.ValidUntil > today &&
                x.Status == StatusContract.Active);
    }

    #endregion

    #region Private

    private async Task<File> FillContractFile(NewContract dto)
    {
        var model = await Context.Files
            .Where(x => x.Type == FileType.ModelContract)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (model is null)
            throw new ValidationException("No contract model is available.");

        var storageService = ServiceProvider.GetRequiredService<StorageService>();
        using var sourceStream = await storageService.GetFileStreamAsync(model.IdStorage);
        using var document = DocX.Load(sourceStream);

        var extenso = new Extenso();
        var escrever = extenso
            .Escrever(OpcoesPredefinidas.Predefinicoes[Predefinicoes.Cardinais]);

        var genderSuffix = dto.Tenant.Gender switch
        {
            Gender.Male => "brasileiro",
            Gender.Female => "brasileira",
            _ => "brasileiro(a)"
        };

        var maritalStatus = dto.Tenant.MaritalStatus switch
        {
            MaritalStatus.Single => "solteir" + (dto.Tenant.Gender == Gender.Female ? "a" : "o"),
            MaritalStatus.Married => "casad" + (dto.Tenant.Gender == Gender.Female ? "a" : "o"),
            MaritalStatus.Widowed => "viúv" + (dto.Tenant.Gender == Gender.Female ? "a" : "o"),
            _ => "solteir" + (dto.Tenant.Gender == Gender.Female ? "a" : "o")
        };

        var replacements = new Dictionary<string, string>
        {
            { "{firstName}", dto.Tenant.FirstName },
            { "{lastName}", dto.Tenant.LastName },
            { "{brazilian}", genderSuffix },
            { "{merried}", maritalStatus },
            { "{ap}", dto.Unit.Number },
            { "{cpf}", dto.Tenant.Cpf },
            { "{validSince}", dto.ValidSince.ToString("dd/MM/yyyy") },
            { "{validUntil}", dto.ValidUntil.ToString("dd/MM/yyyy") },
            { "{rent}", dto.Rent.ToString("C") },
            { "{rentWorld}", escrever.Numero(dto.Rent) }
        };

        foreach (var (placeholder, replacement) in replacements)
            document.ReplaceText(placeholder, replacement);

        var outputStream = new MemoryStream();
        document.SaveAs(outputStream);
        outputStream.Position = 0;

        var fileName = $"Contract_{dto.Tenant.Id}_{DateTime.Now:yyyyMMddHHmmss}.docx";
        var file = await FileService.UploadContractAndSaveAsync(outputStream, fileName, FileType.Contract);

        return file;
    }

    private void ValidateContract(NewContract dto)
    {
        if (dto.Tenant is null)
            throw new ValidationException("No tenant found with this identifier.");

        if (dto.Unit is null)
            throw new ValidationException("No unit found with this identifier.");

        var today = DateTime.Today;

        var hasActiveContract = dto.Contract is not null &&
                                today < dto.Contract.ValidUntil &&
                                dto.Contract.Status == StatusContract.Active;

        if (hasActiveContract)
            throw new ValidationException("The unit has an contract.");
    }

    private void ValidateNewModel(IFormFile file)
    {
        if (file is null)
            throw new ValidationException("File cannot be null.");

        if (!file.FileName.EndsWith(".docx"))
            throw new ValidationException("The file type is not supported.");
    }

    private async Task<List<string>> ExtractPlaceholdersAsync(string idStorage)
    {
        try
        {
            var storageService = ServiceProvider.GetRequiredService<StorageService>();
            using var stream = await storageService.GetFileStreamAsync(idStorage);
            using var document = DocX.Load(stream);

            var matches = Regex.Matches(document.Text, "\\{([^{}]+)\\}");
            return matches.Select(m => m.Value).Distinct().ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    #endregion
}