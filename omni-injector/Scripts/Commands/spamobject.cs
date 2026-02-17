using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;

namespace ProjectApparatus
{
    [Command("carpet")]
    sealed class CarpetCommand : ICommand
    {
        // Méthode utilitaire pour trouver un joueur par une partie de son nom
        private static PlayerControllerB GetPlayer(string name)
        {
            if (StartOfRound.Instance == null) return null;
            
            // Recherche insensible à la casse
            return StartOfRound.Instance.allPlayerScripts
                   .FirstOrDefault(p => p.playerUsername.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // La logique "Sale" du spam, exécutée à chaque frame
        static Action<float> SpamObjectsOnTarget(PlaceableShipObject shipObject, PlayerControllerB targetPlayer) => (timeElapsed) =>
        {
            // Vérifications de sécurité
            if (shipObject == null || targetPlayer == null || ShipBuildModeManager.Instance == null) return;
            if (shipObject.parentObject == null) return;

            // 1. Gestion du Stockage (Force l'objet à sortir du placard)
            if (StartOfRound.Instance.unlockablesList.unlockables[shipObject.unlockableID].inStorage)
            {
                StartOfRound.Instance.ReturnUnlockableFromStorageServerRpc(shipObject.unlockableID);
            }

            NetworkObject netObj = shipObject.parentObject.GetComponent<NetworkObject>();
            if (netObj == null) return;

            // --- SÉQUENCE DE L'EXPLOIT ---
            
            // Étape A : Placement Local
            // On place l'objet sur le joueur cible (localement) pour initialiser l'état
            ShipBuildModeManager.Instance.PlaceShipObject(
                targetPlayer.transform.position,
                targetPlayer.transform.eulerAngles, // Suit la rotation du joueur
                shipObject
            );

            // Étape B : Annulation du mode construction
            // C'est CRUCIAL. Cela "ferme" l'action de construction du côté client avant d'envoyer la requête au serveur.
            ShipBuildModeManager.Instance.CancelBuildMode(false);

            // Étape C : Envoi au Serveur (RPC)
            // On force le serveur à mettre l'objet à la position du joueur cible.
            // Note : On utilise -1 ou le ClientId local. Dans l'exploit original, c'était souvent lié à qui "possède" l'objet.
            ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(
                targetPlayer.transform.position,
                shipObject.mainMesh.transform.eulerAngles,
                netObj,
                (int)StartOfRound.Instance.localPlayerController.playerClientId // On signe avec notre ID pour valider l'action
            );
        };

        static Action<PlaceableShipObject> ActivateCarpet(ulong duration, PlayerControllerB targetPlayer) => (shipObject) =>
            Helper.CreateComponent<TransientBehaviour>()
                  .Init(CarpetCommand.SpamObjectsOnTarget(shipObject, targetPlayer), duration);

        public async Task Execute(Arguments args, CancellationToken cancellationToken)
        {
            // Vérification des arguments : carpet <joueur> <durée>
            if (args.Length < 2)
            {
                Chat.Print("Usage: carpet <player_name> <duration>");
                return;
            }

            string playerName = args[0];
            string durationStr = args[1];

            // 1. Trouver le joueur
            PlayerControllerB targetPlayer = GetPlayer(playerName);
            if (targetPlayer == null)
            {
                Chat.Print($"Error: Player '{playerName}' not found.");
                return;
            }

            // 2. Parser la durée
            if (!ulong.TryParse(durationStr, out ulong duration))
            {
                Chat.Print($"Error: Duration '{durationStr}' is not a valid number.");
                return;
            }

            Chat.Print($"Spamming carpet on {targetPlayer.playerUsername} for {duration} seconds...");

            // 3. Exécuter la boucle sur tous les objets
            Helper.FindObjects<PlaceableShipObject>()
                  .ForEach(CarpetCommand.ActivateCarpet(duration, targetPlayer));
        }
    }
}