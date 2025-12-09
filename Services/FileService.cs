using apiBozzi.Models;
using apiBozzi.Services.Firebase;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services;

using File = Models.File;

public class FileService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    /// <summary>
    /// Salva um arquivo no banco de dados
    /// </summary>
    public async Task<File?> SaveFileAsync(File file)
    {
        try
        {
            Context.Files.Add(file);
            await Context.SaveChangesAsync();
            return file;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao salvar arquivo no banco: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Recupera um arquivo do banco de dados pelo ID
    /// </summary>
    public async Task<File?> GetFileByIdAsync(int fileId)
    {
        try
        {
            return await Context.Files?.FirstOrDefaultAsync(f => f.Id == fileId)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Recupera um arquivo pelo ID de storage (Firebase)
    /// </summary>
    public async Task<File?> GetFileByIdStorageAsync(string idStorage)
    {
        try
        {
            return await Context.Files.FirstOrDefaultAsync(f => f.IdStorage == idStorage)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao recuperar arquivo por IdStorage: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deleta um arquivo do banco de dados
    /// </summary>
    public async Task<bool> DeleteFileAsync(int fileId)
    {
        try
        {
            var file = await Context.Files?.FirstOrDefaultAsync(f => f.Id == fileId)!;
            if (file == null)
                return false;

            Context.Files?.Remove(file);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar arquivo do banco: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Atualiza um arquivo no banco de dados
    /// </summary>
    public async Task<File?> UpdateFileAsync(File file)
    {
        try
        {
            Context.Files?.Update(file);
            await Context.SaveChangesAsync();
            return file;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar arquivo no banco: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Faz upload de arquivo e salva no banco (operação completa)
    /// </summary>
    public async Task<File?> UploadAndSaveAsync(Stream fileStream, string fileName)
    {
        try
        {
            var storageService = serviceProvider.GetRequiredService<StorageService>();
            var file = await storageService.UploadFileAsync(fileStream, fileName);

            if (file == null)
                return null;

            return await SaveFileAsync(file);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao fazer upload e salvar arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deleta arquivo do Firebase e do banco (operação completa)
    /// </summary>
    public async Task<bool> DeleteAndRemoveAsync(int fileId)
    {
        try
        {
            var file = await GetFileByIdAsync(fileId);
            if (file == null)
                return false;

            var storageService = serviceProvider.GetRequiredService<StorageService>();
            await storageService.DeleteFileAsync(file.IdStorage);

            return await DeleteFileAsync(fileId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar arquivo do Firebase e banco: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Atualiza arquivo do Firebase e do banco (operação completa)
    /// </summary>
    public async Task<File?> UpdateAndReplaceAsync(int fileId, Stream novoArquivo, string nomeArquivo)
    {
        try
        {
            var arquivo = await GetFileByIdAsync(fileId);
            if (arquivo == null)
                return null;

            var storageService = serviceProvider.GetRequiredService<StorageService>();
            var novoFile = await storageService.UpdateFileAsync(arquivo.IdStorage, novoArquivo, nomeArquivo);

            if (novoFile == null)
                return null;

            arquivo.IdStorage = novoFile.IdStorage;
            arquivo.Url = novoFile.Url;
            arquivo.FileName = novoFile.FileName;
            arquivo.FileSize = novoFile.FileSize;
            arquivo.ContentType = novoFile.ContentType;
            arquivo.CreatedAt = novoFile.CreatedAt;

            return await UpdateFileAsync(arquivo);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar e substituir arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Recupera a URL de download do arquivo
    /// </summary>
    public async Task<string?> GetDownloadUrlAsync(int fileId)
    {
        try
        {
            var file = await GetFileByIdAsync(fileId);
            if (file == null)
                return null;

            var storageService = serviceProvider.GetRequiredService<StorageService>();
            return await storageService.GetDownloadUrlAsync(file.IdStorage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter URL de download: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifica se arquivo existe no Firebase
    /// </summary>
    public async Task<bool> FileExistsInStorageAsync(int fileId)
    {
        try
        {
            var file = await GetFileByIdAsync(fileId);
            if (file == null)
                return false;

            var storageService = serviceProvider.GetRequiredService<StorageService>();
            return await storageService.FileExistsAsync(file.IdStorage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao verificar existência do arquivo: {ex.Message}", ex);
        }
    }
}