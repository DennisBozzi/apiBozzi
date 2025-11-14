using apiBozzi.Context;
using apiBozzi.Services.FelicianoBozzi;
using apiBozzi.Services.Firebase;

namespace apiBozzi.Services;

public abstract class ServiceBase(IServiceProvider serviceProvider) : ServiceCollection
{
    // O ServiceBase foi criado com o intuido de diminuir a quantidade de instancias de context e httpClient.
    // Dessa forma não é necessário adicionar manualmente todos os novos services ao escopo.
    
    private HttpClient? _httpClient;
    private AppDbContext? _context;
    private FirebaseUserProvider? _userProvider;
    private FirebaseService? _firebaseService;
    private ApartmentService? _apartmentService;
    private TenantService? _tenantService;
    
    protected HttpClient HttpClient => _httpClient ??= ServiceProvider.GetRequiredService<HttpClient>();
    protected AppDbContext Context => _context ??= ServiceProvider.GetRequiredService<AppDbContext>();
    protected FirebaseUserProvider UserProvider => _userProvider = ServiceProvider.GetRequiredService<FirebaseUserProvider>();
    protected FirebaseService FirebaseService => _firebaseService = ServiceProvider.GetRequiredService<FirebaseService>();
    protected ApartmentService ApartmentService=> _apartmentService = serviceProvider.GetRequiredService<ApartmentService>();
    protected TenantService TenantService=> _tenantService = serviceProvider.GetRequiredService<TenantService>();

    private IServiceProvider ServiceProvider { get; } = serviceProvider;
}