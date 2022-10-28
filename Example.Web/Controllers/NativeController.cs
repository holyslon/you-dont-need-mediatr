using Example.App.Logging;
using Example.App.Metrics;
using Example.App.Native;
using Example.App.Tracing;
using Example.App.Transactions;
using Example.Web.Validation;
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
    ;

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input, CancellationToken ct)
    {
        var target = ValidateNullable.GetOrThrow(ctx => ctx.Get(input.target));
        using(_scope.WithScope(input))
        using(Elapsed.WithMeter<MediatRController.CalculateInput>())
        using(Trace.WithTrace<MediatRController.CalculateInput>())
        await using var transaction = await Transaction<CalculationContext>.Begin(
            createContext: _ => Task.FromResult(new CalculationContext(input.target.Value)),
            commitChanges: (_, _) => Task.CompletedTask,
            rollbackChanges: (_, _) => Task.CompletedTask,
            ct: ct);
        {
            var result = await _unit.DoCalulate(transaction.Context.Target);
            await transaction.Commit(ct);
            return result;
        }
    }
}