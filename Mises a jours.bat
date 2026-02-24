@echo off
setlocal enabledelayedexpansion
:: ==========================================
:: CONFIGURATION
:: ==========================================
title OMNI INJECTOR - UPDATER
mode con: cols=85 lines=30
color 0B

:: --- PARAMETRES DU DEPOT ---
set "REPO_URL=https://github.com/avh-ldc/tartingrad.git"
set "BRANCH=main"

:: --- CORRECTIF CRITIQUE ---
cd /d "%~dp0"
:: --------------------------

:: ==========================================
:: BANNIERE ASCII
:: ==========================================
cls
echo.
echo  ============================================================
echo   ___  __  __ _   _ ___   ___ _   _     _  _____ _____ ___  ____  
echo  / _ \^|  \/  ^| \ ^| ^|_ _^| ^|_ _^| \ ^| ^|   ^| ^|^| ____^|_   _/ _ \^|  _ \ 
echo ^| ^| ^| ^| ^|\/^| ^|  \^| ^|^| ^|    ^| ^| ^|  \^| ^|_  ^| ^|^|  _^|   ^| ^|^| ^| ^| ^| ^|_) ^|
echo ^| ^|_^| ^| ^|  ^| ^| ^|\  ^|^| ^|    ^| ^| ^| ^|\  ^| ^|_^| ^|^| ^|___  ^| ^|^| ^|_^| ^|  _ ^< 
echo  \___/^|_^|  ^|_^|_^| \_^|___^|  ^|___^|_^| \_^|\___/^|_____^| ^|_^| \___/^|_^| \_\
echo.
echo                 [ MODULE DE MISE A JOUR ]
echo  ============================================================
echo.

:: ==========================================
:: ETAPE 1 : VERIFICATION DOSSIER GIT
:: ==========================================
if not exist ".git" (
    color 0C
    echo.
    echo  [!] ERREUR FATALE :
    echo      Ce dossier n'est pas un depot Git valide.
    echo      Le dossier ".git" est introuvable.
    echo.
    echo  Appuyez sur une touche pour quitter...
    pause >nul
    exit /b
)

:: ==========================================
:: ETAPE 2 : CONFIGURATION ET CONNEXION
:: ==========================================
echo  [*] Configuration de l'adresse distante...

git remote set-url origin !REPO_URL! >nul 2>&1
if !ERRORLEVEL! NEQ 0 (
    git remote add origin !REPO_URL! >nul 2>&1
)

echo  [*] Connexion au serveur GitHub...
git fetch origin !BRANCH! >nul 2>&1

if !ERRORLEVEL! NEQ 0 (
    color 0C
    echo.
    echo  [!] ECHEC DE CONNEXION AU DEPOT.
    echo      Analyse des causes probables :
    echo      1. La branche n'est pas "main" - testez avec "master".
    echo      2. Le depot est prive et Git attend une authentification.
    echo      3. Le lien du depot est incorrect ou n'existe plus.
    echo.
    echo  Appuyez sur une touche pour quitter...
    pause >nul
    exit /b
)

:: ==========================================
:: ETAPE 3 : COMPARAISON DES VERSIONS
:: ==========================================
for /f "delims=" %%i in ('git rev-parse HEAD') do set LOCAL=%%i
for /f "delims=" %%i in ('git rev-parse origin/!BRANCH!') do set REMOTE=%%i

echo  [*] Verification de l'integrite des fichiers...

if "!LOCAL!" == "!REMOTE!" (
    :: DEJA A JOUR
    echo.
    echo  [+] AUCUNE MISE A JOUR REQUISE.
    echo      Hash actuel : !LOCAL:~0,7!
    echo.
) else (
    :: MISE A JOUR REQUISE
    color 0E
    echo.
    echo  [!] NOUVELLE VERSION DISPONIBLE !
    echo      Installation de la mise a jour en cours...
    echo  -------------------------------------------------------
    
    git pull origin !BRANCH!
    
    if !ERRORLEVEL! EQU 0 (
        echo.
        echo  [+] Mise a jour installee avec succes.
    ) else (
        color 0C
        echo.
        echo  [!] Erreur lors de l'installation.
        echo      Conflit possible avec des fichiers locaux modifies.
    )
    echo  -------------------------------------------------------
)

:: ==========================================
:: FINAL - MESSAGE SPECIFIQUE LDC
:: ==========================================
color 0A
echo.
echo  ============================================================
echo  [OK] VOTRE CHEAT EST A JOUR.
echo.
echo        MERCI D'AVOIR CHOISI LA LDC.
echo  ============================================================
echo.
pause >nul