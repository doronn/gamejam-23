using System;
using Cysharp.Threading.Tasks;
using GameJamKit.Scripts.Utils.Singleton;
using RootsOfTheGods.Scripts.DialogueBox;
using RootsOfTheGods.Scripts.Fader;
using UnityEngine;

namespace RootsOfTheGods.Scripts.SingletonsManager
{
    public class SingletonManager : Singleton<SingletonManager>, IInstanceSetter<object>
    {
        public IFadeToBlack FadeToBlackInstance { get; private set; } = new FadeToBlackInstanceImplementation();
        public IDialogueManager DialogueMamagerInstance { get; private set; } = new DialogueManagerBaseInstanceImplementation();
        
        public void RegisterInstance(object instanceToRegister)
        {
            if (instanceToRegister == null)
            {
                Debug.LogError("Do not register a null object. Use UnRegister instead with the original instance or provide a new instance");
            }
            switch (instanceToRegister)
            {
                case IFadeToBlack instanceOfType:
                    (FadeToBlackInstance as BaseInstanceSetter<IFadeToBlack>)?.RegisterInstance(instanceOfType);
                    break;
                case IDialogueManager instanceOfType:
                    (DialogueMamagerInstance as BaseInstanceSetter<IDialogueManager>)?.RegisterInstance(instanceOfType);
                    break;
            }
        }

        public bool UnRegisterInstance(object instanceToUnRegister)
        {
            var result = false;
            switch (instanceToUnRegister)
            {
                case IFadeToBlack instanceOfType:
                    result = (FadeToBlackInstance as BaseInstanceSetter<IFadeToBlack>)?.UnRegisterInstance(instanceOfType) ??
                             false;
                    break;
                case IDialogueManager instanceOfType:
                    result = (DialogueMamagerInstance as BaseInstanceSetter<IDialogueManager>)?.UnRegisterInstance(instanceOfType) ??
                             false;
                    break;
            }

            if (result == false)
            {
                Debug.LogError("Tried to unregister an item that was not registered before");
            }
            
            return result;
        }

        public void ResetSingletons()
        {
            (FadeToBlackInstance as BaseInstanceSetter<IFadeToBlack>)?.RegisterInstance(null);
            (DialogueMamagerInstance as BaseInstanceSetter<IDialogueManager>)?.RegisterInstance(null);
        }
    }

    internal interface IInstanceSetter<in T>
    {
        void RegisterInstance(T instanceToRegister);
        bool UnRegisterInstance(T instanceToUnRegister);

    }

    internal abstract class BaseInstanceSetter<T> where T : class
    {
        protected T _instance;
        public void RegisterInstance(T instanceToRegister)
        {
            _instance = instanceToRegister;
        }
        
        public bool UnRegisterInstance(T instanceToRegister)
        {
            if (_instance == null)
            {
                return true;
            }
            if (Equals(_instance, instanceToRegister))
            {
                _instance = null;
                return true;
            }

            return false;
        }
    }
    internal class FadeToBlackInstanceImplementation : BaseInstanceSetter<IFadeToBlack>, IFadeToBlack
    {
        public async UniTask FadeIn()
        {
            if (_instance == null)
            {
                return;
            }
            await _instance.FadeIn();
        }

        public async UniTask FadeOut()
        {
            if (_instance == null)
            {
                return;
            }
            await _instance.FadeOut();
        }
    }
    internal class DialogueManagerBaseInstanceImplementation : BaseInstanceSetter<IDialogueManager>, IDialogueManager 
    {
        public async UniTask ShowMessage(string messageText)
        {
            if (_instance == null)
            {
                return;
            }

            await _instance.ShowMessage(messageText);
        }

        public async UniTask HideMessage()
        {
            if (_instance == null)
            {
                return;
            }

            await _instance.HideMessage();
        }
    }

}