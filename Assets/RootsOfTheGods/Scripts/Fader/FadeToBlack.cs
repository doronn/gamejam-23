using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamKit.Scripts.Utils.Singleton;
using RootsOfTheGods.Scripts.SingletonsManager;
using UnityEngine;
using UnityEngine.UI;

namespace RootsOfTheGods.Scripts.Fader
{
    public class FadeToBlack : MonoBehaviour, IFadeToBlack
    {
        [SerializeField]
        private Image BlackOverlay;

        private CancellationToken _cancellationToken;

        private static FadeToBlack _privateInstance;
        private void Awake()
        {
            if (_privateInstance == null)
            {
                _privateInstance = this;
            }
            else if (this != _privateInstance)
            {
                Destroy(gameObject);
                return;
            }
            
            if (BlackOverlay == null)
            {
                BlackOverlay = GetComponentInChildren<Image>();
            }
            
            DontDestroyOnLoad(gameObject);
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            SingletonManager.Instance.RegisterInstance(this);
        }

        public async UniTask FadeIn()
        {
            if (BlackOverlay == null)
            {
                Debug.LogError("There is no fader in the scene, you might have forgotten to add it. skipping fade");
                return;
            }
            var fade = BlackOverlay.DOFade(1f, 0.5f);
            await UniTask.WaitUntil(() => !fade.active || fade.IsComplete(), cancellationToken: _cancellationToken);
        }
        
        public async UniTask FadeOut()
        {
            if (BlackOverlay == null)
            {
                Debug.LogError("There is no fader in the scene, you might have forgotten to add it. skipping fade");
                return;
            }
            var fade = BlackOverlay.DOFade(0f, 0.5f);
            await UniTask.WaitUntil(() => !fade.active || fade.IsComplete(), cancellationToken: _cancellationToken);
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

    public interface IFadeToBlack
    {
        UniTask FadeIn();
        UniTask FadeOut();
    }
}