using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("spam-kill")]
sealed class SpamKillCommand : ICommand
{
    // Maintient la référence de la boucle en cours pour pouvoir l'interrompre
    private static CancellationTokenSource activeSpamToken;
    private static string currentTargetName = string.Empty;

    private static async Task SpamDamageLoop(PlayerControllerB target, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // Vérifie que le joueur est valide et toujours en vie avant chaque itération
                if (target != null && !target.isPlayerDead)
                {
                    // Paramètres : (damageNumber, hasDamageSFX, callRPC, causeOfDeath, deathAnimation, fallDamage, force)
                    target.DamagePlayer(0, true, true, CauseOfDeath.Unknown, 0, false, default);
                }

                await Task.Delay(150, token); // 150ms prévient le spam excessif des requêtes RPC
            }
        }
        catch (TaskCanceledException) { /* Arrêt silencieux attendu */ }
        catch (Exception ex) { Chat.Print($"[Erreur Spam-Kill] {ex.Message}"); }
    }

    private static async Task SpamDamageAllLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (Helper.Players != null)
                {
                    foreach (var player in Helper.Players)
                    {
                        if (player != null && !player.isPlayerDead)
                        {
                            player.DamagePlayer(0, true, true, CauseOfDeath.Unknown, 0, false, default);
                        }
                    }
                }

                await Task.Delay(150, token);
            }
        }
        catch (TaskCanceledException) { /* Arrêt silencieux attendu */ }
        catch (Exception ex) { Chat.Print($"[Erreur Spam-Kill] {ex.Message}"); }
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        // 1. Logique de désactivation (Toggle Off)
        // Si une boucle tourne déjà, exécuter la commande à nouveau l'arrêtera, peu importe les arguments.
        if (activeSpamToken != null)
        {
            activeSpamToken.Cancel();
            activeSpamToken.Dispose();
            activeSpamToken = null;
            
            Chat.Print($"Spam-kill désactivé (était actif sur : {currentTargetName}).");
            currentTargetName = string.Empty;
            return;
        }

        // 2. Vérification des arguments
        if (args.Length == 0)
        {
            Chat.Print("Usage: /spam-kill <joueur> ou /spam-kill --all");
            return;
        }

        // 3. Logique d'activation (Toggle On)
        activeSpamToken = new CancellationTokenSource();
        currentTargetName = args[0];

        if (args[0] == "--all")
        {
            _ = SpamDamageAllLoop(activeSpamToken.Token);
            Chat.Print("Spam-kill activé sur TOUS les joueurs. Refaites la commande pour stopper.");
        }
        else
        {
            PlayerControllerB targetPlayer = Helper.GetActivePlayer(args[0]);
            
            if (targetPlayer == null)
            {
                Chat.Print($"Le joueur '{args[0]}' est introuvable ou n'est pas instancié.");
                activeSpamToken.Cancel();
                activeSpamToken.Dispose();
                activeSpamToken = null;
                return;
            }

            currentTargetName = targetPlayer.playerUsername;
            _ = SpamDamageLoop(targetPlayer, activeSpamToken.Token);
            Chat.Print($"Spam-kill activé sur {currentTargetName}. Refaites la commande pour stopper.");
        }
    }
}