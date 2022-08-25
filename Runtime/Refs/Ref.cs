using System;
using Dythervin.Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dythervin.SerializedReference.Refs
{
    [Serializable]
    public struct Ref<T> : ISerializationCallbackReceiver, IRef
        where T : class
    {
        [SerializeField] private Object value;

        [NonSerialized] public T Value;

        public Object Obj
        {
            get => value;
            set
            {
                switch (value)
                {
                    case GameObject gameObject:
                    {
                        this.value = gameObject.TryGetComponent(out T component) ? component as Object : null;
                        break;
                    }
                    case T component:
                        this.value = component as Object;
                        break;
                    default:
                        this.value = null;
                        break;
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR && !ODIN_INSPECTOR
            if (value)
                Obj = value;
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Value = value as T;
        }

        public Ref(T value)
        {
            this.value = value as Object;
            Value = value;
        }

        private bool Validate(Object obj)
        {
            switch (obj)
            {
                case GameObject gameObject:
                {
                    if (!gameObject.TryGetComponent(out T component))
                        return false;

                    value = component as Object;
                    return true;
                }
                default:
                    return obj is T;
            }
        }

        public static implicit operator T(Ref<T> a)
        {
            return a.Value;
        }

        public static implicit operator Ref<T>(Object obj)
        {
            return new Ref<T> { Obj = obj };
        }

        public static implicit operator Ref<T>(T obj)
        {
            return new Ref<T>(obj);
        }
    }

#if UNITY_EDITOR
    internal static class Ref
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnCompile()
        {
            Symbols.AddSymbol("DYTHERVIN_REF");
        }
    }
#endif
}