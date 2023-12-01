using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RL.MainMenu;
using UnityEngine.UI;
using RL.UI;
using System.IO;

namespace RL.MainMenu.MapList
{
    [RequireComponent(typeof(RectTransform))]
    public class MapListWindow : MainMenuWindow
    {
        [Serializable]
        public sealed class MapPreview
        {
            [Serializable]
            public sealed class Tools
            {
                public RectTransform RT;
                public UnityEngine.UI.Button Play;
            }
            public Tools tools = new();
        }
        public MapPreview mapPreview;
        public RectTransform Graphic;
        public void Start()
        {
            UpdateMapList();
        }
        private void OnValidate()
        {
            Debug.Log(mapPreview.tools.Play.transform.position);
            Graphic = GetComponent<RectTransform>();
        }
        /*protected override async Task OpenAnimation()
        {
            await Graphic.AnchorMinAnimAsync(new Vector2(0,0),0.5f);
            mapPreview.tools.RT.PositionAnim(new Vector2(0,20),0.25f);
        }
        protected override async Task CloseAnimation()
        {
            await mapPreview.tools.RT.PositionAnimAsync(new Vector2(0,-120),0.25f);
            await Graphic.AnchorMinAnimAsync(new Vector2(1, 0), 0.5f);
        }*/
        #region Map List

        [Header("Map list")]
        [SerializeField] private MapListButton Button;
        [SerializeField] private RectTransform Template;

        [SerializeField] private RectTransform ListContent;

        /*public List<string> MapNames = new()
        {
            "Hello!","First test music!..","...and second test music?","Ok"
        };
        public List<string> ArtistNames = new()
        {
            "Hello!","First test music!..","...and second test music?","Ok"
        };
        public List<Sprite> Images = new();*/
        public async void UpdateMapList()
        {
            List<Card> rlms = new(); 
            foreach(DirectoryInfo DI in CreatedMapsDirectory.GetDirectories())
            {
                foreach(FileInfo FI in DI.GetFiles())
                {
                    if(FI.Extension == ".RLM")
                    {
                        rlms.Add(JsonUtility.FromJson<Card>(File.ReadAllText(FI.FullName)));
                    }
                }
            }
            Debug.Log("Yep");
            RectTransform ButtonRT = Button.GetComponent<RectTransform>();
            var Height = Template.sizeDelta.y;
            var ButtonHeight = ButtonRT.sizeDelta.y;

            var Padding = ButtonRT.anchoredPosition;

            List<Card> ClearingRlms = new();

            for (int i = 0; i < rlms.Count; i++)
            {
                if (rlms[i] != null)
                {
                    ClearingRlms.Add(rlms[i]);
                }
            }
            ListContent.sizeDelta = new Vector2(Padding.x, (-ButtonHeight * ClearingRlms.Count) + (Padding.y * ClearingRlms.Count + Padding.y)) * -1;
            for (int i = 0; i < ClearingRlms.Count; i++)
            {
                Debug.Log("Yep");
                string MapName = ClearingRlms[i].Name;
                string ArtistName = ClearingRlms[i].Artits;

                MapListButton MLB = Instantiate(Button.gameObject, ListContent).GetComponent<MapListButton>();

                if (ClearingRlms[i].BackgroundFile != null && ClearingRlms[i].BackgroundFile.Exists)
                {
                    var image = await RLIO.GetSpriteFromPathAsync(ClearingRlms[i].BackgroundFile.FullName);
                    MLB.BackgroundImage.sprite = image;
                }

                MLB.GetComponent<RectTransform>().anchoredPosition = new(Padding.x, (-ButtonHeight * i) + (Padding.y * i + Padding.y));
                MLB.MapName.text = MapName;
                MLB.ArtistName.text = ArtistName;
                MLB.map = ClearingRlms[i];
            }
        }
        //public void OnRectTransformDimensionsChange() => UpdateMapList();
        #endregion
    }
}
