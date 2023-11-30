using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RL.GameEditor
{
    public class Page : MonoBehaviour
    {
        public static Scene? LoadedScene = null;

        [System.Serializable]
        public class AdditionalWindows
        {
            public Window[] Windows;
            public bool AllClose = false;
            public void CloseAll() => CloseAllAsync();
            public void CloseAllAsync()
            {
                foreach (var window in Windows)
                {
                    window.CloseAsync();
                }
            }
            public void OpenAll() => OpenAllAsync();
            public void OpenAllAsync()
            {
                foreach (var window in Windows)
                {
                    window.OpenAsync();
                }
            }

            public void OpenCloseAllToggle()
            {
                AllClose = !AllClose;
                if (AllClose) CloseAll();
                else OpenAll();
            }
        }
        public AdditionalWindows additionalWindows;
        protected virtual Task OpenAnimation(CancellationToken ct) => Task.CompletedTask;
        protected virtual Task CloseAnimation(CancellationToken ct) => Task.CompletedTask;
    }
}