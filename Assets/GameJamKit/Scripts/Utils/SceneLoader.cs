using Cysharp.Threading.Tasks;
using RootsOfTheGods.Scripts.Fader;
using RootsOfTheGods.Scripts.SingletonsManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJamKit.Scripts.Utils
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            LoadSceneWithFade(sceneName).Forget();
        }

        private async UniTaskVoid LoadSceneWithFade(string sceneName)
        {
            await SingletonManager.Instance.FadeToBlackInstance.FadeIn();
            SceneManager.LoadScene(sceneName);
        }
    }
}