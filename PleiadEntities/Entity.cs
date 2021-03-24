namespace PleiadEntities
{
    /// <summary>
    /// A container/reference containing Entity ID
    /// </summary>
    public struct Entity
    {
        /// <summary>
        /// A container/reference containing Entity ID
        /// </summary>
        public Entity(int id)
        {
            ID = id;
        }
        /// <summary>
        /// Entity ID
        /// </summary>
        public int ID { get; }
    }
}
