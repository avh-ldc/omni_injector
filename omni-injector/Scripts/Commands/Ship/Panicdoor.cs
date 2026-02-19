using System.Threading;
using System.Threading.Tasks;

[Command("panicdoor")]
sealed class PanicDoorCommand : ICommand, IShipDoor
{
    private static CancellationTokenSource _cts;

    public async Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        // Si déjà en cours → on annule
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            return; // Stoppe ici (toggle OFF)
        }

        // Sinon on démarre
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            while (!token.IsCancellationRequested)
            {
                this.SetShipDoorState(false);
                await Task.Delay(1, token);

                this.SetShipDoorState(true);
                await Task.Delay(1, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Normal quand on annule
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }
}
