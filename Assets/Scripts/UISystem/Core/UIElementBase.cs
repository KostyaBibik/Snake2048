using System;
using GameUtilities.CoroutineHelper;
using GameUtilities.UI;
using UnityEngine;

namespace UISystem
{
    public abstract class UIElementBase : MonoBehaviour
    {
        public bool IsShow =>
            gameObject.activeInHierarchy &&
            (_showSequence == null || !_showSequence.IsRunning) &&
            (_hideSequence == null || !_hideSequence.IsRunning);

        ///<summary>
        /// Указывает является ли элемент перманентным.
        /// Перманентные элементы не учитываются при 
        /// поиске активных UI элементов в момент открытия меню,
        /// что позволяет открыть меню без деактивации данного элемента.
        ///</summary>
        [field:NonSerialized]
        public virtual bool IsPermanent { get; set; } = false;

        private CoroutineSequence _showSequence;
        private CoroutineSequence _hideSequence;

        public CoroutineSequence BeginShow(bool useExternalEffect = false)
        {
            ThrowIfPermanent();
            InitializeSequences();

            _showSequence.End();
            _hideSequence.End();
            
            OnShowBegin();

            if (!useExternalEffect)
            {
                PlayShowEffect(_showSequence);
            }
            
            if(!_showSequence.IsRunning)
            {
                OnShowEnd();
            }

            return _showSequence;
        }

        public CoroutineSequence BeginHide(bool useExternalEffect = false)
        {
            ThrowIfPermanent();
            InitializeSequences();

            _showSequence.End();
            _hideSequence.End();

            OnHideBegin();

            if (!useExternalEffect)
            {
                PlayHideEffect(_hideSequence);
            }

            if(!_hideSequence.IsRunning)
            {
                OnHideEnd();
            }

            return _hideSequence;
        }

        public virtual void Initialization()
        {
        }

        protected virtual void PlayShowEffect(CoroutineSequence sequence)
        {
            sequence.Run(
                TweenBuilder.SetActive(gameObject, true),
                UITweens.FadeInUnscaled(UIConstantDictionary.Values.DefaultFadeDuration, gameObject));
        }

        protected virtual void PlayHideEffect(CoroutineSequence sequence)
        {
            sequence.Run(
                UITweens.FadeOutUnscaled(UIConstantDictionary.Values.DefaultFadeDuration, gameObject),
                TweenBuilder.SetActive(gameObject, false));
        }

        #region Callbacks
        protected virtual void OnShowBegin() { }

        protected virtual void OnHideBegin() { }

        protected virtual void OnShowEnd() { }

        protected virtual void OnHideEnd() { }
        #endregion
        
        private void InitializeSequences()
        {
            if(_showSequence == null)
            {
                _showSequence = new CoroutineSequence(OnShowEnd);
            }

            if(_hideSequence == null)
            {
                _hideSequence = new CoroutineSequence(OnHideEnd);
            }
        }

        private void ThrowIfPermanent()
        {
            if (IsPermanent)
            {
                throw new PermanentUIElementException();
            }
        }
    }
}