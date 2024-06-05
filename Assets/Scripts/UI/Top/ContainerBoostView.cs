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
        [AutoSetupField] private Slider _speedBoost;
        
        protected override void UpdateView(ContainerBoostModel model)
        {
            _speedBoost.value = model.value;
        }
    }
}