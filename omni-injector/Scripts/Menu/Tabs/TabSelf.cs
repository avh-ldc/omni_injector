using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;

namespace Omni.Menu.Tabs
{
    public class TabSelf : MenuTab
    {
        private CancellationTokenSource spammToken;

        public TabSelf() : base("PERSO") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("ÉTAT DU JOUEUR");
            GUILayout.BeginVertical(UI.boxStyle);
            DrawCommandToggle(ref MenuSettings.godMode, "God Mode (Invincible)", "/god");
            DrawCommandToggle(ref MenuSettings.infiniteStamina, "Stamina Infinie", "/stamina");
            DrawCommandToggle(ref MenuSettings.noclip, "NoClip (Voler)", "/noclip");
            DrawCommandToggle(ref MenuSettings.unlimitedJump, "Sauts Infinis", "/jump");
            DrawCommandToggle(ref MenuSettings.rapidUse, "Rapid Fire (Action Rapide)", "/rapid");
            DrawCommandToggle(ref MenuSettings.invis, "Invisible", "/invis");
            DrawCommandToggle(ref MenuSettings.hearAll, "Tout Entendre (Talkie Global)", "/hear");
            DrawCommandToggle(ref MenuSettings.betaMode, "Mode Beta", "/beta");
            GUILayout.EndVertical();

            UI.DrawSectionHeader("CLICS MAGIQUES");
            GUILayout.BeginVertical(UI.boxStyle);
            DrawCommandToggle(ref MenuSettings.stunClick, "Clic Gauche = STUN", "/stunclick");
            DrawCommandToggle(ref MenuSettings.killClick, "Clic Gauche = KILL", "/killclick");
            GUILayout.EndVertical();

            UI.DrawSectionHeader("MOUVEMENT");
            GUILayout.BeginVertical(UI.boxStyle);
            GUILayout.Label($"Vitesse de course : {MenuSettings.speedHack:F1}x", UI.labelStyle);
            float newSpeed = GUILayout.HorizontalSlider(MenuSettings.speedHack, 1f, 10f);
            if (Math.Abs(newSpeed - MenuSettings.speedHack) > 0.1f) 
            { 
                MenuSettings.speedHack = newSpeed; 
                MenuUI.ExecuteCommand($"/speed {MenuSettings.speedHack:F1}"); 
            }
            GUILayout.EndVertical();

            UI.DrawSectionHeader("XP & LEVEL");
            GUILayout.BeginVertical(UI.boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Valeur XP :", UI.labelStyle, GUILayout.Width(80));
            MenuSettings.xpInput = GUILayout.TextField(MenuSettings.xpInput, UI.textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("DÉFINIR", UI.buttonStyle, GUILayout.Width(100))) 
                MenuUI.ExecuteCommand($"/xp {MenuSettings.xpInput}");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            UI.DrawSectionHeader("APPARENCE (SUITS)");
            GUILayout.BeginVertical(UI.boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Orange", UI.buttonStyle)) MenuUI.ExecuteCommand("/suit orange");
            if (GUILayout.Button("Vert", UI.buttonStyle)) MenuUI.ExecuteCommand("/suit green");
            if (GUILayout.Button("Hazmat", UI.buttonStyle)) MenuUI.ExecuteCommand("/suit hazard");
            if (GUILayout.Button("Pyjama", UI.buttonStyle)) MenuUI.ExecuteCommand("/suit pajama");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button(MenuSettings.spammSuit ? "ARRÊTER SPAM SUIT" : "LANCER SPAM SUIT", MenuSettings.spammSuit ? UI.activeTab : UI.buttonStyle))
            {
                if (MenuSettings.spammSuit) 
                { 
                    MenuSettings.spammSuit = false; 
                    spammToken?.Cancel(); 
                }
                else 
                { 
                    MenuSettings.spammSuit = true; 
                    spammToken = new CancellationTokenSource(); 
                    _ = RunSpammWear(spammToken.Token); 
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawCommandToggle(ref bool state, string label, string command)
        {
            if (UI.DrawToggle(ref state, label))
            {
                MenuUI.ExecuteCommand(command);
                if (MenuUI.Instance != null)
                    MenuUI.Instance.Notify($"{command} {(state ? "ACTIVÉ" : "DÉSACTIVÉ")}");
            }
        }

        private async Task RunSpammWear(CancellationToken token)
        {
            string[] suits = { "armor", "hazmat", "spacesuit", "riot", "stealth" };
            try
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var suit in suits)
                    {
                        if (token.IsCancellationRequested) break;
                        MenuUI.ExecuteCommand($"/suit {suit}");
                        await Task.Delay(100, token);
                    }
                }
            }
            catch (TaskCanceledException) { /* Ignoré lors de l'annulation */ }
        }
    }
}