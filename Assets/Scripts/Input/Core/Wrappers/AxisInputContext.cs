using UnityEngine;

public class AxisInputContext : InputValueContext<Vector2>
{
    //TODO: Add threshould constant.
    public bool IsShifted => Value.magnitude >= 0.3F;
    public bool IsRealesed => Value.magnitude < 0.2F;
}
