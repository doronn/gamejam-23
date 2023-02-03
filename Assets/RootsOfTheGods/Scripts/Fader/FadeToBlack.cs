using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamKit.Scripts.Utils.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace RootsOfTheGods.Scripts.Fader
{
    public class FadeToBlack : Singleton<FadeToBlack>
    {
        [SerializeField]
        private Image BlackOverlay;

        private CancellationToken _cancellationToken;

        private void Awake()
        {
            if (BlackOverlay == null)
            {
                BlackOverlay = GetComponentInChildren<Image>();
            }
            
            DontDestroyOnLoad(this);
            _cancellationToken = this.GetCancellationTokenOnDestroy();
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
    }
}