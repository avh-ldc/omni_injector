using UnityEngine;

namespace Omni.Menu.Core
{
    public abstract class MenuTab
    {
        public string TabName { get; protected set; }
        
        // CORRECTION : Passé en public pour que MenuUI puisse gérer le défilement
        public Vector2 scrollPosition = Vector2.zero; 

        protected MenuTab(string name)
        {
            TabName = name;
        }

        /// <summary>
        /// Méthode principale appelée par le MenuUI pour dessiner le contenu de l'onglet.
        /// Doit être surchargée (override) par chaque onglet spécifique.
        /// </summary>
        public abstract void Draw();
    }
}