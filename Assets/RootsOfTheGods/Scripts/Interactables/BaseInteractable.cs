using RootsOfTheGods.Scripts.Portals;
using UnityEngine;

namespace RootsOfTheGods.Scripts.Interactables
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public bool IsInteractive { get; protected set; }

        [SerializeField]
        private LayerMask _interactableLayers;

        protected IPortalInteractionManager _portalInteractionManager;

        private void OnTriggerEnter(Collider other)
        {
            if ((_interactableLayers & (1 << other.gameObject.layer)) != 0)
            {
                IsInteractive = true;
                _portalInteractionManager.SetPortalAsActive(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_interactableLayers & (1 << other.gameObject.layer)) != 0)
            {
                IsInteractive = false;
            }
        }

        public abstract void Interact();
    }
}