namespace RL.Paths
{
    public interface ISyncPoint<TSelf, TPath> : IPathPoint
        where TPath : IPath<TPath, TSelf>
        where TSelf : ISyncPoint<TSelf, TPath>
    {
        public TPath Path { get; set; }
        public int Index { get; set; }

        public TSelf Previous { get; }
        public TSelf Next { get; }
    }
}