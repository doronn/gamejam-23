using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using RootsOfTheGods.Scripts.Collectibles;
using RootsOfTheGods.Scripts.DialogueBox;
using RootsOfTheGods.Scripts.Fader;
using RootsOfTheGods.Scripts.SingletonsManager;
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
        private Collectible[] _collectibles;
        
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private PlayerInitializer _playerInitializer;
        [SerializeField]
        private CollectiblesConfigurations _collectiblesConfigurations;

        private IInteractable _activeInteractable;
        private static bool _inTransition = false;
        private static bool _firstTimeInAPortal = true;
        private static bool _isFirstLoad = true;
        private static PortalProperties _nextPortalProperties = null;
        private static CollectiblesConfigurations _currentCollectiblesState = null;
        private IFadeToBlack _fadeToBlackInstance;
        private IDialogueManager _dialogueManagerInstance;

        private void Awake()
        {
            _fadeToBlackInstance = SingletonManager.Instance.FadeToBlackInstance;
            _dialogueManagerInstance = SingletonManager.Instance.DialogueMamagerInstance;
        }

        private void Start()
        {
            for (var i = 0; i < _portalObjects.Length; i++)
            {
                _portalObjects[i].Setup(this);
            }

            InitAsync().Forget();

            if (_currentCollectiblesState == null)
            {
                _currentCollectiblesState = Instantiate(_collectiblesConfigurations);
            }

            for (var i = 0; i < _collectibles?.Length; i++)
            {
                var collectible = _collectibles[i];
                if (!_currentCollectiblesState.Collectibles.Collectibles.ContainsKey(collectible.CollectibleType))
                {
                    Destroy(collectible.gameObject);
                }
                else
                {
                    collectible.Setup(this);
                }
            }

            _collectibles = null;
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

                if (_firstTimeInAPortal)
                {
                    await _dialogueManagerInstance.ShowMessage("Wow! A portal! Where am I going?");
                }

                await _fadeToBlackInstance.FadeOut();
                
                if (_firstTimeInAPortal)
                {
                    _firstTimeInAPortal = false;
                    await _dialogueManagerInstance.ShowMessage("I'm in another dimension!!! Oh my god...");
                }
                
                await _dialogueManagerInstance.HideMessage();
            }
            else
            {
                if (_isFirstLoad)
                {
                    _isFirstLoad = false;
                    var startGameTexts = _collectiblesConfigurations.StartGameTexts;
                    for (var i = 0; i < startGameTexts.Length; i++)
                    {
                        await _dialogueManagerInstance.ShowMessage(startGameTexts[i]);
                    }
                    await _dialogueManagerInstance.HideMessage();

                }
                _playerInitializer.InitPlayerDefaultInScene();
                await _fadeToBlackInstance.FadeOut();
            }

            _inTransition = false;
            _nextPortalProperties = null;
        }

        public void InteractWithPortal(InputAction.CallbackContext context)
        {
            if (_inTransition || _activeInteractable is not { IsInteractive: true })
            {
                return;
            }
            
            _activeInteractable.Interact();
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
                await _fadeToBlackInstance.FadeIn();
                _playerController.SetNextPosition(portalObject.transform.localPosition);
                await _fadeToBlackInstance.FadeOut();
                _playerController.ConnectController();
                _inTransition = false;
                return;
            }
            
            _inTransition = true;
            _nextPortalProperties = portalProperties;
            _playerController.StopPlayer();
            await _fadeToBlackInstance.FadeIn();
            await SceneManager.LoadSceneAsync(portalProperties.PortalScene);
        }

        public async UniTask CollectObject(CollectibleType collectibleType)
        {
            _inTransition = true;
            if (_currentCollectiblesState.Collectibles.Collectibles.ContainsKey(collectibleType))
            {
                var collectibleData = _currentCollectiblesState.Collectibles.Collectibles[collectibleType];
                _currentCollectiblesState.Collectibles.Collectibles.Remove(collectibleType);

                _playerController.StopPlayer();
                await _dialogueManagerInstance.ShowMessage(collectibleData.TextToComment);
                await _dialogueManagerInstance.HideMessage();
                _playerController.ConnectController();
            }

            if (_currentCollectiblesState.Collectibles.Collectibles.Count == 0)
            {
                await _dialogueManagerInstance.ShowMessage(_currentCollectiblesState.EndGameText);
                await _dialogueManagerInstance.ShowMessage(_currentCollectiblesState.EndGameTextBeforeDark);
                await _fadeToBlackInstance.FadeIn();
                await _dialogueManagerInstance.ShowMessage(_currentCollectiblesState.EndGameTextAfterDark);
                await _dialogueManagerInstance.HideMessage();
                SingletonManager.Instance.FadeToBlackInstance.FadeOut().Forget();
                _currentCollectiblesState = null;
                _isFirstLoad = true;
                await SceneManager.LoadSceneAsync(0);
            }
            
            _inTransition = false;
        }

        public void SetPortalAsActive(IInteractable portalObject)
        {
            _activeInteractable = portalObject;
        }
    }

    public interface IPortalInteractionManager
    {
        void SetPortalAsActive(IInteractable portalObject);
        UniTaskVoid GotToPortalWithProperties(PortalProperties portalPropertiesDestinationPortal);
        UniTask CollectObject(CollectibleType collectibleType);
    }
}