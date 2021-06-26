namespace PleiadEntities.Model
{
    public struct ChunkIndex
    {
        public ChunkIndex(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public static implicit operator int(ChunkIndex chunkId) => chunkId.Index;

        public static ChunkIndex operator ++(ChunkIndex index) => new(index.Index + 1);
        public static ChunkIndex operator --(ChunkIndex index) => new(index.Index - 1);
    }
}
