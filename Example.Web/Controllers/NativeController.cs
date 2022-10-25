using Example.App.Logging;
using Example.App.Native;
using Example.App.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

[ApiController]
[Route("native")]
public class NativeController : ControllerBase
{
    private readonly CalculationUnit _unit;
    private readonly IScope _scope;

    public NativeController(CalculationUnit unit)
    {
        _unit = unit;
    }

    public record CalculateInput(int? target)
    {
    }

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input, CancellationToken ct)
    {
        if (!input.target.HasValue)
        {
            throw new BadHttpRequestException($"{nameof(MediatRController.CalculateInput.target)} should be present",
                400);
        }

        using var _ = _scope.WithScope(input);
        await using var transaction = await Transaction<CalculationContext>.Begin(
            createContext: _ => Task.FromResult(new CalculationContext(input.target.Value)),
            commitChanges: (_, _) => Task.CompletedTask,
            rollbackChanges: (_, _) => Task.CompletedTask,
            ct: ct);
        var result = await _unit.DoCalulate(transaction.Context.Target);
        await transaction.Commit(ct);
        return result;
    }
}