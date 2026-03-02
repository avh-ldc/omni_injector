using UnityEngine;
using UnityEngine.InputSystem;

namespace Omni.Menu.Data
{
    public static class MenuSettings
    {
        // --- Paramètres de la Fenêtre ---
        public static Key openKey = Key.Insert;
        public static Rect windowRect = new Rect(50, 50, 780, 680);
        public static float menuWidth = 780f;
        public static float menuHeight = 680f;
        public static float menuAlpha = 0.98f;
        public static float uiScale = 1f;
        public static int fontSizeBase = 13;

        // --- Esthétique & Thèmes ---
        public static bool notifications = true;
        public static bool showWatermark = true;
        public static bool pulseTitle = false;
        public static int themeIndex = 0;
        public static bool rainbowMode = false;
        public static float rainbowSpeed = 0.5f;
        public static bool customColorMode = false;

        public static Color customAccentColor = new Color(0f, 0.8f, 1f);
        public static Color customBgColor = new Color(0.08f, 0.08f, 0.1f);
        public static Color customTextColor = new Color(0.9f, 0.9f, 0.9f);

        // --- États des Cheats (Toggles) ---
        public static bool godMode, infiniteStamina, noclip, unlimitedJump;
        public static bool stunClick, killClick, invis, hearAll, rapidUse, betaMode;
        public static bool esp, brightVision;
        public static bool fakeLag, invertControls, headSpin, cameraShake;
        public static bool fovPulse, rainbowScreen, timeJitter, fakeFreeze;
        public static bool notifSpam, drunkCamera, uiGlitch, extremeHeadSpin;
        public static bool spammSuit;

        // --- Valeurs Numériques (Sliders & Inputs) ---
        public static float speedHack = 1f;
        public static float brightness = 1f;
        public static float headSpinSpeed = 1000f;
        
        public static string xpInput = "1000";
        public static string visitInput = "Titan";
        public static string moneyInput = "5000";
        public static string quotaInput = "2000";
        public static string buyInput = "shovel";
        public static string buyQty = "1";
        public static string chatInput = "LDC ON TOP !";
        public static string noiseDuration = "30";
        public static string poisonDmg = "15";
        public static string poisonDur = "30";
        public static string poisonDelay = "2";
        public static string maskAmount = "1";
        public static string spinInput = "10";
        public static string enemyToSpawn = "Girl";
        public static string spawnAmount = "1";
    }
}