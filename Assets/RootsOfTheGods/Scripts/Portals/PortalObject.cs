using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RootsOfTheGods.Scripts.Portals
{
    public class PortalObject : MonoBehaviour
    {
        public bool IsPortalInteractive;
        [field: SerializeField]
        public PortalProperties PortalProperties { get; private set; }
        
        [SerializeField]
        private LayerMask _interactableLayers;

        private IPortalInteractionManager _portalInteractionManager;

        private void OnTriggerEnter(Collider other)
        {
            if ((_interactableLayers & (1 << other.gameObject.layer)) != 0)
            {
                IsPortalInteractive = true;
                _portalInteractionManager.SetPortalAsActive(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_interactableLayers & (1 << other.gameObject.layer)) != 0)
            {
                IsPortalInteractive = false;
            }
        }

        public void Setup(IPortalInteractionManager portalInteractionManager)
        {
            _portalInteractionManager = portalInteractionManager;
        }
    }
}