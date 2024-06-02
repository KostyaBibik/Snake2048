﻿using DG.Tweening;
using Enums;
using GameUtilities.CoroutineHelper;
using Services;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Components.Boxes.Views.Impl
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoxView : MonoBehaviour, IBoxView
    {
        public BoxContext stateContext;
        
        [SerializeField] private EBoxGrade grade;
        [SerializeField] private TextMeshProUGUI nickText;
        [SerializeField] private Slider accelerationSlider;
        [SerializeField] private Transform meshTransform;

        [Header("Collider")] 
        [SerializeField] private Collider triggerCollider;
        [SerializeField] private Collider physicCollider;

        private Rigidbody _rigidbody;
        private Vector3 _originalScale;
        private Tween _scaleTween;
        private SignalBus _signalBus;
        private GameMatchService _gameMatchService;
        private CoroutineSequence _sliderSequence;
        
        public EBoxGrade Grade => grade;
        public Rigidbody Rigidbody => _rigidbody;
        
        public bool IsAccelerationActive  { get; set; }
        public bool IsSpeedBoosted  { get; set; }
        public bool isMerging { get; set; }

        public bool isDestroyed { get; set; }

        public bool isPlayer { get; set; }

        public bool isIdle { get; set; }

        public bool isBot { get; set; }
        public float meshOffset { get; set; }
        
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
            _sliderSequence = new CoroutineSequence();
            _rigidbody = GetComponent<Rigidbody>();
            _originalScale = meshTransform.localScale;
            
            var mesh = meshTransform.transform;
            meshOffset = (_originalScale.y - 1);
            var forcedPos = mesh.position + new Vector3(0f, meshOffset / 2, 0f);

            nickText.transform.position += new Vector3(0f, meshOffset, 0f);
            accelerationSlider.transform.position += new Vector3(0f, meshOffset, 0f);
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
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

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
            nickText.gameObject.SetActive(activeText);
            nickText.text = nick;
        }

        public void UpdateAccelerationSliderValue(float value)
        {
            accelerationSlider.value = value;
        }
        
        public void UpdateAccelerationSliderStatus(float value, bool isEnabled = true)
        {
            accelerationSlider.value = value;
            if(accelerationSlider.gameObject.activeSelf != isEnabled)
            {
                if(_sliderSequence.IsRunning)
                    _sliderSequence.End();

                if (isEnabled)
                {
                    _sliderSequence.Run(
                        TweenBuilder.SetActive(accelerationSlider.gameObject, true),
                        UITweens.FadeInUnscaled(UIConstantDictionary.Values.DefaultFadeDuration, accelerationSlider.gameObject));
                }
                else
                {
                    _sliderSequence.Run(
                        UITweens.FadeOutUnscaled(UIConstantDictionary.Values.DefaultFadeDuration, accelerationSlider.gameObject),
                        TweenBuilder.SetActive(accelerationSlider.gameObject, false));
                }
            }
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