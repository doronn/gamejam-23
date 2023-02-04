using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamKit.Scripts.Utils.Attributes;
using RootsOfTheGods.Scripts.SingletonsManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RootsOfTheGods.Scripts.DialogueBox
{
    public class DialogueManager : MonoBehaviour, IDialogueManager
    {
        [SerializeField]
        private Image _backgroundOverlay;
        
        [SerializeField]
        private TextMeshProUGUI _text;
        
        [SerializeField]
        private float _fadeDuration = 0.25f;

        private CancellationToken _cancellationToken;

        private bool _isAlreadyVisible;
        private bool _animatingIn;
        private bool _animatingOut;

        private bool _continueClicked = false;

        private static DialogueManager _privateInstance;
        private void Awake()
        {
            if (_privateInstance == null)
            {
                _privateInstance = this;
            }
            else if (this != _privateInstance)
            {
                return;
            }
            
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            SingletonManager.Instance.RegisterInstance(this);
        }

        public async UniTask ShowMessage(string messageText)
        {
            await UniTask.WaitUntil(() => !_animatingIn && !_animatingOut,
                cancellationToken: _cancellationToken);
            _animatingIn = true;
            if (_isAlreadyVisible)
            {
                await DoFade(0, true);
            }
            _text.text = messageText;
            await DoFade(1, _isAlreadyVisible);
            _isAlreadyVisible = true;
            _animatingIn = false;
            _backgroundOverlay.raycastTarget = true;
 
            await UniTask.WaitUntil(() => _continueClicked, cancellationToken: _cancellationToken);
            _continueClicked = false;
            _backgroundOverlay.raycastTarget = false;
        }

        public async UniTask HideMessage()
        {
            _backgroundOverlay.raycastTarget = false;
            await UniTask.WaitUntil(() => !_animatingIn && !_animatingOut,
                cancellationToken: _cancellationToken);

            _animatingOut = true;
            _isAlreadyVisible = false;
            await DoFade(0);
            _text.text = string.Empty;
            _animatingOut = false;
        }

        [Button]
        public void OnContinueClicked()
        {
            _continueClicked = true;
        }

        private async Task DoFade(float fadeInValue, bool onlyText = false)
        {
            if (!onlyText)
            {
                var fadeBackground = _backgroundOverlay.DOFade(fadeInValue, _fadeDuration);
                await UniTask.WaitUntil(() => !fadeBackground.active || fadeBackground.IsComplete(),
                    cancellationToken: _cancellationToken);
            }

            var fadeText = _text.DOFade(fadeInValue, _fadeDuration);
            await UniTask.WaitUntil(() => !fadeText.active || fadeText.IsComplete(), cancellationToken: _cancellationToken);
        }

        private void OnDestroy()
        {
            if (!SingletonManager.IsAvailable())
            {
                return;
            }
            SingletonManager.Instance.UnRegisterInstance(this);
        }
    }

    public interface IDialogueManager
    {
        UniTask ShowMessage(string messageText);
        UniTask HideMessage();
    }
}