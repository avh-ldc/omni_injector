using System;
using UnityEngine;
using Omni.Menu.Core;
using Omni.Menu.Data;
using Omni.Menu.Util;

namespace Omni.Menu.Tabs
{
    public class TabWorld : MenuTab
    {
        public TabWorld() : base("MONDE") { }

        public override void Draw()
        {
            UI.DrawSectionHeader("VISION");
            GUILayout.BeginVertical(UI.boxStyle);
            
            if (UI.DrawToggle(ref MenuSettings.esp, "ESP (Wallhack)"))
                MenuUI.ExecuteCommand("/esp");
            
            if (UI.DrawToggle(ref MenuSettings.brightVision, "Vision Nocturne"))
                MenuUI.ExecuteCommand("/bright");

            GUILayout.Space(10);
            GUILayout.Label($"Niveau de Luminosité : {MenuSettings.brightness:F1}", UI.labelStyle);
            float newBright = GUILayout.HorizontalSlider(MenuSettings.brightness, 0.5f, 5f);
            if (Math.Abs(newBright - MenuSettings.brightness) > 0.1f) 
            { 
                MenuSettings.brightness = newBright; 
                MenuUI.ExecuteCommand($"/brightness {MenuSettings.brightness:F1}"); 
            }
            GUILayout.EndVertical();

            UI.DrawSectionHeader("SÉCURITÉ & HACK");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("UNLOCK ALL DOORS", UI.buttonStyle)) MenuUI.ExecuteCommand("/unlock");
            GUILayout.Space(5);
            if (GUILayout.Button("LOCK ALL DOORS", UI.buttonStyle)) MenuUI.ExecuteCommand("/lock");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Garage", UI.buttonStyle)) MenuUI.ExecuteCommand("/garage");
            if (GUILayout.Button("Détruire Mines", UI.buttonStyle)) MenuUI.ExecuteCommand("/explode mine");
            if (GUILayout.Button("Détruire Tourelles", UI.buttonStyle)) MenuUI.ExecuteCommand("/explode turret");
            if (GUILayout.Button("Détruire Jetpacks", UI.buttonStyle)) MenuUI.ExecuteCommand("/explode jetpack");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Tourelles Berserk", UI.buttonStyle)) MenuUI.ExecuteCommand("/berserk");
            if (GUILayout.Button("Destroy Basic", UI.buttonStyle)) MenuUI.ExecuteCommand("/destroy");
            if (GUILayout.Button("Destroy ALL", UI.buttonStyle)) MenuUI.ExecuteCommand("/destroy --all");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            UI.DrawSectionHeader("NAVIGATION SPATIALE");
            GUILayout.BeginVertical(UI.boxStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID Lune :", UI.labelStyle, GUILayout.Width(65));
            MenuSettings.visitInput = GUILayout.TextField(MenuSettings.visitInput, UI.textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("VOYAGER", UI.buttonStyle, GUILayout.Width(100))) 
                MenuUI.ExecuteCommand($"/visit {MenuSettings.visitInput}");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Titan", UI.buttonStyle)) MenuUI.ExecuteCommand("/visit Titan");
            if (GUILayout.Button("Rend", UI.buttonStyle)) MenuUI.ExecuteCommand("/visit Rend");
            if (GUILayout.Button("Artifice", UI.buttonStyle)) MenuUI.ExecuteCommand("/visit Artifice");
            if (GUILayout.Button("Dine", UI.buttonStyle)) MenuUI.ExecuteCommand("/visit Dine");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
    }
}