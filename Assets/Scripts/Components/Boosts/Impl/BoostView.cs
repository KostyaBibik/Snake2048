using System.Collections;
using Components.Boxes.Views.Impl;
using Enums;
using Infrastructure.Pools.Impl;
using Signals;
using UnityEngine;
using Zenject;

namespace Components.Boosts.Impl
{
    public class BoostView : MonoBehaviour, IBoostView
    {
        [SerializeField] private EBoxBoost boostType;
        [SerializeField] private GameObject fxOnDestroy;
        [SerializeField] private GameObject boostFX;

        private SignalBus _signalBus;
        private BoostPool _boostPool;
        
        public bool isDestroyed { get; set; }
        public EBoxBoost BoostType => boostType;
        public bool IsInteractable { get; set; }

        [Inject]
        private void Construct(
            SignalBus signalBus,
            BoostPool pool
            )
        {
            _signalBus = signalBus;
            _boostPool = pool;
        }

        private void OnEnable()
        {
            boostFX.SetActive(true);
            fxOnDestroy.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDestroyed || !IsInteractable)
                return;
            
            if (other.transform.parent && other.transform.parent.TryGetComponent(out BoxView box))
            {
                if(box.isIdle || box.isDestroyed)
                    return;
                
                _signalBus.Fire(new BoxBoostSignal
                {
                    box = box,
                    boostType = BoostType
                });

                IsInteractable = false;
                StartCoroutine(nameof(DeactivateView));
            }
        }

        private IEnumerator DeactivateView()
        {
            boostFX.SetActive(false);
            fxOnDestroy.SetActive(true);
            
            yield return new WaitForSeconds(1f);
            
            _boostPool.ReturnToPool(this, BoostType);
        }
    }
}