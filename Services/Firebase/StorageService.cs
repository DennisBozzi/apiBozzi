﻿using apiBozzi.Models;
using apiBozzi.Utils;
using Firebase.Storage;
using System.IO;

namespace apiBozzi.Services.Firebase;

using File = Models.File;

public class StorageService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    private readonly string _bucketName = Environment.GetEnvironmentVariable("FIREBASE_STORAGE_BUCKET") ?? string.Empty;

    private string BucketName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_bucketName))
            {
                throw new InvalidOperationException(
                    "The FIREBASE_STORAGE_BUCKET environment variable is not configured.");
            }

            return _bucketName;
        }
    }

    private FirebaseStorage CreateStorage(string token)
    {
        return new FirebaseStorage(BucketName, new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(token)
        });
    }

    private string EnsureAuthToken()
    {
        var token = UserProvider.AuthToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Usuário não autenticado: cabeçalho Authorization ausente ou inválido.");
        }

        return token;
    }

    /// <summary>
    /// Faz upload de um arquivo para o Firebase Storage
    /// </summary>
    public async Task<File?> UploadFileAsync(Stream fileStream, string fileName, string path = "contracts")
    {
        try
        {
            var token = EnsureAuthToken();
            var storage = CreateStorage(token);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            await storage
                .Child(path)
                .Child(uniqueFileName)
                .PutAsync(fileStream);

            var url =
                $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(path)}%2F{Uri.EscapeDataString(uniqueFileName)}?alt=media";

            return new File
            {
                IdStorage = uniqueFileName,
                FileName = fileName,
                Url = url,
                FileSize = fileStream.Length,
                ContentType = Util.GetContentType(fileName),
                CreatedAt = DateTime.UtcNow
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
            var storage = CreateStorage(EnsureAuthToken());
            await storage
                .Child("contracts")
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
    /// Baixa o arquivo do Firebase Storage em memória
    /// </summary>
    public async Task<MemoryStream> GetFileStreamAsync(string idStorage, string path = "contracts")
    {
        try
        {
            var token = EnsureAuthToken();
            var storage = CreateStorage(token);

            var reference = storage
                .Child(path)
                .Child(idStorage);

            var downloadUrl = await reference.GetDownloadUrlAsync();
            if (string.IsNullOrWhiteSpace(downloadUrl))
                throw new InvalidOperationException("Não foi possível obter a URL do arquivo.");

            var content = await HttpClient.GetByteArrayAsync(downloadUrl);
            return new MemoryStream(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao baixar arquivo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtém a URL de download de um arquivo do Firebase Storage
    /// </summary>
    public async Task<string?> GetDownloadUrlAsync(string idStorage)
    {
        try
        {
            var storage = CreateStorage(EnsureAuthToken());
            var link = await storage
                .Child("contracts")
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
            var storage = CreateStorage(EnsureAuthToken());
            await storage
                .Child("contracts")
                .Child(idStorage)
                .GetMetaDataAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}