using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public abstract class InputValueContextBase
{
    public abstract void SetValue(InputAction.CallbackContext context);
    public abstract void RegisterValueChangingCallback(Action callback, GameSystemType systemType);
}

public abstract class InputValueContext<T> : InputValueContextBase where T : struct
{
    public T Value { get; private set; }

    private Dictionary<GameSystemType, List<Action>> _callbacks = new();

    public override void SetValue(InputAction.CallbackContext context)
    {
        // Source: Input player component
        if (!(context.performed || (context.canceled)))
            return;

        var tempValue = ValueProcessor(context.ReadValue<T>(), context);
        var valueChanged = !Value.Equals(tempValue);

        Value = tempValue;

        if (valueChanged)
        {
            foreach (var callbackItem in _callbacks)
            {
                //if (GameInputFocus.IsActive(callbackItem.Key))
                {
                    callbackItem.Value.ForEach(x => x?.Invoke());
                }
            }
        }
    }

    public override void RegisterValueChangingCallback(Action callback, GameSystemType systemType)
    {
        if (!_callbacks.ContainsKey(systemType))
        {
            _callbacks.Add(systemType, new List<Action>());
            _callbacks = _callbacks.OrderByDescending(x => (int)x.Key)
                .ToDictionary(
                    x => x.Key, 
                    x => x.Value);
        }

        _callbacks[systemType].Add(callback);
    }

    protected virtual T ValueProcessor(T value, InputAction.CallbackContext context)
    {
        return value;
    }
}