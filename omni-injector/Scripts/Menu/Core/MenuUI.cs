using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Omni.Menu.Data;
using Omni.Menu.Util;
using Omni.Menu.Tabs; 
using Hax; // Permet d'accéder à Helper.LocalPlayer et Chat

namespace Omni.Menu.Core
{
    public class MenuUI : MonoBehaviour
    {
        public static MenuUI Instance { get; private set; }

        private bool showMenu;
        private bool cursorMode;
        private int tabIndex = 0;
        
        // Permet de savoir si on vient juste de fermer le menu pour rendre les contrôles
        private bool wasMenuOpen = false; 
        
        private List<MenuTab> tabs = new List<MenuTab>();

        private string notif;
        private float notifTimer;

        public Transform playerHead; 

        void Awake()
        {
            if (Instance != null) { Destroy(this.gameObject); return; }
            Instance = this;
            
            DontDestroyOnLoad(this.gameObject);

            tabs.Add(new TabSelf());
            tabs.Add(new TabPlayers());
            tabs.Add(new TabWorld());
            tabs.Add(new TabHost());
            tabs.Add(new TabTroll());
            tabs.Add(new TabSettings());
        }

        void Update()
        {
            // 1. Écoute des frappes clavier
            if (Keyboard.current != null)
            {
                if (Keyboard.current[MenuSettings.openKey].wasPressedThisFrame)
                    showMenu = !showMenu;

                if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
                    cursorMode = !cursorMode;
            }

            // 2. Notifications
            if (notifTimer > 0) notifTimer -= Time.deltaTime;

            // 3. Effets en jeu (SpeedHack temps, Spin)
            ApplyGlobalEffects();
        }

        // --- NOUVEAU : On utilise LateUpdate pour être sûr de forcer le jeu ---
        void LateUpdate()
        {
            bool isMenuOpen = showMenu || cursorMode;

            if (isMenuOpen)
            {
                // On force le curseur libre à CHAQUE frame
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // On fige la caméra et les clics du joueur local
                if (Helper.LocalPlayer != null)
                {
                    Helper.LocalPlayer.disableLookInput = true;
                    Helper.LocalPlayer.disableInteract = true;
                }
                
                wasMenuOpen = true;
            }
            else if (wasMenuOpen)
            {
                // Le menu vient d'être fermé : on rend instantanément le contrôle au joueur
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                if (Helper.LocalPlayer != null)
                {
                    Helper.LocalPlayer.disableLookInput = false;
                    Helper.LocalPlayer.disableInteract = false;
                }
                
                wasMenuOpen = false;
            }
        }

        private void ApplyGlobalEffects()
        {
            if (MenuSettings.timeJitter) Time.timeScale = UnityEngine.Random.Range(0.3f, 1.7f);
            else if (!MenuSettings.fakeFreeze) Time.timeScale = 1f;
            if (MenuSettings.fakeFreeze) Time.timeScale = 0f;

            if (MenuSettings.extremeHeadSpin && playerHead != null)
                playerHead.localRotation *= Quaternion.Euler(0, MenuSettings.headSpinSpeed * Time.deltaTime, 0);
        }

        void OnGUI()
        {
            UI.HandleStyles();

            // --- Éléments constants ---
            if (MenuSettings.showWatermark)
            {
                UI.DrawShadowText(new Rect(15, 15, 300, 30), "★ OMNI INJECTOR ★", UI.watermarkStyle, UI.accentColor);
            }

            if (MenuSettings.notifications && notifTimer > 0)
            {
                GUI.color = UI.accentColor;
                GUI.Box(new Rect(15, Screen.height - 65, 610, 50), "", UI.boxStyle); 
                GUI.color = Color.white;
                UI.DrawShadowText(new Rect(25, Screen.height - 60, 600, 40), notif, UI.labelStyle, UI.textColor);
            }

            // --- Affichage ---
            if (!showMenu)
            {
                GUI.backgroundColor = UI.buttonColor;
                if (GUI.Button(new Rect(Screen.width - 170, Screen.height - 50, 150, 35), $"★ OPEN ({MenuSettings.openKey}) ★", UI.buttonStyle))
                    showMenu = true;
                return;
            }

            GUI.backgroundColor = new Color(1f, 1f, 1f, MenuSettings.menuAlpha); 
            GUI.contentColor = new Color(UI.textColor.r, UI.textColor.g, UI.textColor.b, MenuSettings.menuAlpha);

            string title = "★ OMNI INJECTOR V4 ★";
            if (MenuSettings.pulseTitle) 
            {
                Color pulseCol = Color.Lerp(Color.white, UI.accentColor, Mathf.PingPong(Time.time * 2, 1));
                GUI.color = new Color(pulseCol.r, pulseCol.g, pulseCol.b, MenuSettings.menuAlpha);
            }
            else
            {
                GUI.color = new Color(1f, 1f, 1f, MenuSettings.menuAlpha);
            }
            
            MenuSettings.windowRect.width = MenuSettings.menuWidth;
            MenuSettings.windowRect.height = MenuSettings.menuHeight;
            MenuSettings.windowRect = GUI.Window(0, MenuSettings.windowRect, DrawWindow, title, UI.windowStyle);
            
            GUI.color = Color.white; 
            
            // Affichage des infobulles par-dessus
            UI.DrawTooltip();
        }

        void DrawWindow(int id)
        {
            DrawTabs();
            GUILayout.Space(15);

            if (tabs == null || tabs.Count == 0)
            {
                GUILayout.Label("Chargement des modules...", UI.labelStyle);
                GUI.DragWindow(new Rect(0, 0, MenuSettings.menuWidth, 40));
                return;
            }

            tabs[tabIndex].scrollPosition = GUILayout.BeginScrollView(tabs[tabIndex].scrollPosition, false, true);
            tabs[tabIndex].Draw();
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"FERMER ({MenuSettings.openKey})", UI.buttonStyle, GUILayout.Height(35))) showMenu = false;
            GUILayout.Space(10);
            if (GUILayout.Button("QUITTER LE JEU", UI.buttonStyle, GUILayout.Height(35))) Application.Quit();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            GUI.DragWindow(new Rect(0, 0, MenuSettings.menuWidth, 40));
        }

        void DrawTabs()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabs.Count; i++)
            {
                if (GUILayout.Button(tabs[i].TabName, tabIndex == i ? UI.activeTab : UI.tabStyle, GUILayout.Height(35)))
                {
                    tabIndex = i;
                }
            }
            GUILayout.EndHorizontal();
        }

        public void Notify(string msg)
        {
            notif = msg;
            notifTimer = 3.5f;
        }

        public static void ExecuteCommand(string commandName)
        {
            if (!commandName.StartsWith("/")) commandName = "/" + commandName;
            
            try 
            {
                global::Chat.ExecuteCommand(commandName); 
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OmniInjector] Échec de l'exécution de la commande {commandName} : {ex.Message}");
            }
        }
    }
}