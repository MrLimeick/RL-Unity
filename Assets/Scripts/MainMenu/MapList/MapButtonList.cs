using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RL.MainMenu.MapList
{
    internal class MapButtonList : MonoBehaviour
    {
        [System.Serializable]
        public class ButtonOfList
        {
            public UnityEngine.UI.Button Button;
            public TMPro.TMP_Text Text;
            public Image PreviewImage;
            public string Name;
            public string Artist;
            public Sprite Background;
        }
        public RectTransform UpButton;
        public RectTransform DownButton;


    }
}
