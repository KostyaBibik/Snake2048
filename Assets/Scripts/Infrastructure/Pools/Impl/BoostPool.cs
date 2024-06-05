using System;
using System.Collections.Generic;
using System.Linq;
using Components.Boosts.Impl;
using Database;
using Enums;
using Infrastructure.Factories.Impl;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Infrastructure.Pools.Impl
{
    public class BoostPool : IInitializable, IDisposable
    {
        private BoostFactory _boostFactory;
        private BoostPoolConfig _boostPoolConfig;

        private Dictionary<EBoxBoost, ObjectPool<BoostView>> _pools;
        private Dictionary<EBoxBoost, Transform> _poolTransforms;

        [Inject]
        public void Construct(
            BoostFactory boostFactory,
            BoostPoolConfig boostPoolConfig
        )
        {
            _boostFactory = boostFactory;
            _boostPoolConfig = boostPoolConfig;
        }

        public void Initialize()
        {
            _pools = new Dictionary<EBoxBoost, ObjectPool<BoostView>>();
            _poolTransforms = new Dictionary<EBoxBoost, Transform>();

            var poolsParent = new GameObject(nameof(BoostPool)).transform;

            foreach (EBoxBoost type in Enum.GetValues(typeof(EBoxBoost)))
            {
                if (type == EBoxBoost.None)
                    continue;

                var poolTransform = new GameObject($"Pool_{type}").transform;
                poolTransform.parent = poolsParent;
                _poolTransforms[type] = poolTransform;

                var pool = CreatePoolForType(type, poolTransform);
                _pools[type] = pool;

                var initialCount = _boostPoolConfig.config.First(g => g.grade == type).initialCount;

                PreloadPool(pool, initialCount);
            }
        }

        private ObjectPool<BoostView> CreatePoolForType(EBoxBoost grade, Transform parentTransform)
        {
            var capacity = _boostPoolConfig.config.First(g => g.grade == grade).initialCount;
            var maxSize = _boostPoolConfig.config.First(g => g.grade == grade).maxCount;

            return new ObjectPool<BoostView>(
                () => CreateBoost(grade, parentTransform),
                OnGetBoost,
                OnReleaseBoost,
                null,
                true,
                capacity,
                maxSize
            );
        }

        private BoostView CreateBoost(EBoxBoost grade, Transform parentTransform)
        {
            var box = _boostFactory.Create(grade);
            box.transform.parent = parentTransform;
            return box;
        }

        private void PreloadPool(ObjectPool<BoostView> pool, int count)
        {
            var tempArray = new BoostView[count];
            for (var i = 0; i < count; i++)
            {
                tempArray[i] = pool.Get();
            }

            foreach (var box in tempArray)
            {
                pool.Release(box);
            }
        }

        public BoostView GetBoost(EBoxBoost grade)
        {
            if (_pools.TryGetValue(grade, out var pool))
            {
                return pool.Get();
            }

            throw new ArgumentException($"No pool found for grade {grade}");
        }

        public void ReturnToPool(BoostView box, EBoxBoost grade)
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

        private void OnGetBoost(BoostView view)
        {
            view.IsInteractable = true;
            view.isDestroyed = false;
        }

        private void OnReleaseBoost(BoostView view)
        {
            view.isDestroyed = true;
            view.IsInteractable = false;
            view.gameObject.SetActive(false);
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
}