using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Services.Firebase;
using Microsoft.EntityFrameworkCore;
using Xceed.Words.NET;

namespace apiBozzi.Services.FelicianoBozzi;

public class ContractService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<ContractResponse> NewContract(NewContract dto)
    {
        dto.Tenant = await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.TenantId);
        dto.Unit = await Context.Units.FirstOrDefaultAsync(x => x.Id == dto.UnitId);

        ValidateContract(dto);

        var contract = new Contract(dto);
        contract.Status = StatusContract.Active;

        // TODO: Criar e salvar o contrato (File)

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

    #endregion

    #region Private

    private void ValidateContract(NewContract dto)
    {
        if (dto.Tenant is null)
            throw new ValidationException("No tenant found with this identifier.");

        if (dto.Unit is null)
            throw new ValidationException("No unit found with this identifier.");
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