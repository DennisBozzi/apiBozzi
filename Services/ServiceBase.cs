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
    private UnitService? _apartmentService;
    private TenantService? _tenantService;
    private FileService? _fileService;
    private ContractService? _contractService;
    
    protected HttpClient HttpClient => _httpClient ??= ServiceProvider.GetRequiredService<HttpClient>();
    protected AppDbContext Context => _context ??= ServiceProvider.GetRequiredService<AppDbContext>();
    protected FirebaseUserProvider UserProvider => _userProvider = ServiceProvider.GetRequiredService<FirebaseUserProvider>();
    protected FirebaseService FirebaseService => _firebaseService = ServiceProvider.GetRequiredService<FirebaseService>();
    protected UnitService UnitService => _apartmentService = serviceProvider.GetRequiredService<UnitService>();
    protected TenantService TenantService => _tenantService = serviceProvider.GetRequiredService<TenantService>();
    protected FileService FileService => _fileService = serviceProvider.GetRequiredService<FileService>();
    protected ContractService ContractService => _contractService = serviceProvider.GetRequiredService<ContractService>();
    
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;
}