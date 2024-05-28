using UISystem;

public abstract class UIWindow : UIElementBase
{
    protected override void OnShowBegin()
    {
        GameInputFocus.Focus(GameSystemType.UI, this);
    }

    protected override void OnHideBegin()
    {
        GameInputFocus.Unfocus(this);
    }
}

public abstract class UIWindow<T> : UIElementView<T>
{
    protected override void OnShowBegin()
    {
        GameInputFocus.Focus(GameSystemType.UI, this);
    }

    protected override void OnHideBegin()
    {
        GameInputFocus.Unfocus(this);
    }
}
