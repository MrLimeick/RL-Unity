using RL;
using RL.CardEditor;
using RL.UI;
using Game = RL.Game.GameController;
using Image = UnityEngine.UI.Image;
using Text = TMPro.TMP_Text;

public class MapListButton : UnityEngine.MonoBehaviour
{
    public Image BackgroundImage;
    public Text MapName;
    public Text ArtistName;

    public Button Play, Edit;
    public Card map;

    public void Awake()
    {
        //Play.OnClick += async () =>
        //{
        //    Game.map = map;
        //    await SceneLoader.LoadSceneAsync("Game");
        //};

        //Edit.OnClick += async () =>
        //{
        //    EditorController.Map = map;
        //    await SceneLoader.LoadSceneAsync("GameEditor");
        //};
    }
}
