using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RootsOfTheGods.Scripts.Fader;
using Scripts.Player.Platformer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RootsOfTheGods.Scripts.Portals
{
    public class PortalsManager : MonoBehaviour, IPortalInteractionManager
    {
        [SerializeField]
        private PortalObject[] _portalObjects;
        
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private PlayerInitializer _playerInitializer;

        private PortalObject _activePortal;
        private static bool _inTransition = false;
        private static PortalProperties _nextPortalProperties = null;
        private void Start()
        {
            for (var i = 0; i < _portalObjects.Length; i++)
            {
                _portalObjects[i].Setup(this);
            }

            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync()
        {
            if (_inTransition && _nextPortalProperties != null)
            {
                var portalObject =
                    _portalObjects.FirstOrDefault(o => o.PortalProperties.PortalId == _nextPortalProperties.PortalId);
                if (portalObject == null)
                {
                    _playerInitializer.InitPlayerDefaultInScene();
                }
                else
                {
                    _playerInitializer.InitPlayerInSpecificPlace(portalObject.transform.position);
                }
                await FadeToBlack.Instance.FadeOut();
            }
            else
            {
                _playerInitializer.InitPlayerDefaultInScene();
                await FadeToBlack.Instance.FadeOut();
            }

            _inTransition = false;
            _nextPortalProperties = null;
        }

        public void InteractWithPortal(InputAction.CallbackContext context)
        {
            if (_inTransition || !_activePortal || !_activePortal.IsPortalInteractive)
            {
                return;
            }
            
            GotToPortalWithProperties(_activePortal.PortalProperties.DestinationPortal).Forget();
        }

        public async UniTaskVoid GotToPortalWithProperties(PortalProperties portalProperties)
        {
            if (SceneManager.GetActiveScene().buildIndex == portalProperties.PortalScene)
            {
                var portalObject =
                    _portalObjects.FirstOrDefault(o => o.PortalProperties.PortalId == portalProperties.PortalId);
                if (portalObject == null)
                {
                    return;
                }

                _inTransition = true;
                _playerController.StopPlayer();
                await FadeToBlack.Instance.FadeIn();
                _playerController.SetNextPosition(portalObject.transform.localPosition);
                await FadeToBlack.Instance.FadeOut();
                _playerController.ConnectController();
                _inTransition = false;
                return;
            }
            
            _inTransition = true;
            _nextPortalProperties = portalProperties;
            _playerController.StopPlayer();
            await FadeToBlack.Instance.FadeIn();
            await SceneManager.LoadSceneAsync(portalProperties.PortalScene);
        }

        public void SetPortalAsActive(PortalObject portalObject)
        {
            _activePortal = portalObject;
        }
    }

    public interface IPortalInteractionManager
    {
        void SetPortalAsActive(PortalObject portalObject);
    }
}