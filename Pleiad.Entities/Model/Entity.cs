namespace Pleiad.Entities.Model
{
    /// <summary>
    /// A container/reference containing Entity ID
    /// </summary>
    public struct Entity
    {
        public Entity(int id)
        {
            Id = id;
        }

        public int Id { get; }
        public override string ToString() => Id.ToString();

        public static implicit operator int(Entity entity) => entity.Id;
        public static Entity operator ++(Entity entity) => new(entity.Id + 1);
        public static Entity operator --(Entity entity) => new(entity.Id - 1);
    }
}
