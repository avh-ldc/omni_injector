using System;
using System.Threading;
using System.Threading.Tasks;

[Command("panicdoor")]
sealed class PanicDoorCommand : ICommand, IShipDoor
{
    // L'état statique survit aux changements de parties
    private static CancellationTokenSource _cts;

    // La signature demande de retourner une Task, mais nous allons la compléter immédiatement.
    public Task Execute(Arguments args, CancellationToken cancellationToken)
    {
        // 1. Toggle OFF : Arrêt de l'instance existante
        if (_cts != null)
        {
            try 
            { 
                _cts.Cancel(); 
                _cts.Dispose();
            } 
            catch { /* Sécurité contre les erreurs d'objets déjà détruits par le jeu */ }
            
            _cts = null;
            
            // On rend la main au framework immédiatement.
            return Task.CompletedTask;
        }

        // 2. Toggle ON : Démarrage d'une nouvelle instance
        _cts = new CancellationTokenSource();
        
        // On lance la boucle de manière asynchrone SANS l'attendre (pas de 'await').
        // L'assignation discard '_' indique explicitement que l'on détache cette tâche.
        _ = RunPanicLoopAsync(_cts.Token);

        // La commande se termine avec succès aux yeux du jeu instantanément.
        return Task.CompletedTask;
    }

    // Méthode dédiée et isolée pour gérer le cycle de la porte
    private async Task RunPanicLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                this.SetShipDoorState(false);
                await Task.Delay(50, token);

                this.SetShipDoorState(true);
                await Task.Delay(50, token);
            }
        }
        catch (Exception)
        {
            // Intercepte toutes les exceptions, notamment la NullReferenceException 
            // ou MissingReferenceException si this.SetShipDoorState() est appelé 
            // alors que la porte a été détruite (quand vous quittez la partie brusquement).
        }
        finally
        {
            // Nettoyage strict : on ne dispose la ressource que si ce jeton est 
            // toujours le jeton actif en cours, évitant d'interférer avec une nouvelle partie.
            if (_cts != null && _cts.Token == token)
            {
                try { _cts.Dispose(); } catch { }
                _cts = null;
            }
        }
    }
}