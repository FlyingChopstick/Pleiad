namespace Pleiad.Common.Model
{
    public class ValueClass<T>
    {
        public T Value { get; init; }

        public static implicit operator T(ValueClass<T> val) => val.Value;
    }
}
