using System;
using System.Collections.Generic;
using System.Linq;
using Components.Boxes.Views.Impl;
using Database;
using Enums;
using Infrastructure.Factories.Impl;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class BoxPool : IInitializable, IDisposable
{
    private BoxFactory _boxFactory;
    private BoxPoolConfig _boxPoolConfig;
    private BoxStateFactory _boxStateFactory;
    
    private Dictionary<EBoxGrade, ObjectPool<BoxView>> _pools;
    private Dictionary<EBoxGrade, Transform> _poolTransforms;

    [Inject]
    public void Construct(
        BoxFactory boxFactory,
        BoxPoolConfig boxPoolConfig,
        BoxStateFactory boxStateFactory
    )
    {
        _boxFactory = boxFactory;
        _boxPoolConfig = boxPoolConfig;
        _boxStateFactory = boxStateFactory;
    }

    public void Initialize()
    {
        _pools = new Dictionary<EBoxGrade, ObjectPool<BoxView>>();
        _poolTransforms = new Dictionary<EBoxGrade, Transform>();

        var poolsParent = new GameObject("BoxPools").transform;

        foreach (EBoxGrade grade in Enum.GetValues(typeof(EBoxGrade)))
        {
            if (grade == EBoxGrade.None) 
                continue;

            var poolTransform = new GameObject($"Pool_{grade}").transform;
            poolTransform.parent = poolsParent;
            _poolTransforms[grade] = poolTransform;

            var pool = CreatePoolForGrade(grade, poolTransform);
            _pools[grade] = pool;
            
            var initialCount = _boxPoolConfig.config.First(g => g.grade == grade).initialCount;

            PreloadPool(pool, initialCount);
        }
    }

    private ObjectPool<BoxView> CreatePoolForGrade(EBoxGrade grade, Transform parentTransform)
    {
        var capacity = _boxPoolConfig.config.First(g => g.grade == grade).initialCount;
        var maxSize = _boxPoolConfig.config.First(g => g.grade == grade).maxCount;
        
        return new ObjectPool<BoxView>(
            () => CreateBox(grade, parentTransform),
            OnGetBox,
            OnReleaseBox,
            null,
            true, 
            capacity,  
            maxSize  
        );
    }

    private BoxView CreateBox(EBoxGrade grade, Transform parentTransform)
    {
        var box = _boxFactory.Create(grade);
        box.transform.parent = parentTransform;
        return box;
    }

    private void PreloadPool(ObjectPool<BoxView> pool, int count)
    {
        var tempArray = new BoxView[count];
        for (var i = 0; i < count; i++)
        {
            tempArray[i] = pool.Get();
        }

        foreach (var box in tempArray)
        {
            pool.Release(box);
        }
    }
    
    public BoxView GetBox(EBoxGrade grade)
    {
        if (_pools.TryGetValue(grade, out var pool))
        {
            return pool.Get();
        }
        
        throw new ArgumentException($"No pool found for grade {grade}");
    }

    public void ReturnToPool(BoxView box, EBoxGrade grade)
    {
        if (_pools.TryGetValue(grade, out var pool))
        {
            pool.Release(box);
        }
        else
        {
            throw new ArgumentException($"No pool found for grade {grade}");
        }
    }

    private void OnGetBox(BoxView box)
    {
        var idleState = _boxStateFactory.CreateIdleState();
        box.stateContext.SetState(idleState);
        box.gameObject.SetActive(true);
        box.isBot = false;
        box.isPlayer = false;
        box.isMerging = false;
        box.isIdle = false;
        box.isDestroyed = false;
        box.IsSpeedBoosted = false;
        box.DisableNick();
        box.UpdateBoostVFXStatus(false);
        box.UpdateAccelerationSliderStatus(0f, false);
    }

    private void OnReleaseBox(BoxView box)
    {
        box.isDestroyed = true;
        box.DisableNick();
        box.UpdateBoostVFXStatus(false);
        box.UpdateAccelerationSliderStatus(0f, false);
        box.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Dispose();
        }
        _pools.Clear();
    }
}
