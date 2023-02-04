using RootsOfTheGods.Scripts.Interactables;
using UnityEngine;

namespace RootsOfTheGods.Scripts.Portals
{
    public class PortalObject : BaseInteractable
    {
        [field: SerializeField]
        public PortalProperties PortalProperties { get; private set; }

        public override void Interact()
        {
            _portalInteractionManager.GotToPortalWithProperties(PortalProperties.DestinationPortal).Forget();
        }

        public void Setup(IPortalInteractionManager portalInteractionManager)
        {
            _portalInteractionManager = portalInteractionManager;
        }
    }

    public interface IInteractable
    {
        bool IsInteractive { get; }
        void Interact();
    }
}