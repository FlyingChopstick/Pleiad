using System;

namespace Pleiad.Misc.Model
{
    public struct TypeHash
    {
        public TypeHash(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException("Type was null");
            }

            _valueInternal = type.GetHashCode();
        }

        private int _valueInternal;
        public int Value
        {
            get
            {
                if (_valueInternal == default)
                {
                    throw new ArgumentException($"Value of {nameof(TypeHash)} cannot be default");
                }

                return _valueInternal;
            }
        }


        public static bool operator ==(TypeHash l, TypeHash r)
        {
            return l._valueInternal == r._valueInternal;
        }
        public static bool operator !=(TypeHash l, TypeHash r)
        {
            return !(l == r);
        }
        public override bool Equals(object obj)
        {
            return obj is TypeHash hash &&
                   _valueInternal == hash._valueInternal;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_valueInternal);
        }
    }
}
