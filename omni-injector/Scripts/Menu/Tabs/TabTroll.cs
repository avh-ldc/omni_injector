using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;
// using Hax; // Décommentez pour accéder à Helper.Players

namespace Omni.Menu.Tabs
{
    public class TabTroll : MenuTab
    {
        public TabTroll() : base("TROLL") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("CHAT & MESSAGES");
            GUILayout.BeginVertical(UI.boxStyle);
            MenuSettings.chatInput = GUILayout.TextField(MenuSettings.chatInput, UI.textFieldStyle);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PARLER EN TANT QUE HOST", UI.buttonStyle)) 
                MenuUI.ExecuteCommand($"/say Host {MenuSettings.chatInput}");
            GUILayout.Space(5);
            if (GUILayout.Button("SIGNAL TERMINAL", UI.buttonStyle)) 
                MenuUI.ExecuteCommand($"/signal {MenuSettings.chatInput}");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button("NETTOYER LE CHAT", UI.buttonStyle)) MenuUI.ExecuteCommand("/clear");
            GUILayout.EndVertical();

            UI.DrawSectionHeader("ACTIONS TROLL GLOBALES (TOUS)");
            GUILayout.BeginVertical(UI.boxStyle);
            if (GUILayout.Button("SPAM-KILL TOUS LES JOUEURS", UI.buttonStyle, GUILayout.Height(30))) 
                MenuUI.ExecuteCommand("/spam-kill --all");
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Durée de bruit (s) :", UI.labelStyle, GUILayout.Width(120));
            MenuSettings.noiseDuration = GUILayout.TextField(MenuSettings.noiseDuration, UI.textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("SPAMMER LE BRUIT", UI.buttonStyle)) 
            {
                // NOTE: Nécessite que la classe Helper soit accessible
                /* if (Helper.Players != null) 
                    foreach(var p in Helper.Players) 
                        MenuUI.ExecuteCommand($"/noise {p.playerUsername} {MenuSettings.noiseDuration}"); */
                MenuUI.Instance?.Notify("Fonction Spam Bruit en attente de Helper.Players");
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Durée Rotation (s) :", UI.labelStyle, GUILayout.Width(120));
            MenuSettings.spinInput = GUILayout.TextField(MenuSettings.spinInput, UI.textFieldStyle, GUILayout.Width(80));
            GUILayout.Space(10);
            if (GUILayout.Button("FAST SPIN ", UI.buttonStyle)) MenuUI.ExecuteCommand($"/fastspin {MenuSettings.spinInput}");
            if (GUILayout.Button("SPIN (10s)", UI.buttonStyle)) MenuUI.ExecuteCommand("/spin 10");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("STUN TOUT LE MONDE (5s)", UI.buttonStyle)) MenuUI.ExecuteCommand("/stun 5");
            if (GUILayout.Button("FERMER PORTES D'URGENCE", UI.buttonStyle)) MenuUI.ExecuteCommand("/panicdoor");
            if (GUILayout.Button("PROVOQUER UN CRASH", UI.buttonStyle)) MenuUI.ExecuteCommand("/crash");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            UI.DrawSectionHeader("EFFETS VISUELS CLIENT (VOUS SEUL)");
            GUILayout.BeginVertical(UI.boxStyle);
            GUILayout.BeginHorizontal();
            UI.DrawToggle(ref MenuSettings.fakeLag, "Fake Lag");
            UI.DrawToggle(ref MenuSettings.headSpin, "Head Spin");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            UI.DrawToggle(ref MenuSettings.rainbowScreen, "Rainbow Screen");
            UI.DrawToggle(ref MenuSettings.uiGlitch, "UI Glitch");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label($"Vitesse Rotation Extrême : {MenuSettings.headSpinSpeed:F0}", UI.labelStyle);
            MenuSettings.headSpinSpeed = GUILayout.HorizontalSlider(MenuSettings.headSpinSpeed, 100f, 5000f);
            
            GUILayout.Space(5);
            if (GUILayout.Button(MenuSettings.extremeHeadSpin ? "STOP ROTATION EXTRÊME" : "START ROTATION EXTRÊME", MenuSettings.extremeHeadSpin ? UI.activeTab : UI.buttonStyle))
            {
                MenuSettings.extremeHeadSpin = !MenuSettings.extremeHeadSpin;
                MenuUI.Instance?.Notify(MenuSettings.extremeHeadSpin ? "SPIN ACTIVÉ !" : "Spin arrêté.");
            }
            GUILayout.EndVertical();
        }
    }
}