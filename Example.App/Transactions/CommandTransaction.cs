namespace Example.App.Transactions;

public interface ICommand
{
}

public interface IContext<out TCommand> where TCommand : ICommand
{
    TCommand Command { get; }
}

public delegate Task<TContext> CreateContext<TContext, in TCommand>(TCommand command, CancellationToken ct)
    where TContext : IContext<TCommand>
    where TCommand : ICommand;

public delegate Task<TContext> CommitChanges<TContext, in TCommand>(TContext context, CancellationToken ct)
    where TContext : IContext<TCommand>
    where TCommand : ICommand;

public delegate Task<TContext> RollbackChanges<TContext, in TCommand>(TContext context, CancellationToken ct)
    where TContext : IContext<TCommand>
    where TCommand : ICommand;

public class CommandTransaction<TContext, TCommand> : IAsyncDisposable
    where TContext : IContext<TCommand>
    where TCommand : ICommand
{
    private readonly TCommand _command;
    private readonly CommitChanges<TContext, TCommand> _commitChanges;
    private readonly RollbackChanges<TContext, TCommand> _rollbackChanges;
    private bool _isCommitted;

    private CommandTransaction(TCommand command, CommitChanges<TContext, TCommand> commitChanges,
        RollbackChanges<TContext, TCommand> rollbackChanges)
    {
        _command = command;
        _commitChanges = commitChanges;
        _rollbackChanges = rollbackChanges;

        _isCommitted = false;
    }

    public TContext? Context { get; private set; }

    public static async Task<CommandTransaction<TContext, TCommand>> Begin(
        TCommand command,
        CreateContext<TContext, TCommand> createCreateContext,
        CommitChanges<TContext, TCommand> commitChanges,
        RollbackChanges<TContext, TCommand> rollbackChanges,
        CancellationToken ct)
    {
        var transaction = new CommandTransaction<TContext, TCommand>(command, commitChanges, rollbackChanges);

        try
        {
            transaction.Context = await createCreateContext(command, ct);
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
        if (Context != null)
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