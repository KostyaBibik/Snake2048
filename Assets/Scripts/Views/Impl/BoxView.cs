using Components.Boxes;
using DG.Tweening;
using Enums;
using Services;
using Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views.Impl
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoxView : MonoBehaviour, IEntityView
    {
        public BoxContext stateContext;
        
        [SerializeField] private EBoxGrade grade;
        [SerializeField] private TextMeshProUGUI _nickText;
        [SerializeField] private Transform meshTransform;

        [Header("Collider")] 
        [SerializeField] private Collider triggerCollider;
        [SerializeField] private Collider physicCollider;

        private Rigidbody _rigidbody;
        private Vector3 _originalScale;
        private Tween _scaleTween;
        private SignalBus _signalBus;
        private GameMatchService _gameMatchService;
        private string _nick;
        
        public EBoxGrade Grade => grade;
        public Rigidbody Rigidbody => _rigidbody;
        public string Nick => _nick;
        public bool isMerging { get; set; }

        public bool isDestroyed { get; set; }

        public bool isPlayer { get; set; }

        public bool isIdle { get; set; }

        public bool isBot { get; set; }

        [Inject]
        private void Construct(
            SignalBus signalBus,
            GameMatchService gameMatchService
        )
        {
            _signalBus = signalBus;
            _gameMatchService = gameMatchService;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _originalScale = meshTransform.localScale;
            
            var mesh = meshTransform.transform;
            var forcedPos = mesh.position + new Vector3(0f, (_originalScale.y - 1) / 2, 0f);
            
            mesh.position = forcedPos;
            UpdateTransform(triggerCollider.transform, forcedPos, _originalScale);
            UpdateTransform(physicCollider.transform, forcedPos, _originalScale);
            
        }

        private void UpdateTransform(Transform targetTransform, Vector3 position, Vector3 scale)
        {
            targetTransform.position = position;
            targetTransform.localScale = scale;
        }
        private void FixedUpdate()
        {
            if(isDestroyed)
                return;
            
            if(_gameMatchService.EGameModeStatus != EGameModeStatus.Play)
                return;

            stateContext?.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isIdle)
                return;

            if (isDestroyed)
                return;

            if (other.transform.parent && other.transform.parent.TryGetComponent(out BoxView enemyBox))
            {
                _signalBus.Fire(new EatBoxSignal
                {
                    eatenBox = enemyBox,
                    newOwner = this
                });
            }
            else if (other.CompareTag("Wall"))
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        public void SetNickname(string nick, bool activeText = true)
        {
            _nick = nick;
            _nickText.gameObject.SetActive(activeText);
            _nickText.text = nick;
        }

        public void AnimateUpscale(float delay)
        {
            if (isIdle)
                return;
            
            DeactivateUpScaleAnim();

            _scaleTween = DOTween.Sequence()
                .PrependInterval(delay)
                .Append(meshTransform.DOScale(_originalScale + new Vector3(.25f, .25f, .25f), .15f))
                .Append(meshTransform.DOScale(_originalScale, .15f))
                .Play();
        }

        public void DisableNick()
        {
            SetNickname(string.Empty, false);
        }

        private void DeactivateUpScaleAnim()
        {
            _scaleTween?.Kill();
            meshTransform.localScale = _originalScale;
        }
        
        private void OnDisable()
        {
            DeactivateUpScaleAnim();
            DisableNick();
        }
    }
}