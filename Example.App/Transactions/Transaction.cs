namespace Example.App.Transactions;

public interface IContext
{
}

public delegate Task<TContext> CreateContext<TContext>(CancellationToken ct)
    where TContext : IContext;

public delegate Task CommitChanges<in TContext>(TContext context, CancellationToken ct)
    where TContext : IContext;

public delegate Task RollbackChanges<in TContext>(TContext context, CancellationToken ct)
    where TContext : IContext;

public class Transaction<TContext> : IAsyncDisposable where TContext : IContext
{
    private readonly CommitChanges<TContext> _commitChanges;
    private readonly RollbackChanges<TContext> _rollbackChanges;
    private bool _isCommitted;

    private Transaction(CommitChanges<TContext> commitChanges,
        RollbackChanges<TContext> rollbackChanges)
    {
        _commitChanges = commitChanges;
        _rollbackChanges = rollbackChanges;

        _isCommitted = false;
        Context = default!;
    }

    public TContext Context { get; private set; }

    public static async Task<Transaction<TContext>> Begin(
        CreateContext<TContext> createContext,
        CommitChanges<TContext> commitChanges,
        RollbackChanges<TContext> rollbackChanges,
        CancellationToken ct)
    {
        var transaction = new Transaction<TContext>(commitChanges, rollbackChanges);

        try
        {
            transaction.Context = await createContext(ct);
            return transaction;
        }
        catch
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    public async Task Commit(CancellationToken ct)
    {
        if (Context is null)
        {
            await _commitChanges(Context, ct);
            _isCommitted = true;
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (Context != null && !_isCommitted)
        {
            await _rollbackChanges(Context, CancellationToken.None);
        }
    }
}