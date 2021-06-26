namespace PleiadEntities.Model
{
    /// <summary>
    /// A container/reference containing Entity ID
    /// </summary>
    public struct Entity
    {
        public Entity(int id)
        {
            ID = id;
        }

        public int ID { get; }

        public static implicit operator int(Entity entity) => entity.ID;
        public static Entity operator ++(Entity entity) => new(entity.ID + 1);
        public static Entity operator --(Entity entity) => new(entity.ID - 1);
    }
}
