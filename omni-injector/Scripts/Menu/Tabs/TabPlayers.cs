using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;
// using Hax; // Assurez-vous d'importer le namespace contenant votre classe Helper

namespace Omni.Menu.Tabs
{
    public class TabPlayers : MenuTab
    {
        public TabPlayers() : base("JOUEURS") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("CONFIG PARAMÈTRES JOUEURS");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Poison Dégâts:", UI.labelStyle, GUILayout.Width(95));
            MenuSettings.poisonDmg = GUILayout.TextField(MenuSettings.poisonDmg, UI.textFieldStyle);
            GUILayout.Space(10);
            GUILayout.Label("Durée(s):", UI.labelStyle, GUILayout.Width(60));
            MenuSettings.poisonDur = GUILayout.TextField(MenuSettings.poisonDur, UI.textFieldStyle);
            GUILayout.Space(10);
            GUILayout.Label("Délai(s):", UI.labelStyle, GUILayout.Width(55));
            MenuSettings.poisonDelay = GUILayout.TextField(MenuSettings.poisonDelay, UI.textFieldStyle);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Quantité Mask:", UI.labelStyle, GUILayout.Width(95));
            MenuSettings.maskAmount = GUILayout.TextField(MenuSettings.maskAmount, UI.textFieldStyle);
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            UI.DrawSectionHeader("JOUEURS EN LIGNE");
            
            // NOTE : Helper.LocalPlayer et Helper.Players doivent être accessibles.
            // S'ils provoquent une erreur, vérifiez leurs namespaces respectifs.
            /* string selfName = Helper.LocalPlayer != null ? Helper.LocalPlayer.playerUsername : "";
            var players = Helper.Players; 

            if (players == null || players.Length == 0) 
            { 
                GUILayout.Label("Aucun joueur détecté.", UI.labelStyle); 
                return; 
            }

            foreach (var player in players)
            {
                if (player == null) continue;
                string playerName = player.playerUsername;
                bool isMe = player == Helper.LocalPlayer;
                
                GUILayout.BeginVertical(UI.boxStyle);
                GUILayout.BeginHorizontal();
                UI.DrawShadowText(GUILayoutUtility.GetRect(200, 25), $"{(isMe ? "👤" : "🌐")} {playerName}", UI.headerStyle, isMe ? UI.accentColor : UI.textColor);
                GUILayout.FlexibleSpace();
                
                if (!isMe)
                {
                    if (GUILayout.Button("TP A LUI", UI.buttonStyle, GUILayout.Width(80))) 
                        MenuUI.ExecuteCommand($"/tp {playerName}");
                    GUILayout.Space(5);
                    if (GUILayout.Button("TP ICI", UI.buttonStyle, GUILayout.Width(80))) 
                    { 
                        if (!string.IsNullOrEmpty(selfName)) MenuUI.ExecuteCommand($"/tp {playerName} {selfName}"); 
                    }
                }
                GUILayout.EndHorizontal();

                if (!isMe)
                {
                    GUILayout.Space(10);
                    
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("KILL", UI.buttonStyle)) MenuUI.ExecuteCommand($"/kill {playerName}");
                    if (GUILayout.Button("SPAM-KILL", UI.buttonStyle)) MenuUI.ExecuteCommand($"/spam-kill {playerName}");
                    if (GUILayout.Button("BOMB", UI.buttonStyle)) MenuUI.ExecuteCommand($"/bomb {playerName}");
                    if (GUILayout.Button("VOID", UI.buttonStyle)) MenuUI.ExecuteCommand($"/void {playerName}");
                    if (GUILayout.Button("MASK", UI.buttonStyle)) MenuUI.ExecuteCommand($"/mask {MenuSettings.maskAmount} {playerName}");
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(2);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("CARPET", UI.buttonStyle)) MenuUI.ExecuteCommand($"/carpet {playerName}");
                    if (GUILayout.Button("JAIL", UI.buttonStyle)) MenuUI.ExecuteCommand($"/jail {playerName}");
                    if (GUILayout.Button("HEAL", UI.buttonStyle)) MenuUI.ExecuteCommand($"/heal {playerName}");
                    if (GUILayout.Button("POISON", UI.buttonStyle)) MenuUI.ExecuteCommand($"/poison {playerName} {MenuSettings.poisonDmg} {MenuSettings.poisonDur} {MenuSettings.poisonDelay}");
                    if (GUILayout.Button("RANDOM", UI.buttonStyle)) MenuUI.ExecuteCommand($"/random {playerName}");
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(2);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("FATALITY (Giant)", UI.buttonStyle)) MenuUI.ExecuteCommand($"/fatality {playerName} ForestGiant");
                    if (GUILayout.Button("FATALITY (Jester)", UI.buttonStyle)) MenuUI.ExecuteCommand($"/fatality {playerName} Jester");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            */
            
            // Fallback d'affichage temporaire si Helper n'est pas encore lié :
            GUILayout.Label("En attente de la liaison avec Helper.Players...", UI.labelStyle);
        }
    }
}