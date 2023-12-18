using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace RL.MainMenu
{
    public class GameSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _resolutionsDropdown;
        [SerializeField] private TMP_Dropdown _screenMode;
        private Resolution[] _resolutions;

        private void Start()
        {
            UpdateResolutions();
            UpdateScreenModes();
        }

        void UpdateResolutions()
        {
            _resolutions = Screen.resolutions;

            _resolutionsDropdown.ClearOptions();
            _resolutionsDropdown.AddOptions(_resolutions.Select((res) => res.ToString()).ToList());

            _resolutionsDropdown.SetValueWithoutNotify(Array.IndexOf(_resolutions, Screen.currentResolution));
        }

        void UpdateScreenModes()
        {
            _screenMode.ClearOptions();
            List<string> options = new();

#if UNITY_STANDALONE_WIN
        options.Add("Fullscreen");
        options.Add("Borderless");
        options.Add("Windowed");

        _screenMode.AddOptions(options);

        _screenMode.SetValueWithoutNotify(Screen.fullScreenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => 0,
            FullScreenMode.Windowed => 2,
            _ => 1
        });
#elif UNITY_STANDALONE_OSX
            options.Add("Maximized window");
            options.Add("Borderless");
            options.Add("Windowed");

            _screenMode.AddOptions(options);

            _screenMode.SetValueWithoutNotify(Screen.fullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => 0,
                FullScreenMode.Windowed => 2,
                _ => 1
            });
#else
        options.Add("Fullscreen");

        _screenMode.AddOptions(options);

        _screenMode.SetValueWithoutNotify(0);
#endif

        }

        public void UpdateScreen()
        {
            Resolution resolution = _resolutions[_resolutionsDropdown.value];

#if PLATFORM_STANDALONE_OSX
            Screen.SetResolution(resolution.width, resolution.height, _screenMode.value switch
            {
                0 => FullScreenMode.MaximizedWindow,
                1 => FullScreenMode.FullScreenWindow,
                _ => FullScreenMode.Windowed
            });
#elif UNITY_STANDALONE_WIN
        Screen.SetResolution(resolution.width, resolution.height, _screenMode.value switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.Windowed
        });
#endif
        }
    }
}