using UISystem;
using UnityEngine.UI;

namespace UI.Top
{
    public class ContainerBoostModel
    {
        public float value;
    }
    
    public class ContainerBoostView : UIElementView<ContainerBoostModel>
    {
        [AutoSetupField] private Image _speedBoost;
        
        protected override void UpdateView(ContainerBoostModel model)
        {
            _speedBoost.fillAmount = model.value;
        }
    }
}