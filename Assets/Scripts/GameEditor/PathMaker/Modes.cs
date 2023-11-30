namespace RL.GameEditor
{
    public partial class PathMaker
    {
        [System.Serializable]
        public enum Modes
        {
            None,
            EditingPath,
            BuildsPath,
            EditingCurvesPoints,
        }

        [System.Serializable]
        public enum BuildModes
        {
            ByGrid,
            LockedHeight
        }
    }
}