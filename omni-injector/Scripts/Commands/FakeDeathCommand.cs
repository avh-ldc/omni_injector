using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("crash")]
sealed class crashCommand : ICommand
{
    public async Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Setting.EnableFakeDeath = true;

        int bodyCount = 1000;
        float spread = 5f;

        for (int i = 0; i < bodyCount; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-spread, spread),
                Random.Range(1f, 3f),
                Random.Range(-spread, spread)
            );

            Vector3 randomVelocity = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(0f, 5f),
                Random.Range(-2f, 2f)
            );

            player.KillPlayerServerRpc(
                playerId: player.PlayerIndex(),
                spawnBody: true,
                bodyVelocity: randomVelocity,
                causeOfDeath: unchecked((int)CauseOfDeath.Unknown),
                deathAnimation: 0,
                positionOffset: offset,
                setOverrideDropItems: false
            );
        }

        await Helper.WaitUntil(() => player.playersManager.shipIsLeaving, cancellationToken);
        player.KillPlayer();
    }
}