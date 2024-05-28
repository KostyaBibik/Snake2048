using UnityEngine;

namespace UISystem
{
    public abstract class UIController : MonoBehaviour
    {
        public abstract void HideIrregular(UIElementBase view);

        public abstract void ShowAsIrregular(UIElementBase view);
    }
}