using UnityEngine;
using UnityEngine.SceneManagement;

namespace RootsOfTheGods.Scripts.Portals
{
    [CreateAssetMenu(fileName = "PortalProperties", menuName = "Portals/New Portal Properties", order = 0)]
    public class PortalProperties : ScriptableObject
    {
        public int PortalId;
        public int PortalScene;
        public PortalProperties DestinationPortal;
    }
}