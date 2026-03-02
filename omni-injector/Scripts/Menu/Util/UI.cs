using System.Collections.Generic;
using UnityEngine;
using Omni.Menu.Data;

namespace Omni.Menu.Util
{
    /// <summary>
    /// Moteur de rendu de l'interface graphique (Version Premium).
    /// </summary>
    public static class UI
    {
        // --- Couleurs Calculées ---
        public static Color bgColor, buttonColor, textColor, accentColor, borderColor;

        // --- Styles Mis en Cache ---
        public static GUIStyle windowStyle, buttonStyle, tabStyle, activeTab;
        public static GUIStyle labelStyle, headerStyle, watermarkStyle, boxStyle, textFieldStyle;
        
        // Cache pour nos textures générées à la volée (optimisation des FPS)
        private static Dictionary<Color, Texture2D> solidTextures = new Dictionary<Color, Texture2D>();
        
        private static bool stylesInitialized = false;

        public static void HandleStyles()
        {
            if (MenuSettings.rainbowMode)
            {
                MenuSettings.customAccentColor = Color.HSVToRGB((Time.time * MenuSettings.rainbowSpeed) % 1f, 0.8f, 1f);
                stylesInitialized = false; 
            }

            if (!stylesInitialized)
            {
                ApplyTheme();
                InitStyles();
                stylesInitialized = true;
            }
        }

        public static void RequestStyleRefresh()
        {
            stylesInitialized = false;
            solidTextures.Clear(); // On vide le cache des couleurs
        }

        private static void ApplyTheme()
        {
            if (MenuSettings.customColorMode && !MenuSettings.rainbowMode)
            {
                bgColor = new Color(MenuSettings.customBgColor.r, MenuSettings.customBgColor.g, MenuSettings.customBgColor.b, MenuSettings.menuAlpha);
                buttonColor = new Color(MenuSettings.customBgColor.r + 0.08f, MenuSettings.customBgColor.g + 0.08f, MenuSettings.customBgColor.b + 0.08f, MenuSettings.menuAlpha);
                textColor = MenuSettings.customTextColor;
                accentColor = MenuSettings.customAccentColor;
                borderColor = new Color(textColor.r, textColor.g, textColor.b, 0.2f);
                return;
            }

            Color baseBg = Color.black;
            Color baseBtn = Color.gray;
            Color baseText = Color.white;
            Color baseAccent = Color.cyan;

            switch (MenuSettings.themeIndex)
            {
                case 0: // Obsidian (Sombre classique)
                    baseBg = new Color(0.08f, 0.08f, 0.09f);
                    baseBtn = new Color(0.13f, 0.13f, 0.15f);
                    baseText = new Color(0.9f, 0.9f, 0.9f);
                    baseAccent = new Color(0f, 0.8f, 1f);
                    break;
                case 1: // Cyberpunk (Violet/Jaune)
                    baseBg = new Color(0.09f, 0.05f, 0.15f);
                    baseBtn = new Color(0.18f, 0.10f, 0.28f);
                    baseText = new Color(0.98f, 0.92f, 0.1f);
                    baseAccent = new Color(0.1f, 0.95f, 0.98f);
                    break;
                case 2: // Ocean (Bleu marin)
                    baseBg = new Color(0.02f, 0.08f, 0.12f);
                    baseBtn = new Color(0.06f, 0.15f, 0.22f);
                    baseText = new Color(0.9f, 0.95f, 1f);
                    baseAccent = new Color(0f, 0.9f, 0.6f);
                    break;
                case 3: // Crimson (Rouge sang)
                    baseBg = new Color(0.1f, 0.03f, 0.03f);
                    baseBtn = new Color(0.18f, 0.06f, 0.06f);
                    baseText = new Color(1f, 0.85f, 0.85f);
                    baseAccent = new Color(1f, 0.3f, 0.3f);
                    break;
                case 4: // Hacker (Matrix Vert) - NOUVEAU
                    baseBg = new Color(0.01f, 0.05f, 0.01f);
                    baseBtn = new Color(0.02f, 0.12f, 0.02f);
                    baseText = new Color(0.2f, 1f, 0.2f);
                    baseAccent = new Color(0.4f, 1f, 0.4f);
                    break;
                case 5: // Midnight (Sombre élégant) - NOUVEAU
                    baseBg = new Color(0.05f, 0.05f, 0.08f);
                    baseBtn = new Color(0.1f, 0.1f, 0.15f);
                    baseText = new Color(0.85f, 0.85f, 0.9f);
                    baseAccent = new Color(0.5f, 0.4f, 1f);
                    break;
            }

            bgColor = new Color(baseBg.r, baseBg.g, baseBg.b, MenuSettings.menuAlpha);
            buttonColor = new Color(baseBtn.r, baseBtn.g, baseBtn.b, MenuSettings.menuAlpha);
            textColor = baseText;
            borderColor = new Color(baseText.r, baseText.g, baseText.b, 0.15f);

            if (MenuSettings.rainbowMode) 
                accentColor = MenuSettings.customAccentColor;
            else 
            { 
                accentColor = baseAccent; 
                MenuSettings.customAccentColor = baseAccent; 
                MenuSettings.customBgColor = baseBg; 
                MenuSettings.customTextColor = baseText; 
            }
        }

        private static void InitStyles()
        {
            Texture2D windowGradient = MakeGradientTex(128, 128, new Color(bgColor.r + 0.05f, bgColor.g + 0.05f, bgColor.b + 0.05f, MenuSettings.menuAlpha), bgColor);
            Texture2D btnTex = MakeTexWithBorder(128, 32, buttonColor, borderColor, 1);
            Texture2D btnHoverTex = MakeTexWithBorder(128, 32, new Color(buttonColor.r + 0.05f, buttonColor.g + 0.05f, buttonColor.b + 0.05f, MenuSettings.menuAlpha), accentColor, 1);
            Texture2D activeTex = MakeTexWithBorder(128, 32, new Color(accentColor.r, accentColor.g, accentColor.b, 0.8f), accentColor, 2);
            Texture2D boxTex = MakeTexWithBorder(128, 128, new Color(bgColor.r + 0.02f, bgColor.g + 0.02f, bgColor.b + 0.02f, MenuSettings.menuAlpha), borderColor, 1);

            windowStyle = new GUIStyle(GUI.skin.window)
            {
                fontStyle = FontStyle.Bold,
                fontSize = MenuSettings.fontSizeBase + 3,
                alignment = TextAnchor.UpperCenter,
                padding = new RectOffset(10, 10, 30, 10)
            };
            windowStyle.normal.background = windowGradient;
            windowStyle.normal.textColor = accentColor;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = MenuSettings.fontSizeBase,
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
            activeTab.normal.textColor = Color.white;
            activeTab.hover.textColor = Color.white;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = MenuSettings.fontSizeBase,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            labelStyle.normal.textColor = textColor;

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = MenuSettings.fontSizeBase + 2,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            watermarkStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = MenuSettings.fontSizeBase + 4,
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
                fontSize = MenuSettings.fontSizeBase,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(5, 5, 3, 3)
            };
            textFieldStyle.normal.background = btnTex;
            textFieldStyle.hover.background = btnHoverTex;
            textFieldStyle.normal.textColor = accentColor;
            textFieldStyle.focused.textColor = accentColor;

            GUI.skin.horizontalSlider.normal.background = btnTex;
            GUI.skin.horizontalSlider.fixedHeight = 10f;
            GUI.skin.horizontalSliderThumb.normal.background = activeTex;
            GUI.skin.horizontalSliderThumb.hover.background = activeTex;
            GUI.skin.horizontalSliderThumb.fixedWidth = 15f;
            GUI.skin.horizontalSliderThumb.fixedHeight = 15f;
        }

        // ==========================================
        // COMPOSANTS RÉUTILISABLES
        // ==========================================

        public static void DrawSectionHeader(string title)
        {
            GUILayout.Space(15);
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(title), headerStyle);
            DrawShadowText(rect, title, headerStyle, accentColor);
            GUILayout.Space(5);
        }

        public static void DrawShadowText(Rect position, string text, GUIStyle style, Color mainColor)
        {
            style.normal.textColor = new Color(0, 0, 0, 0.7f);
            Rect shadowRect = new Rect(position.x + 1, position.y + 1, position.width, position.height);
            GUI.Label(shadowRect, text, style);
            style.normal.textColor = mainColor;
            GUI.Label(position, text, style);
        }

        /// <summary>
        /// Bouton standard avec support des infobulles (Tooltip)
        /// </summary>
        public static bool DrawButton(string text, string tooltip = "")
        {
            // On utilise un GUIContent pour attacher un Tooltip au bouton
            return GUILayout.Button(new GUIContent(text, tooltip), buttonStyle);
        }

        /// <summary>
        /// NOUVEAU: Un interrupteur graphique (Switch) moderne au lieu d'un bouton ON/OFF classique.
        /// </summary>
        public static bool DrawToggle(ref bool state, string label, string tooltip = "")
        {
            bool changed = false;
            GUILayout.BeginHorizontal();
            
            // Texte avec Tooltip potentiel
            GUILayout.Label(new GUIContent(label, tooltip), labelStyle);
            GUILayout.FlexibleSpace();
            
            // Zone de dessin de l'interrupteur
            Rect switchRect = GUILayoutUtility.GetRect(44f, 22f);
            
            // Clic invisible sur la zone
            if (GUI.Button(switchRect, "", GUIStyle.none))
            {
                state = !state;
                changed = true;
            }

            // --- Dessin procédural du Switch ---
            // 1. Le fond (Gris si OFF, Couleur du thème si ON)
            Color trackColor = state ? new Color(accentColor.r, accentColor.g, accentColor.b, 0.8f) : new Color(0.2f, 0.2f, 0.2f, 0.8f);
            GUI.DrawTexture(switchRect, GetSolidTexture(trackColor));

            // 2. Les bordures de l'interrupteur
            DrawRectOutline(switchRect, borderColor, 1);

            // 3. Le curseur (Le petit carré à l'intérieur)
            float thumbX = state ? switchRect.x + 24 : switchRect.x + 4; // Se déplace à droite si ON
            Rect thumbRect = new Rect(thumbX, switchRect.y + 4, 16, 14);
            
            Color thumbColor = state ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            GUI.DrawTexture(thumbRect, GetSolidTexture(thumbColor));

            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            
            return changed;
        }

        /// <summary>
        /// Fonction à appeler à la toute fin de OnGUI (dans MenuUI) pour afficher l'infobulle actuelle.
        /// </summary>
        public static void DrawTooltip()
        {
            if (!string.IsNullOrEmpty(GUI.tooltip))
            {
                Vector2 mousePos = Event.current.mousePosition;
                GUIContent content = new GUIContent(GUI.tooltip);
                Vector2 size = labelStyle.CalcSize(content);
                
                // Positionnement de l'infobulle sous la souris
                Rect tooltipRect = new Rect(mousePos.x + 15, mousePos.y + 15, size.x + 16, size.y + 8);
                
                GUI.DrawTexture(tooltipRect, GetSolidTexture(new Color(0.1f, 0.1f, 0.1f, 0.95f)));
                DrawRectOutline(tooltipRect, accentColor, 1);
                
                GUIStyle ttStyle = new GUIStyle(labelStyle);
                ttStyle.normal.textColor = Color.white;
                ttStyle.alignment = TextAnchor.MiddleCenter;
                
                GUI.Label(tooltipRect, content, ttStyle);
            }
        }

        // ==========================================
        // OUTILS DE DESSIN BAS NIVEAU
        // ==========================================

        private static void DrawRectOutline(Rect rect, Color color, int thickness)
        {
            Texture2D tex = GetSolidTexture(color);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), tex); // Haut
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), tex); // Bas
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), tex); // Gauche
            GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), tex); // Droite
        }

        /// <summary>
        /// Récupère ou génère une texture unie de 1x1 pixel (très optimisé grâce au dictionnaire)
        /// </summary>
        private static Texture2D GetSolidTexture(Color color)
        {
            if (!solidTextures.ContainsKey(color))
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, color);
                tex.Apply();
                solidTextures[color] = tex;
            }
            return solidTextures[color];
        }

        private static Texture2D MakeTexWithBorder(int width, int height, Color col, Color borderCol, int borderWidth)
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

        private static Texture2D MakeGradientTex(int width, int height, Color topColor, Color bottomColor)
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
    }
}