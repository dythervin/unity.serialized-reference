using System.Data.SqlTypes;
using UnityEngine;

namespace Dythervin.SerializedReference.Refs
{
    [System.Serializable]
    public struct ObjPtr<T> : INullable, ISerializationCallbackReceiver, IRef
        where T : Object
    {
        [SerializeField] private T value;

        [HideInInspector, SerializeField] private bool notNull;

        public ObjPtr(T value) : this()
        {
            Value = value;
        }

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                notNull = value;
            }
        }

        public readonly bool HasValue => notNull;
        public bool IsNull => !notNull;

        public readonly bool TryGet(out T value)
        {
            value = this.value;
            return notNull;
        }

        public void OnBeforeSerialize()
        {
            notNull = value != null;
        }

        public void OnAfterDeserialize() { }

        public static implicit operator T(ObjPtr<T> a)
        {
            return a.value;
        }

        public static implicit operator ObjPtr<T>(T value)
        {
            return new ObjPtr<T>(value);
        }

        public static implicit operator bool(ObjPtr<T> objPtr)
        {
            return objPtr.notNull;
        }

        Object IRef.Obj
        {
            get => value;
            set => this.value = (T)value;
        }
    }
}