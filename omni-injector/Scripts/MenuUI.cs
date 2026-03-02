using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax
{
    public class MenuUI : MonoBehaviour
    {
        // ===== FENETRE & STYLE =====
        private bool showMenu;
        private bool cursorMode;
        private Rect windowRect = new Rect(50, 50, 780, 680);
        private int tabIndex;
        private Vector2 scrollPosition;

        // --- PERSONNALISATION VISUELLE PREMIUM ---
        private float menuWidth = 780f;
        private float menuHeight = 680f;
        private float menuAlpha = 0.98f; 
        private float uiScale = 1f;
        private int fontSizeBase = 13;

        private bool rainbowMode;
        private float rainbowSpeed = 0.5f;
        private bool customColorMode; 
        
        // Palette de couleurs
        private Color customAccentColor = new Color(0, 0.8f, 1f);
        private Color customBgColor = new Color(0.08f, 0.08f, 0.1f);
        private Color customTextColor = new Color(0.9f, 0.9f, 0.9f);
        private Color borderColor; // Calculée dynamiquement

        private bool showWatermark = true;
        private bool pulseTitle = false; 
        
        // ===== ETATS =====
        private bool godMode, infiniteStamina, noclip, unlimitedJump;
        private bool stunClick, killClick, invis, hearAll, rapidUse;
        private bool esp, brightVision;
        private bool betaMode; // NOUVEAU: Beta Mode
        private float speedHack = 1f;
        private float brightness = 1f;
        
        // ===== INPUTS =====
        private string xpInput = "1000";
        private string visitInput = "Titan";
        private string moneyInput = "5000";
        private string quotaInput = "2000";
        private string buyInput = "shovel";
        private string buyQty = "1";
        private string chatInput = "LDC ON TOP !";
        private string noiseDuration = "30";
        private string poisonDmg = "15";
        private string poisonDur = "30";
        private string poisonDelay = "2";
        private string maskAmount = "1"; // NOUVEAU: Quantité de masques
        private string spinInput = "10";
        private string enemyToSpawn = "Girl";
        private string spawnAmount = "1";

        // ===== TROLL & FX =====
        private bool fakeLag, invertControls, headSpin;
        private bool cameraShake, fovPulse, rainbowScreen;
        private bool timeJitter, fakeFreeze, notifSpam;
        private bool drunkCamera, uiGlitch;
        private bool extremeHeadSpin;
        private float headSpinSpeed = 1000f;

        // ===== SYSTEME =====
        private bool spammSuit;
        private CancellationTokenSource spammToken;
        private bool notifications = true;
        private int themeIndex;
        private Key openKey = Key.Insert;

        // ===== STYLES =====
        private Color bgColor, buttonColor, textColor, accentColor;
        private GUIStyle tabStyle, activeTab, windowStyle, labelStyle, headerStyle, boxStyle, buttonStyle, textFieldStyle, watermarkStyle;
        private GUIStyle toggleBtnOn, toggleBtnOff;
        private bool stylesInitialized = false;
        
        // ===== NOTIFICATIONS =====
        private string notif;
        private float notifTimer;

        // ===== REFS =====
        public Transform playerHead; 

        void Update()
        {
            if (Keyboard.current[openKey].wasPressedThisFrame)
                ToggleMenu();

            if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
            {
                cursorMode = !cursorMode;
                UpdateCursorState();
            }

            if (notifTimer > 0) notifTimer -= Time.deltaTime;

            if (rainbowMode)
            {
                customAccentColor = Color.HSVToRGB((Time.time * rainbowSpeed) % 1f, 0.8f, 1f);
                stylesInitialized = false; // Force la mise à jour des bordures
            }

            if (timeJitter) Time.timeScale = UnityEngine.Random.Range(0.3f, 1.7f);
            else if (!fakeFreeze) Time.timeScale = 1f;
            if (fakeFreeze) Time.timeScale = 0f;

            if (extremeHeadSpin && playerHead != null)
                playerHead.localRotation *= Quaternion.Euler(0, headSpinSpeed * Time.deltaTime, 0);
            
            if (!spammSuit && spammToken != null) { spammToken.Cancel(); spammToken = null; }

            windowRect.width = menuWidth;
            windowRect.height = menuHeight;
        }

        void ToggleMenu()
        {
            showMenu = !showMenu;
            UpdateCursorState();
        }

        void UpdateCursorState()
        {
            bool shouldShow = showMenu || cursorMode;
            Cursor.visible = shouldShow;
            Cursor.lockState = shouldShow ? CursorLockMode.None : CursorLockMode.Locked;
        }

        void OnGUI()
        {
            ApplyTheme();
            InitStyles();

            if (showWatermark)
            {
                DrawShadowText(new Rect(15, 15, 300, 30), "★ OMNI INJECTOR ★", watermarkStyle, accentColor);
            }

            if (notifications && notifTimer > 0)
            {
                GUI.color = accentColor;
                GUI.Box(new Rect(15, Screen.height - 65, 610, 50), "", boxStyle); 
                GUI.color = Color.white;
                DrawShadowText(new Rect(25, Screen.height - 60, 600, 40), notif, labelStyle, textColor);
            }

            if (!showMenu)
            {
                GUI.backgroundColor = buttonColor;
                if (GUI.Button(new Rect(Screen.width - 170, Screen.height - 50, 150, 35), "★ OPEN (INS) ★", buttonStyle))
                    ToggleMenu();
                return;
            }

            float glitchScale = uiGlitch ? UnityEngine.Random.Range(0.99f, 1.01f) : 1f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * uiScale * glitchScale);

            GUI.backgroundColor = Color.white; 
            GUI.contentColor = textColor;

            string title = "★ OMNI INJECTOR V4 ★";
            if (pulseTitle) GUI.color = Color.Lerp(Color.white, accentColor, Mathf.PingPong(Time.time * 2, 1));
            
            windowRect = GUI.Window(0, windowRect, DrawWindow, title, windowStyle);
            GUI.color = Color.white; 
        }

        void DrawWindow(int id)
        {
            DrawTabs();
            GUILayout.Space(15);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            switch (tabIndex)
            {
                case 0: DrawSelf(); break;
                case 1: DrawRealTimePlayers(); break;
                case 2: DrawWorld(); break;
                case 3: DrawGameAndItems(); break;
                case 4: DrawTrollAndFX(); break;
                case 5: DrawSettings(); break;
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("FERMER (INS)", buttonStyle, GUILayout.Height(35))) ToggleMenu();
            GUILayout.Space(10);
            if (GUILayout.Button("QUITTER LE JEU", buttonStyle, GUILayout.Height(35))) Application.Quit();
            
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            GUI.DragWindow(new Rect(0, 0, 10000, 30));
        }

        void DrawTabs()
        {
            GUILayout.BeginHorizontal();
            DrawTab("PERSO", 0);
            DrawTab("JOUEURS", 1);
            DrawTab("MONDE", 2);
            DrawTab("HOST", 3);
            DrawTab("TROLL", 4);
            DrawTab("CONFIG", 5);
            GUILayout.EndHorizontal();
        }

        void DrawTab(string name, int id)
        {
            if (GUILayout.Button(name, tabIndex == id ? activeTab : tabStyle, GUILayout.Height(35)))
            {
                tabIndex = id;
                scrollPosition = Vector2.zero;
            }
        }

        // ==========================================
        // CONTENU ONGLETS
        // ==========================================

        void DrawSelf()
        {
            DrawSectionHeader("ÉTAT DU JOUEUR");
            GUILayout.BeginVertical(boxStyle);
            DrawToggle(ref godMode, "God Mode (Invincible)", "/god");
            DrawToggle(ref infiniteStamina, "Stamina Infinie", "/stamina");
            DrawToggle(ref noclip, "NoClip (Voler)", "/noclip");
            DrawToggle(ref unlimitedJump, "Sauts Infinis", "/jump");
            DrawToggle(ref rapidUse, "Rapid Fire (Action Rapide)", "/rapid");
            DrawToggle(ref invis, "Invisible", "/invis");
            DrawToggle(ref hearAll, "Tout Entendre (Talkie Global)", "/hear");
            DrawToggle(ref betaMode, "Mode Beta", "/beta"); // NOUVEAU
            GUILayout.EndVertical();

            DrawSectionHeader("CLICS MAGIQUES");
            GUILayout.BeginVertical(boxStyle);
            DrawToggle(ref stunClick, "Clic Gauche = STUN", "/stunclick");
            DrawToggle(ref killClick, "Clic Gauche = KILL", "/killclick");
            GUILayout.EndVertical();

            DrawSectionHeader("MOUVEMENT");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label($"Vitesse de course : {speedHack:F1}x", labelStyle);
            float newSpeed = GUILayout.HorizontalSlider(speedHack, 1f, 10f);
            if (Math.Abs(newSpeed - speedHack) > 0.1f) { speedHack = newSpeed; ExecuteCommand($"/speed {speedHack:F1}"); }
            GUILayout.EndVertical();

            DrawSectionHeader("XP & LEVEL");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Valeur XP :", labelStyle, GUILayout.Width(80));
            xpInput = GUILayout.TextField(xpInput, textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("DÉFINIR", buttonStyle, GUILayout.Width(100))) ExecuteCommand($"/xp {xpInput}");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("APPARENCE (SUITS)");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Orange", buttonStyle)) ExecuteCommand("/suit orange");
            if (GUILayout.Button("Vert", buttonStyle)) ExecuteCommand("/suit green");
            if (GUILayout.Button("Hazmat", buttonStyle)) ExecuteCommand("/suit hazard");
            if (GUILayout.Button("Pyjama", buttonStyle)) ExecuteCommand("/suit pajama");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button(spammSuit ? "ARRÊTER SPAM SUIT" : "LANCER SPAM SUIT", spammSuit ? activeTab : buttonStyle))
            {
                if (spammSuit) { spammSuit = false; spammToken?.Cancel(); }
                else { spammSuit = true; spammToken = new CancellationTokenSource(); RunSpammWear(spammToken.Token); }
            }
            GUILayout.EndVertical();
        }

        void DrawRealTimePlayers()
        {
            DrawSectionHeader("CONFIG PARAMÈTRES JOUEURS");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Poison Dégâts:", labelStyle, GUILayout.Width(95));
            poisonDmg = GUILayout.TextField(poisonDmg, textFieldStyle);
            GUILayout.Space(10);
            GUILayout.Label("Durée(s):", labelStyle, GUILayout.Width(60));
            poisonDur = GUILayout.TextField(poisonDur, textFieldStyle);
            GUILayout.Space(10);
            GUILayout.Label("Délai(s):", labelStyle, GUILayout.Width(55));
            poisonDelay = GUILayout.TextField(poisonDelay, textFieldStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Quantité Mask:", labelStyle, GUILayout.Width(95));
            maskAmount = GUILayout.TextField(maskAmount, textFieldStyle); // NOUVEAU
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("JOUEURS EN LIGNE");
            string selfName = Helper.LocalPlayer != null ? Helper.LocalPlayer.playerUsername : "";
            var players = Helper.Players; 

            if (players == null || players.Length == 0) 
            { 
                GUILayout.Label("Aucun joueur détecté.", labelStyle); 
                return; 
            }

            foreach (var player in players)
            {
                if (player == null) continue;
                string playerName = player.playerUsername;
                bool isMe = player == Helper.LocalPlayer;
                
                GUILayout.BeginVertical(boxStyle);
                GUILayout.BeginHorizontal();
                DrawShadowText(GUILayoutUtility.GetRect(200, 25), $"{(isMe ? "👤" : "🌐")} {playerName}", headerStyle, isMe ? accentColor : textColor);
                GUILayout.FlexibleSpace();
                if (!isMe)
                {
                    if (GUILayout.Button("TP A LUI", buttonStyle, GUILayout.Width(80))) ExecuteCommand($"/tp {playerName}");
                    GUILayout.Space(5);
                    if (GUILayout.Button("TP ICI", buttonStyle, GUILayout.Width(80))) { if (!string.IsNullOrEmpty(selfName)) ExecuteCommand($"/tp {playerName} {selfName}"); }
                }
                GUILayout.EndHorizontal();

                if (!isMe)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("KILL", buttonStyle)) ExecuteCommand($"/kill {playerName}");
                    if (GUILayout.Button("SPAM-KILL", buttonStyle)) ExecuteCommand($"/spam-kill {playerName}"); // NOUVEAU
                    if (GUILayout.Button("BOMB", buttonStyle)) ExecuteCommand($"/bomb {playerName}");
                    if (GUILayout.Button("VOID", buttonStyle)) ExecuteCommand($"/void {playerName}");
                    if (GUILayout.Button("MASK", buttonStyle)) ExecuteCommand($"/mask {maskAmount} {playerName}"); // MODIFIÉ
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("CARPET", buttonStyle)) ExecuteCommand($"/carpet {playerName}");
                    if (GUILayout.Button("JAIL", buttonStyle)) ExecuteCommand($"/jail {playerName}");
                    if (GUILayout.Button("HEAL", buttonStyle)) ExecuteCommand($"/heal {playerName}");
                    if (GUILayout.Button("POISON", buttonStyle)) ExecuteCommand($"/poison {playerName} {poisonDmg} {poisonDur} {poisonDelay}");
                    if (GUILayout.Button("RANDOM", buttonStyle)) ExecuteCommand($"/random {playerName}");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("FATALITY (Giant)", buttonStyle)) ExecuteCommand($"/fatality {playerName} ForestGiant");
                    if (GUILayout.Button("FATALITY (Jester)", buttonStyle)) ExecuteCommand($"/fatality {playerName} Jester");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }

        void DrawWorld()
        {
            DrawSectionHeader("VISION");
            GUILayout.BeginVertical(boxStyle);
            DrawToggle(ref esp, "ESP (Wallhack)", "/esp");
            DrawToggle(ref brightVision, "Vision Nocturne", "/bright");
            GUILayout.Space(10);
            GUILayout.Label($"Niveau de Luminosité : {brightness:F1}", labelStyle);
            float newBright = GUILayout.HorizontalSlider(brightness, 0.5f, 5f);
            if (Math.Abs(newBright - brightness) > 0.1f) { brightness = newBright; ExecuteCommand($"/brightness {brightness:F1}"); }
            GUILayout.EndVertical();

            DrawSectionHeader("SÉCURITÉ & HACK");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("UNLOCK ALL DOORS", buttonStyle)) ExecuteCommand("/unlock");
            GUILayout.Space(5);
            if (GUILayout.Button("LOCK ALL DOORS", buttonStyle)) ExecuteCommand("/lock");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Garage", buttonStyle)) ExecuteCommand("/garage");
            if (GUILayout.Button("Détruire Mines", buttonStyle)) ExecuteCommand("/explode mine");
            if (GUILayout.Button("Détruire Tourelles", buttonStyle)) ExecuteCommand("/explode turret");
            if (GUILayout.Button("Détruire Jetpacks", buttonStyle)) ExecuteCommand("/explode jetpack"); // NOUVEAU
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Tourelles Berserk", buttonStyle)) ExecuteCommand("/berserk");
            if (GUILayout.Button("Destroy Basic", buttonStyle)) ExecuteCommand("/destroy"); // NOUVEAU
            if (GUILayout.Button("Destroy ALL", buttonStyle)) ExecuteCommand("/destroy --all"); // NOUVEAU
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("NAVIGATION SPATIALE");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID Lune :", labelStyle, GUILayout.Width(65));
            visitInput = GUILayout.TextField(visitInput, textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("VOYAGER", buttonStyle, GUILayout.Width(100))) ExecuteCommand($"/visit {visitInput}");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Titan", buttonStyle)) ExecuteCommand("/visit Titan");
            if (GUILayout.Button("Rend", buttonStyle)) ExecuteCommand("/visit Rend");
            if (GUILayout.Button("Artifice", buttonStyle)) ExecuteCommand("/visit Artifice");
            if (GUILayout.Button("Dine", buttonStyle)) ExecuteCommand("/visit Dine");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void DrawGameAndItems()
        {
            DrawSectionHeader("CONTRÔLE DU VAISSEAU");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("START PARTIE", buttonStyle)) ExecuteCommand("/start");
            if (GUILayout.Button("LAND (Atterrir)", buttonStyle)) ExecuteCommand("/land");
            if (GUILayout.Button("ORBIT (Décoller)", buttonStyle)) ExecuteCommand("/end");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("REVIVE ALL", buttonStyle)) ExecuteCommand("/revive");
            if (GUILayout.Button("GODS ALL", buttonStyle)) ExecuteCommand("/gods");
            if (GUILayout.Button("EJECT ALL", buttonStyle)) ExecuteCommand("/eject");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Lumières ON/OFF", buttonStyle)) ExecuteCommand("/light"); // NOUVEAU
            if (GUILayout.Button("Mode Disco", buttonStyle)) ExecuteCommand("/disco"); // NOUVEAU
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            DrawSectionHeader("SPAWNER (REQUIS HOST)");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entité :", labelStyle, GUILayout.Width(60));
            enemyToSpawn = GUILayout.TextField(enemyToSpawn, textFieldStyle);
            GUILayout.Space(5);
            GUILayout.Label("Qté :", labelStyle, GUILayout.Width(40));
            spawnAmount = GUILayout.TextField(spawnAmount, textFieldStyle, GUILayout.Width(50));
            GUILayout.Space(10);
            if (GUILayout.Button("FAIRE SPAWN", buttonStyle, GUILayout.Width(120))) ExecuteCommand($"/spawn {enemyToSpawn} me {spawnAmount}");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("GÉNÉRATION & CONTRÔLE D'OBJETS");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Teleporter", buttonStyle)) ExecuteCommand("/build teleporter");
            if (GUILayout.Button("Inverse TP", buttonStyle)) ExecuteCommand("/build inverse");
            if (GUILayout.Button("Terminal", buttonStyle)) ExecuteCommand("/build terminal");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("GRAB TOUS LES OBJETS (MAP)", buttonStyle)) ExecuteCommand("/grab"); // NOUVEAU
            GUILayout.EndVertical();

            DrawSectionHeader("FINANCES DU GROUPE");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Crédits :", labelStyle, GUILayout.Width(65));
            moneyInput = GUILayout.TextField(moneyInput, textFieldStyle);
            if (GUILayout.Button("INJECTER", buttonStyle, GUILayout.Width(100))) ExecuteCommand($"/credit {moneyInput}");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Quota :", labelStyle, GUILayout.Width(65));
            quotaInput = GUILayout.TextField(quotaInput, textFieldStyle);
            if (GUILayout.Button("DÉFINIR", buttonStyle, GUILayout.Width(100))) ExecuteCommand($"/quota {quotaInput}");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("BUREAU D'ACHAT & VENTE");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item :", labelStyle, GUILayout.Width(50));
            buyInput = GUILayout.TextField(buyInput, textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("ACHETER", buttonStyle, GUILayout.Width(100))) ExecuteCommand($"/buy {buyInput} 1");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pelle", buttonStyle)) ExecuteCommand("/buy shovel");
            if (GUILayout.Button("Lampe", buttonStyle)) ExecuteCommand("/buy pro");
            if (GUILayout.Button("Zap", buttonStyle)) ExecuteCommand("/buy zap");
            if (GUILayout.Button("Shotgun", buttonStyle)) ExecuteCommand("/buy shotgun");
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("VENDRE TOUS LES OBJETS (BUREAU)", buttonStyle, GUILayout.Height(30))) ExecuteCommand("/sell");
            GUILayout.EndVertical();
        }

        void DrawTrollAndFX()
        {
            DrawSectionHeader("CHAT & MESSAGES");
            GUILayout.BeginVertical(boxStyle);
            chatInput = GUILayout.TextField(chatInput, textFieldStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PARLER EN TANT QUE HOST", buttonStyle)) ExecuteCommand($"/say Host {chatInput}");
            GUILayout.Space(5);
            if (GUILayout.Button("SIGNAL TERMINAL", buttonStyle)) ExecuteCommand($"/signal {chatInput}");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("NETTOYER LE CHAT", buttonStyle)) ExecuteCommand("/clear");
            GUILayout.EndVertical();

            DrawSectionHeader("ACTIONS TROLL GLOBALES (TOUS)"); // Modifié pour inclure Spam-Kill
            GUILayout.BeginVertical(boxStyle);
            if (GUILayout.Button("SPAM-KILL TOUS LES JOUEURS", buttonStyle, GUILayout.Height(30))) ExecuteCommand("/spam-kill --all"); // NOUVEAU
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Durée de bruit (s) :", labelStyle, GUILayout.Width(120));
            noiseDuration = GUILayout.TextField(noiseDuration, textFieldStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("SPAMMER LE BRUIT", buttonStyle)) 
            {
                if (Helper.Players != null) 
                    foreach(var p in Helper.Players) ExecuteCommand($"/noise {p.playerUsername} {noiseDuration}");
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Durée Rotation (s) :", labelStyle, GUILayout.Width(120));
            spinInput = GUILayout.TextField(spinInput, textFieldStyle, GUILayout.Width(80));
            GUILayout.Space(10);
            if (GUILayout.Button("FAST SPIN ", buttonStyle)) ExecuteCommand($"/fastspin {spinInput}");
            if (GUILayout.Button("SPIN (10s)", buttonStyle)) ExecuteCommand("/spin 10");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("STUN TOUT LE MONDE (5s)", buttonStyle)) ExecuteCommand("/stun 5");
            if (GUILayout.Button("FERMER PORTES D'URGENCE", buttonStyle)) ExecuteCommand("/panicdoor");
            if (GUILayout.Button("PROVOQUER UN CRASH", buttonStyle)) ExecuteCommand("/crash");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawSectionHeader("EFFETS VISUELS CLIENT (VOUS SEUL)");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            DrawToggle(ref fakeLag, "Fake Lag", "");
            DrawToggle(ref headSpin, "Head Spin", "");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            DrawToggle(ref rainbowScreen, "Rainbow Screen", "");
            DrawToggle(ref uiGlitch, "UI Glitch", "");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label($"Vitesse Rotation Extrême : {headSpinSpeed:F0}", labelStyle);
            headSpinSpeed = GUILayout.HorizontalSlider(headSpinSpeed, 100f, 5000f);
            
            GUILayout.Space(5);
            if (GUILayout.Button(extremeHeadSpin ? "STOP ROTATION EXTRÊME" : "START ROTATION EXTRÊME", extremeHeadSpin ? activeTab : buttonStyle))
            {
                extremeHeadSpin = !extremeHeadSpin;
                Notify(extremeHeadSpin ? "SPIN ACTIVÉ !" : "Spin arrêté.");
            }
            GUILayout.EndVertical();
        }

        // ==========================================
        // ONGLET 6: CONFIGURATION (PREMIUM UI)
        // ==========================================
        void DrawSettings()
        {
            DrawSectionHeader("DIMENSIONS & AFFICHAGE GLOBALES");
            GUILayout.BeginVertical(boxStyle);
            
            GUILayout.Label($"Largeur de la fenêtre : {menuWidth:F0}px", labelStyle);
            menuWidth = GUILayout.HorizontalSlider(menuWidth, 500f, 1200f);
            
            GUILayout.Label($"Hauteur de la fenêtre : {menuHeight:F0}px", labelStyle);
            menuHeight = GUILayout.HorizontalSlider(menuHeight, 400f, 1000f);

            GUILayout.Space(10);
            GUILayout.Label($"Échelle globale de l'interface : {uiScale:F2}", labelStyle);
            uiScale = GUILayout.HorizontalSlider(uiScale, 0.5f, 2f);

            GUILayout.Label($"Taille de la police globale : {fontSizeBase}", labelStyle);
            int newFontSize = (int)GUILayout.HorizontalSlider(fontSizeBase, 10, 26);
            if (newFontSize != fontSizeBase)
            {
                fontSizeBase = newFontSize;
                stylesInitialized = false; 
            }
            
            GUILayout.Label($"Opacité du Menu : {menuAlpha:F2}", labelStyle);
            menuAlpha = GUILayout.HorizontalSlider(menuAlpha, 0.4f, 1f);
            GUILayout.EndVertical();

            DrawSectionHeader("COMPORTEMENT ET NOTIFICATIONS");
            GUILayout.BeginVertical(boxStyle);
            DrawToggle(ref notifications, "Afficher les Notifications Flottantes", "");
            DrawToggle(ref showWatermark, "Afficher le Filigrane (Watermark)", "");
            DrawToggle(ref pulseTitle, "Animation Pulsatile du Titre", "");
            GUILayout.EndVertical();

            DrawSectionHeader("THÈMES DE L'INTERFACE");
            GUILayout.BeginVertical(boxStyle);
            int newTheme = GUILayout.Toolbar(themeIndex, new[] { "Obsidian", "Cyberpunk", "Ocean", "Crimson" }, buttonStyle);
            if (newTheme != themeIndex)
            {
                themeIndex = newTheme;
                customColorMode = false;
                rainbowMode = false;
                ApplyTheme();
            }
            GUILayout.EndVertical();

            DrawSectionHeader("COULEURS EXPERTES ET MODIFICATION");
            GUILayout.BeginVertical(boxStyle);
            DrawToggle(ref rainbowMode, "🌈 Mode RGB Dynamique (Teintes changeantes)", "");
            if (rainbowMode)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Vitesse d'évolution RVB : {rainbowSpeed:F1}", labelStyle);
                rainbowSpeed = GUILayout.HorizontalSlider(rainbowSpeed, 0.1f, 3f);
            }
            else
            {
                DrawToggle(ref customColorMode, "🎨 Activer la Palette Totalement Personnalisée", "");
                if (customColorMode)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur Principale (Accentuation) ---", labelStyle);
                    customAccentColor.r = GUILayout.HorizontalSlider(customAccentColor.r, 0f, 1f);
                    customAccentColor.g = GUILayout.HorizontalSlider(customAccentColor.g, 0f, 1f);
                    customAccentColor.b = GUILayout.HorizontalSlider(customAccentColor.b, 0f, 1f);

                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur d'Arrière-Plan (Fond) ---", labelStyle);
                    customBgColor.r = GUILayout.HorizontalSlider(customBgColor.r, 0f, 1f);
                    customBgColor.g = GUILayout.HorizontalSlider(customBgColor.g, 0f, 1f);
                    customBgColor.b = GUILayout.HorizontalSlider(customBgColor.b, 0f, 1f);

                    GUILayout.Space(10);
                    GUILayout.Label("--- Couleur du Texte Général ---", labelStyle);
                    customTextColor.r = GUILayout.HorizontalSlider(customTextColor.r, 0f, 1f);
                    customTextColor.g = GUILayout.HorizontalSlider(customTextColor.g, 0f, 1f);
                    customTextColor.b = GUILayout.HorizontalSlider(customTextColor.b, 0f, 1f);
                }
            }
            GUILayout.EndVertical();
            
            DrawSectionHeader("SÉCURITÉ RÉSEAU DE L'HÔTE");
            GUILayout.BeginVertical(boxStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copier Lobby ID", buttonStyle)) ExecuteCommand("/lobby");
            if (GUILayout.Button("Bloquer le Radar", buttonStyle)) ExecuteCommand("/block radar");
            if (GUILayout.Button("Bloquer Aim Ennemi", buttonStyle)) ExecuteCommand("/block enemy");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(20);
            if (GUILayout.Button("RÉINITIALISER TOUTE LA CONFIGURATION", activeTab, GUILayout.Height(35)))
                ResetConfig();
        }

        // ==========================================
        // HELPERS, STYLES ET RENDU PROCEDURAL
        // ==========================================

        void DrawSectionHeader(string title)
        {
            GUILayout.Space(15);
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(title), headerStyle);
            DrawShadowText(rect, title, headerStyle, accentColor);
            GUILayout.Space(5);
        }

        // Méthode de dessin de texte avec ombre intégrée
        void DrawShadowText(Rect position, string text, GUIStyle style, Color mainColor)
        {
            style.normal.textColor = new Color(0, 0, 0, 0.7f); // Couleur de l'ombre
            Rect shadowRect = new Rect(position.x + 1, position.y + 1, position.width, position.height);
            GUI.Label(shadowRect, text, style);
            style.normal.textColor = mainColor; // Couleur principale
            GUI.Label(position, text, style);
        }

        // Toggle refait style "Modern Switch"
        void DrawToggle(ref bool state, string label, string command)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle);
            GUILayout.FlexibleSpace();
            bool newState = GUILayout.Toggle(state, state ? "ON" : "OFF", state ? toggleBtnOn : toggleBtnOff, GUILayout.Width(60), GUILayout.Height(22));
            if (newState != state)
            {
                state = newState;
                if (!string.IsNullOrEmpty(command))
                {
                    ExecuteCommand(command);
                    Notify($"{command} {(state ? "ACTIVÉ" : "DÉSACTIVÉ")}");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        void ResetConfig()
        {
            godMode = infiniteStamina = noclip = unlimitedJump = esp = brightVision = betaMode = false;
            stunClick = killClick = invis = hearAll = rapidUse = false;
            fakeLag = invertControls = headSpin = cameraShake = rainbowScreen = uiGlitch = timeJitter = fakeFreeze = notifSpam = drunkCamera = extremeHeadSpin = false;
            
            uiScale = speedHack = brightness = headSpinSpeed = 1f;
            menuAlpha = 0.98f;
            menuWidth = 780f;
            menuHeight = 680f;
            fontSizeBase = 13;
            
            rainbowMode = false;
            customColorMode = false;
            themeIndex = 0;
            
            stylesInitialized = false;
            Notify("Configuration réinitialisée à zéro.");
        }

        void Notify(string msg)
        {
            notif = msg;
            notifTimer = 3.5f;
        }

        void ExecuteCommand(string commandName)
        {
            if (!commandName.StartsWith("/")) commandName = "/" + commandName;
            Chat.ExecuteCommand(commandName);
        }

        // ==== GENERATEURS DE TEXTURES (BORDER & GRADIENT) ====

        private Texture2D MakeTexWithBorder(int width, int height, Color col, Color borderCol, int borderWidth)
        {
            Color[] pix = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < borderWidth || x >= width - borderWidth || y < borderWidth || y >= height - borderWidth)
                        pix[y * width + x] = borderCol;
                    else
                        pix[y * width + x] = col;
                }
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private Texture2D MakeGradientTex(int width, int height, Color topColor, Color bottomColor)
        {
            Color[] pix = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                Color lerp = Color.Lerp(bottomColor, topColor, (float)y / (height - 1));
                for (int x = 0; x < width; x++)
                    pix[y * width + x] = lerp;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        void ApplyTheme()
        {
            if (customColorMode && !rainbowMode)
            {
                bgColor = new Color(customBgColor.r, customBgColor.g, customBgColor.b, menuAlpha);
                buttonColor = new Color(customBgColor.r + 0.08f, customBgColor.g + 0.08f, customBgColor.b + 0.08f, menuAlpha);
                textColor = customTextColor;
                accentColor = customAccentColor;
                borderColor = new Color(textColor.r, textColor.g, textColor.b, 0.2f);
                stylesInitialized = false; 
                return;
            }

            Color baseBg = Color.black;
            Color baseBtn = Color.gray;
            Color baseText = Color.white;
            Color baseAccent = Color.cyan;

            switch (themeIndex)
            {
                case 0: // Obsidian (Dark)
                    baseBg = new Color(0.08f, 0.08f, 0.09f);
                    baseBtn = new Color(0.13f, 0.13f, 0.15f);
                    baseText = new Color(0.9f, 0.9f, 0.9f);
                    baseAccent = new Color(0f, 0.8f, 1f);
                    break;
                case 1: // Cyberpunk
                    baseBg = new Color(0.09f, 0.05f, 0.15f);
                    baseBtn = new Color(0.18f, 0.10f, 0.28f);
                    baseText = new Color(0.98f, 0.92f, 0.1f);
                    baseAccent = new Color(0.1f, 0.95f, 0.98f);
                    break;
                case 2: // Ocean
                    baseBg = new Color(0.02f, 0.08f, 0.12f);
                    baseBtn = new Color(0.06f, 0.15f, 0.22f);
                    baseText = new Color(0.9f, 0.95f, 1f);
                    baseAccent = new Color(0f, 0.9f, 0.6f);
                    break;
                case 3: // Crimson (Red)
                    baseBg = new Color(0.1f, 0.03f, 0.03f);
                    baseBtn = new Color(0.18f, 0.06f, 0.06f);
                    baseText = new Color(1f, 0.85f, 0.85f);
                    baseAccent = new Color(1f, 0.3f, 0.3f);
                    break;
            }

            bgColor = new Color(baseBg.r, baseBg.g, baseBg.b, menuAlpha);
            buttonColor = new Color(baseBtn.r, baseBtn.g, baseBtn.b, menuAlpha);
            textColor = baseText;
            borderColor = new Color(baseText.r, baseText.g, baseText.b, 0.15f);
            
            if (rainbowMode) accentColor = customAccentColor;
            else { accentColor = baseAccent; customAccentColor = baseAccent; customBgColor = baseBg; customTextColor = baseText; }
            
            stylesInitialized = false; 
        }

        void InitStyles()
        {
            if (stylesInitialized) return;
            
            Texture2D windowGradient = MakeGradientTex(128, 128, new Color(bgColor.r + 0.05f, bgColor.g + 0.05f, bgColor.b + 0.05f, menuAlpha), bgColor);
            Texture2D btnTex = MakeTexWithBorder(128, 32, buttonColor, borderColor, 1);
            Texture2D btnHoverTex = MakeTexWithBorder(128, 32, new Color(buttonColor.r + 0.05f, buttonColor.g + 0.05f, buttonColor.b + 0.05f, menuAlpha), accentColor, 1);
            Texture2D activeTex = MakeTexWithBorder(128, 32, new Color(accentColor.r, accentColor.g, accentColor.b, 0.8f), accentColor, 2);
            Texture2D boxTex = MakeTexWithBorder(128, 128, new Color(bgColor.r + 0.02f, bgColor.g + 0.02f, bgColor.b + 0.02f, menuAlpha), borderColor, 1);

            windowStyle = new GUIStyle(GUI.skin.window)
            {
                fontStyle = FontStyle.Bold,
                fontSize = fontSizeBase + 3,
                alignment = TextAnchor.UpperCenter,
                padding = new RectOffset(10, 10, 30, 10) // Espace pour le titre
            };
            windowStyle.normal.background = windowGradient;
            windowStyle.normal.textColor = accentColor;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSizeBase,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(8, 8, 4, 4)
            };
            buttonStyle.normal.background = btnTex;
            buttonStyle.hover.background = btnHoverTex;
            buttonStyle.active.background = activeTex;
            buttonStyle.normal.textColor = textColor;
            buttonStyle.hover.textColor = accentColor;

            tabStyle = new GUIStyle(buttonStyle);
            activeTab = new GUIStyle(buttonStyle);
            activeTab.normal.background = activeTex;
            activeTab.normal.textColor = Color.white; // Toujours lisible sur l'accent
            activeTab.hover.textColor = Color.white;

            toggleBtnOn = new GUIStyle(activeTab);
            toggleBtnOn.alignment = TextAnchor.MiddleCenter;
            toggleBtnOff = new GUIStyle(buttonStyle);
            toggleBtnOff.alignment = TextAnchor.MiddleCenter;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSizeBase,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            labelStyle.normal.textColor = textColor;

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSizeBase + 2,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            watermarkStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = fontSizeBase + 4,
                alignment = TextAnchor.UpperLeft
            };

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(12, 12, 12, 12),
                margin = new RectOffset(5, 5, 8, 8)
            };
            boxStyle.normal.background = boxTex;
            
            textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = fontSizeBase,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(5, 5, 3, 3)
            };
            textFieldStyle.normal.background = btnTex;
            textFieldStyle.hover.background = btnHoverTex;
            textFieldStyle.normal.textColor = accentColor;
            textFieldStyle.focused.textColor = accentColor;

            // Remplacement des composants par défaut du système GUI
            GUI.skin.horizontalSlider.normal.background = btnTex;
            GUI.skin.horizontalSlider.fixedHeight = 10f;
            GUI.skin.horizontalSliderThumb.normal.background = activeTex;
            GUI.skin.horizontalSliderThumb.hover.background = activeTex;
            GUI.skin.horizontalSliderThumb.fixedWidth = 15f;
            GUI.skin.horizontalSliderThumb.fixedHeight = 15f;

            stylesInitialized = true;
        }

        async void RunSpammWear(CancellationToken token)
        {
            string[] suits = new string[] { "armor", "hazmat", "spacesuit", "riot", "stealth" };
            try
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var suit in suits)
                    {
                        if (token.IsCancellationRequested) break;
                        ExecuteCommand($"/suit {suit}");
                        await System.Threading.Tasks.Task.Delay(100, token);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}