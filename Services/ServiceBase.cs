using apiBozzi.Context;

namespace apiBozzi.Services;

public abstract class ServiceBase(IServiceProvider serviceProvider) : ServiceCollection
{
    // O ServiceBase foi criado com o intuido de diminuir a quantidade de instancias de context e httpClient.
    // Dessa forma não é necessário adicionar manualmente todos os novos services ao escopo.
    
    private HttpClient? _httpClient;
    private AppDbContext? _context;
    protected HttpClient HttpClient => _httpClient ??= ServiceProvider.GetRequiredService<HttpClient>();
    protected AppDbContext Context => _context ??= ServiceProvider.GetRequiredService<AppDbContext>();

    private IServiceProvider ServiceProvider { get; } = serviceProvider;
}