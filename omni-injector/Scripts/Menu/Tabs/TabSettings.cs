using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;

namespace Omni.Menu.Tabs
{
    public class TabSettings : MenuTab
    {
        public TabSettings() : base("CONFIG") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("DIMENSIONS & AFFICHAGE GLOBALES");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.Label($"Largeur de la fenêtre : {MenuSettings.menuWidth:F0}px", UI.labelStyle);
            MenuSettings.menuWidth = GUILayout.HorizontalSlider(MenuSettings.menuWidth, 500f, 1200f);
            
            GUILayout.Label($"Hauteur de la fenêtre : {MenuSettings.menuHeight:F0}px", UI.labelStyle);
            MenuSettings.menuHeight = GUILayout.HorizontalSlider(MenuSettings.menuHeight, 400f, 1000f);

            GUILayout.Space(10);
            GUILayout.Label($"Échelle globale de l'interface : {MenuSettings.uiScale:F2}", UI.labelStyle);
            MenuSettings.uiScale = GUILayout.HorizontalSlider(MenuSettings.uiScale, 0.5f, 2f);

            GUILayout.Label($"Taille de la police globale : {MenuSettings.fontSizeBase}", UI.labelStyle);
            int newFontSize = (int)GUILayout.HorizontalSlider(MenuSettings.fontSizeBase, 10, 26);
            if (newFontSize != MenuSettings.fontSizeBase)
            {
                MenuSettings.fontSizeBase = newFontSize;
                UI.RequestStyleRefresh(); 
            }
            
            GUILayout.Label($"Opacité du Menu : {MenuSettings.menuAlpha:F2}", UI.labelStyle);
            MenuSettings.menuAlpha = GUILayout.HorizontalSlider(MenuSettings.menuAlpha, 0.4f, 1f);
            GUILayout.EndVertical();

            UI.DrawSectionHeader("COMPORTEMENT ET NOTIFICATIONS");
            GUILayout.BeginVertical(UI.boxStyle);
            UI.DrawToggle(ref MenuSettings.notifications, "Afficher les Notifications Flottantes");
            UI.DrawToggle(ref MenuSettings.showWatermark, "Afficher le Filigrane (Watermark)");
            UI.DrawToggle(ref MenuSettings.pulseTitle, "Animation Pulsatile du Titre");
            GUILayout.EndVertical();

            UI.DrawSectionHeader("THÈMES DE L'INTERFACE");
            GUILayout.BeginVertical(UI.boxStyle);
            int newTheme = GUILayout.Toolbar(MenuSettings.themeIndex, new[] { "Obsidian", "Cyberpunk", "Ocean", "Crimson", "Hacker", "Midnight" }, UI.buttonStyle);
            if (newTheme != MenuSettings.themeIndex)
            {
                MenuSettings.themeIndex = newTheme;
                MenuSettings.customColorMode = false;
                MenuSettings.rainbowMode = false;
                UI.RequestStyleRefresh();
            }
            GUILayout.EndVertical();

            UI.DrawSectionHeader("COULEURS EXPERTES ET MODIFICATION");
            GUILayout.BeginVertical(UI.boxStyle);
            
            if (UI.DrawToggle(ref MenuSettings.rainbowMode, "🌈 Mode RGB Dynamique (Teintes changeantes)"))
                UI.RequestStyleRefresh();

            if (MenuSettings.rainbowMode)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Vitesse d'évolution RVB : {MenuSettings.rainbowSpeed:F1}", UI.labelStyle);
                MenuSettings.rainbowSpeed = GUILayout.HorizontalSlider(MenuSettings.rainbowSpeed, 0.1f, 3f);
            }
            else
            {
                if (UI.DrawToggle(ref MenuSettings.customColorMode, "🎨 Activer la Palette Totalement Personnalisée"))
                    UI.RequestStyleRefresh();

                if (MenuSettings.customColorMode)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur Principale (Accentuation) ---", UI.labelStyle);
                    MenuSettings.customAccentColor.r = GUILayout.HorizontalSlider(MenuSettings.customAccentColor.r, 0f, 1f);
                    MenuSettings.customAccentColor.g = GUILayout.HorizontalSlider(MenuSettings.customAccentColor.g, 0f, 1f);
                    MenuSettings.customAccentColor.b = GUILayout.HorizontalSlider(MenuSettings.customAccentColor.b, 0f, 1f);

                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur d'Arrière-Plan (Fond) ---", UI.labelStyle);
                    MenuSettings.customBgColor.r = GUILayout.HorizontalSlider(MenuSettings.customBgColor.r, 0f, 1f);
                    MenuSettings.customBgColor.g = GUILayout.HorizontalSlider(MenuSettings.customBgColor.g, 0f, 1f);
                    MenuSettings.customBgColor.b = GUILayout.HorizontalSlider(MenuSettings.customBgColor.b, 0f, 1f);

                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur du Texte Général ---", UI.labelStyle);
                    MenuSettings.customTextColor.r = GUILayout.HorizontalSlider(MenuSettings.customTextColor.r, 0f, 1f);
                    MenuSettings.customTextColor.g = GUILayout.HorizontalSlider(MenuSettings.customTextColor.g, 0f, 1f);
                    MenuSettings.customTextColor.b = GUILayout.HorizontalSlider(MenuSettings.customTextColor.b, 0f, 1f);

                    // Si on touche aux couleurs customs, on force l'actualisation
                    if (GUI.changed) UI.RequestStyleRefresh();
                }
            }
            GUILayout.EndVertical();
            
            UI.DrawSectionHeader("SÉCURITÉ RÉSEAU DE L'HÔTE");
            GUILayout.BeginVertical(UI.boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copier Lobby ID", UI.buttonStyle)) MenuUI.ExecuteCommand("/lobby");
            if (GUILayout.Button("Bloquer le Radar", UI.buttonStyle)) MenuUI.ExecuteCommand("/block radar");
            if (GUILayout.Button("Bloquer Aim Ennemi", UI.buttonStyle)) MenuUI.ExecuteCommand("/block enemy");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(20);
            if (GUILayout.Button("RÉINITIALISER TOUTE LA CONFIGURATION", UI.activeTab, GUILayout.Height(35)))
                ResetConfig();
        }

        private void ResetConfig()
        {
            // Reset des variables d'état
            MenuSettings.godMode = MenuSettings.infiniteStamina = MenuSettings.noclip = false;
            MenuSettings.unlimitedJump = MenuSettings.esp = MenuSettings.brightVision = MenuSettings.betaMode = false;
            MenuSettings.stunClick = MenuSettings.killClick = MenuSettings.invis = MenuSettings.hearAll = MenuSettings.rapidUse = false;
            MenuSettings.fakeLag = MenuSettings.invertControls = MenuSettings.headSpin = MenuSettings.cameraShake = false;
            MenuSettings.rainbowScreen = MenuSettings.uiGlitch = MenuSettings.timeJitter = MenuSettings.fakeFreeze = false;
            MenuSettings.notifSpam = MenuSettings.drunkCamera = MenuSettings.extremeHeadSpin = false;
            
            // Reset des paramètres visuels
            MenuSettings.uiScale = MenuSettings.speedHack = MenuSettings.brightness = 1f;
            MenuSettings.headSpinSpeed = 1000f;
            MenuSettings.menuAlpha = 0.98f;
            MenuSettings.menuWidth = 780f;
            MenuSettings.menuHeight = 680f;
            MenuSettings.fontSizeBase = 13;
            
            // Reset des thèmes
            MenuSettings.rainbowMode = false;
            MenuSettings.customColorMode = false;
            MenuSettings.themeIndex = 0;
            
            UI.RequestStyleRefresh();
            MenuUI.Instance?.Notify("Configuration réinitialisée à zéro.");
        }
    }
}