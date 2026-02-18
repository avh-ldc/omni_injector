using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("invis")]
sealed class InvisibleCommand : ICommand
{
    private static CancellationTokenSource invisibilityToken;

    private static async Task MaintainInvisible(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Helper.LocalPlayer != null && Setting.EnableInvisible)
            {
                Vector3 pos = StartOfRound.Instance.shipHasLanded
                    ? StartOfRound.Instance.notSpawnedPosition.position
                    : Vector3.zero;

                // Déplace le joueur côté serveur, tout en restant audible
                Helper.LocalPlayer.UpdatePlayerPositionServerRpc(pos, true, false, false, true);
            }

            await Task.Delay(50, token); // met à jour toutes les 50ms
        }
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        // Stop la boucle si on désactive l’invisibilité
        invisibilityToken?.Cancel();

        if (Setting.EnableInvisible)
        {
            invisibilityToken = new CancellationTokenSource();
            _ = MaintainInvisible(invisibilityToken.Token); // lance la boucle sans attendre
        }
    }
}
