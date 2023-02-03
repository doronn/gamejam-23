using Cysharp.Threading.Tasks;
using RootsOfTheGods.Scripts.Fader;
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
            await FadeToBlack.Instance.FadeIn();
            SceneManager.LoadScene(sceneName);
        }
    }
}