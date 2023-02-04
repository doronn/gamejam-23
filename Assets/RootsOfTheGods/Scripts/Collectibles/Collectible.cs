using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RootsOfTheGods.Scripts.Interactables;
using RootsOfTheGods.Scripts.Portals;
using UnityEngine;

namespace RootsOfTheGods.Scripts.Collectibles
{
    public class Collectible : BaseInteractable
    {
        [field: SerializeField]
        public CollectibleType CollectibleType { get; private set; }

        private CancellationToken _cancellationToken;

        private void Start()
        {
            _cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        public override void Interact()
        {
            Collect().Forget();
        }
        
        public void Setup(IPortalInteractionManager portalInteractionManager)
        {
            _portalInteractionManager = portalInteractionManager;
        }

        private async UniTaskVoid Collect()
        {
            var scaleTween = transform.DOScale(transform.localScale * 3, 0.5f);
            await _portalInteractionManager.CollectObject(CollectibleType);
            await UniTask.WaitUntil(() => !scaleTween.active || scaleTween.IsComplete(),
                cancellationToken: _cancellationToken);
            
            Destroy(gameObject);
        }
    }
}