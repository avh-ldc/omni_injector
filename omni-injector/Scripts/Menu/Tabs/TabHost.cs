using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;

namespace Omni.Menu.Tabs
{
    public class TabHost : MenuTab
    {
        public TabHost() : base("HOST") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("CONTRÔLE DU VAISSEAU");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("START PARTIE", UI.buttonStyle)) MenuUI.ExecuteCommand("/start");
            if (GUILayout.Button("LAND (Atterrir)", UI.buttonStyle)) MenuUI.ExecuteCommand("/land");
            if (GUILayout.Button("ORBIT (Décoller)", UI.buttonStyle)) MenuUI.ExecuteCommand("/end");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("REVIVE ALL", UI.buttonStyle)) MenuUI.ExecuteCommand("/revive");
            if (GUILayout.Button("GODS ALL", UI.buttonStyle)) MenuUI.ExecuteCommand("/gods");
            if (GUILayout.Button("EJECT ALL", UI.buttonStyle)) MenuUI.ExecuteCommand("/eject");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Lumières ON/OFF", UI.buttonStyle)) MenuUI.ExecuteCommand("/light");
            if (GUILayout.Button("Mode Disco", UI.buttonStyle)) MenuUI.ExecuteCommand("/disco");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            UI.DrawSectionHeader("SPAWNER (REQUIS HOST)");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entité :", UI.labelStyle, GUILayout.Width(60));
            MenuSettings.enemyToSpawn = GUILayout.TextField(MenuSettings.enemyToSpawn, UI.textFieldStyle);
            GUILayout.Space(5);
            GUILayout.Label("Qté :", UI.labelStyle, GUILayout.Width(40));
            MenuSettings.spawnAmount = GUILayout.TextField(MenuSettings.spawnAmount, UI.textFieldStyle, GUILayout.Width(50));
            GUILayout.Space(10);
            if (GUILayout.Button("FAIRE SPAWN", UI.buttonStyle, GUILayout.Width(120))) 
                MenuUI.ExecuteCommand($"/spawn {MenuSettings.enemyToSpawn} me {MenuSettings.spawnAmount}");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            UI.DrawSectionHeader("GÉNÉRATION & CONTRÔLE D'OBJETS");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Teleporter", UI.buttonStyle)) MenuUI.ExecuteCommand("/build teleporter");
            if (GUILayout.Button("Inverse TP", UI.buttonStyle)) MenuUI.ExecuteCommand("/build inverse");
            if (GUILayout.Button("Terminal", UI.buttonStyle)) MenuUI.ExecuteCommand("/build terminal");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button("GRAB TOUS LES OBJETS (MAP)", UI.buttonStyle)) MenuUI.ExecuteCommand("/grab");
            
            GUILayout.EndVertical();

            UI.DrawSectionHeader("FINANCES DU GROUPE");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Crédits :", UI.labelStyle, GUILayout.Width(65));
            MenuSettings.moneyInput = GUILayout.TextField(MenuSettings.moneyInput, UI.textFieldStyle);
            if (GUILayout.Button("INJECTER", UI.buttonStyle, GUILayout.Width(100))) 
                MenuUI.ExecuteCommand($"/credit {MenuSettings.moneyInput}");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Quota :", UI.labelStyle, GUILayout.Width(65));
            MenuSettings.quotaInput = GUILayout.TextField(MenuSettings.quotaInput, UI.textFieldStyle);
            if (GUILayout.Button("DÉFINIR", UI.buttonStyle, GUILayout.Width(100))) 
                MenuUI.ExecuteCommand($"/quota {MenuSettings.quotaInput}");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            UI.DrawSectionHeader("BUREAU D'ACHAT & VENTE");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item :", UI.labelStyle, GUILayout.Width(50));
            MenuSettings.buyInput = GUILayout.TextField(MenuSettings.buyInput, UI.textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("ACHETER", UI.buttonStyle, GUILayout.Width(100))) 
                MenuUI.ExecuteCommand($"/buy {MenuSettings.buyInput} 1");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pelle", UI.buttonStyle)) MenuUI.ExecuteCommand("/buy shovel");
            if (GUILayout.Button("Lampe", UI.buttonStyle)) MenuUI.ExecuteCommand("/buy pro");
            if (GUILayout.Button("Zap", UI.buttonStyle)) MenuUI.ExecuteCommand("/buy zap");
            if (GUILayout.Button("Shotgun", UI.buttonStyle)) MenuUI.ExecuteCommand("/buy shotgun");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);
            
            if (GUILayout.Button("VENDRE TOUS LES OBJETS (BUREAU)", UI.buttonStyle, GUILayout.Height(30))) 
                MenuUI.ExecuteCommand("/sell");
                
            GUILayout.EndVertical();
        }
    }
}