using System.Threading;
using System.Threading.Tasks;

[Command("beta")]
sealed class BetaCommand : ICommand
{
    public Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        bool playedDuringBeta = false;

        if (ES3.KeyExists("playedDuringBeta", "LCGeneralSaveData"))
            playedDuringBeta = ES3.Load<bool>("playedDuringBeta", "LCGeneralSaveData");

        bool newValue = !playedDuringBeta;

        ES3.Save<bool>("playedDuringBeta", newValue, "LCGeneralSaveData");

        Chat.Print("Beta badge: " + (newValue ? "obtained" : "removed"));

        return Task.CompletedTask;
    }
}
