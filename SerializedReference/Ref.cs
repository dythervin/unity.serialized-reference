using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dythervin.SerializedReference
{
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    public interface IRef
    {
        public Object Obj { get; set; }
    }

    [Serializable]
#if ODIN_INSPECTOR
    [HideLabel]
#endif
    public struct Ref<T> : ISerializationCallbackReceiver, IRef
        where T : class
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [GUIColor(nameof(GetColor))]
        [LabelText("@" + nameof(StaticName))]
#endif
        [SerializeField] private Object value;

        [NonSerialized] public T Value;

        public Object Obj
        {
            get => value;
            set
            {
                this.value = value switch
                {
                    GameObject gameObject => gameObject.TryGetComponent(out T component)
                        ? component as Object
                        : null,
                    T component => component as Object,
                    _ => null
                };
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
#if UNITY_EDITOR
        private static readonly Color InvalidColor;
        private static readonly string StaticName;

        static Ref()
        {
            Type type = typeof(T);
            StaticName = type.Name;

            if (type.IsInterface)
                StaticName = StaticName.Remove(0, 1);
            if (type.IsGenericType)
            {
                int index = StaticName.IndexOf("`", StringComparison.Ordinal);
                StaticName = StaticName.Remove(index);
                var gParams = type.GenericTypeArguments;
                StaticName = $"{StaticName}<{string.Join(", ", gParams.Select(x => x.Name))}>";
            }

            InvalidColor = new Color(1, .35f, .35f);
        }

        private Color GetColor()
        {
            return Validate(value)
                ? Color.white
                : InvalidColor;
        }
#endif
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

}