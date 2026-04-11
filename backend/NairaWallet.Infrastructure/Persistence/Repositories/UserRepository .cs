namespace NairaWallet.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByWalletTagAsync(WalletTag Tag, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Wallet != null && u.Wallet.Tag == Tag, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }


    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

}
