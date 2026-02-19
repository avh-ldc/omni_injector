using System.Collections;
using UnityEngine;

sealed class AntiKickMod : MonoBehaviour {
    void OnEnable() {
        InputListener.OnBackslashPress += this.ToggleAntiKick;
        GameListener.OnGameStart += this.OnGameStart;
        GameListener.OnGameEnd += this.OnGameEnd;
    }

    void OnDisable() {
        InputListener.OnBackslashPress -= this.ToggleAntiKick;
        GameListener.OnGameStart -= this.OnGameStart;
        GameListener.OnGameEnd -= this.OnGameEnd;
    }

    void OnGameEnd() {
        // On vérifie si on doit intervenir
        if (State.DisconnectedVoluntarily) return;
        if (!Setting.EnableAntiKick) return;

        // Étape 1 : Forcer la déconnexion pour éviter de rester bloqué dans la session (le "zombie state")
        if (Helper.GameNetworkManager != null) {
            Chat.Print("Anti-Kick: Déconnexion forcée pour éviter le bug d'écran noir...");
            Helper.GameNetworkManager.Disconnect();
        }

        // Étape 2 : Lancer la séquence de reconnexion
        _ = this.StartCoroutine(AntiKickMod.RejoinLobby());
    }

    static IEnumerator RejoinLobby() {
        // On vérifie qu'on a bien les infos du lobby précédent en mémoire
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) yield break;

        WaitForEndOfFrame waitForEndOfFrame = new();

        // Attente 1 : S'assurer que le réseau a bien coupé le lien avec le lobby actuel
        while (Helper.GameNetworkManager?.currentLobby.HasValue is true) {
            yield return waitForEndOfFrame;
        }

        // Attente 2 : Attendre que le jeu soit revenu sur la scène du Menu Principal
        while (Helper.FindObject<MenuManager>() is null) {
            yield return waitForEndOfFrame;
        }

        // Pause de sécurité : On laisse une seconde à l'API Steam pour se rafraîchir
        yield return new WaitForSeconds(1.5f);

        // Étape 3 : Tentative de reconnexion au lobby stocké
        Chat.Print($"Anti-Kick: Tentative de reconnexion au lobby de {connectedLobby.SteamId}...");
        Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
    }

    void OnGameStart() {
        if (!Setting.EnableAntiKick) return;
        if (!Setting.EnableInvisible) return;

        Chat.Clear();
        Helper.SendNotification(
            title: "Omni injector",
            body: "Tu es actuellement invisible",
            isWarning: true
        );
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("Tu peux pas mettre l'antikick in game :/");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;
        
        Chat.Print(Setting.EnableAntiKick ? "Anti-Kick: ENABLED" : "Anti-Kick: DISABLED");
    }
}