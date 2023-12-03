namespace RL.CardEditor
{
    public partial class PathMaker
    {
        [System.Serializable]
        public enum Modes
        {
            None,
            EditingPath,
            BuildsPath,
            EditingControlPoints,
        }

        [System.Serializable]
        public enum BuildModes
        {
            ByGrid,
            LockedHeight
        }
    }
}