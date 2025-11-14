using apiBozzi.Models;
using Firebase.Storage;

namespace apiBozzi.Services.Firebase;

using File = Models.File;

public class StorageService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    private readonly string _bucketName = Environment.GetEnvironmentVariable("FIREBASE_STORAGE_BUCKET") ?? "";

    /// <summary>
    /// Faz upload de um arquivo para o Firebase Storage
    /// </summary>
    public async Task<File?> UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            var storage = new FirebaseStorage(_bucketName);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            await storage
                .Child("files")
                .Child(uniqueFileName)
                .PutAsync(fileStream);

            var url =
                $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/files%2F{Uri.EscapeDataString(uniqueFileName)}?alt=media";

            return new File
            {
                IdStorage = uniqueFileName,
                FileName = fileName,
                Url = url,
                FileSize = fileStream.Length,
                ContentType = GetContentType(fileName),
                UploadedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao fazer upload do arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Remove um arquivo do Firebase Storage
    /// </summary>
    public async Task<bool> DeleteFileAsync(string idStorage)
    {
        try
        {
            var storage = new FirebaseStorage(_bucketName);
            await storage
                .Child("files")
                .Child(idStorage)
                .DeleteAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Atualiza um arquivo (deleta o antigo e faz upload do novo)
    /// </summary>
    public async Task<File?> UpdateFileAsync(string idFileAntigo, Stream novoArquivo, string nomeArquivo)
    {
        try
        {
            await DeleteFileAsync(idFileAntigo);
            return await UploadFileAsync(novoArquivo, nomeArquivo);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtém a URL de download de um arquivo do Firebase Storage
    /// </summary>
    public async Task<string?> GetDownloadUrlAsync(string idStorage)
    {
        try
        {
            var storage = new FirebaseStorage(_bucketName);
            var link = await storage
                .Child("files")
                .Child(idStorage)
                .GetDownloadUrlAsync();

            return link;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter URL de download: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifica se um arquivo existe no Firebase Storage
    /// </summary>
    public async Task<bool> FileExistsAsync(string idStorage)
    {
        try
        {
            var storage = new FirebaseStorage(_bucketName);
            await storage
                .Child("files")
                .Child(idStorage)
                .GetMetaDataAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtém o tipo MIME do arquivo baseado na extensão
    /// </summary>
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}