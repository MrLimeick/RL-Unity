using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace RL.MainMenu
{
    [System.Serializable]
    public class ScreenSettings
    {
        public TMPro.TMP_Dropdown Resolution;
        public TMPro.TMP_Dropdown Mode;
    }
    [RequireComponent(typeof(GraphicRaycaster))]
    public class SettingsWindow : MainMenuWindow
    {
        public RectTransform List, Settings;

        public TMPro.TMP_Dropdown Language;
        public ScreenSettings ScreenSettings;

        public void Awake()
        {
            #region ScreenSettings

            ScreenSettings.Mode.onValueChanged.AddListener((arg) =>
            {
                Screen.fullScreenMode = arg switch
                {
                    0 => FullScreenMode.ExclusiveFullScreen,
                    1 => FullScreenMode.FullScreenWindow,
                    _ => FullScreenMode.Windowed,
                };
            });

            List<TMPro.TMP_Dropdown.OptionData> resultions = new();
            foreach (var res in Screen.resolutions)
            {
                resultions.Add(new(res.ToString()));
            }
            ScreenSettings.Resolution.options = resultions;
            ScreenSettings.Resolution.onValueChanged.AddListener((arg) =>
            {
                var resultions = Screen.resolutions;
                Screen.SetResolution(resultions[arg].width,resultions[arg].height,Screen.fullScreenMode,Screen.resolutions[arg].refreshRateRatio);
            });

            #endregion
        }
        /*protected override async Task OpenAnimation()
        {
            await List.SizeAnimAsync(new(100,0), 0.1f, EasingType.QuartIn);
            await Settings.PositionAnimAsync(new(-20, 0), 0.02f, EasingType.None);
            await Settings.SizeAnimAsync(new(500, 0), 0.5f, EasingType.QuartOut);
        }
        protected override async Task CloseAnimation()
        {
            await Settings.SizeAnimAsync(new(0, 0), 0.5f,EasingType.QuartIn);
            await Settings.PositionAnimAsync(new(0, 0), 0.02f, EasingType.None);
            await List.SizeAnimAsync(new(0, 0), 0.1f, EasingType.QuartOut);
        }*/
    }
}
