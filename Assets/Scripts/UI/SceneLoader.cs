using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RL.UI
{
    public class SceneLoader : MonoBehaviour // TODO: Более красивая загрузка?
    {
        protected static SceneLoader Instance;

        [SerializeField] private TMPro.TMP_Text m_Text;
        private static string Text
        {
            get => Instance.m_Text.text;
            set => Instance.m_Text.text = value;
        }

        private static bool IsActive
        {
            get => Instance.gameObject.activeSelf;
            set => Instance.gameObject.SetActive(value);
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init() => InstanceObject.InstantiatePrefab(nameof(SceneLoader));

        void Awake()
        {
            this.SetInstance(ref Instance);
        }

        public static void LoadScene(int buildIndex)
            => LoadSceneInternal(() => SceneManager.LoadSceneAsync(buildIndex));

        public static void LoadScene(string sceneName)
            => LoadSceneInternal(() => SceneManager.LoadSceneAsync(sceneName));

        private static async void LoadSceneInternal(Func<AsyncOperation> loadScene)
        {
            IsActive = true;

            var scene = loadScene();
            while (!scene.isDone)
            {
                Text = $"Loading… {Mathf.Round(scene.progress * 100)}";
                await Task.Yield();
            }

            IsActive = false;
        }
    }
}