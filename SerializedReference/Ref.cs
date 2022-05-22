using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if AUTO_ATTACH
using System.Reflection;
using Dythervin.AutoAttach;
#endif

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
            return new Ref<T> { value = obj };
        }

        public static implicit operator Ref<T>(T obj)
        {
            return new Ref<T>(obj);
        }
    }

#if AUTO_ATTACH
    internal class RefSetter : AutoSetter
    {
        public override bool Compatible(Type value)
        {
            return value.IsGenericType && value.GetGenericTypeDefinition() == typeof(Ref<>);
        }

        public override bool TrySetField(Component target, FieldInfo fieldInfo, AutoAttachAttribute attribute)
        {
            Type targetType = fieldInfo.FieldType.GenericTypeArguments[0];
            IRef referenceField = (IRef)fieldInfo.GetValue(target);
            if (referenceField.Obj)
                return false;


            referenceField.Obj = attribute.type switch
            {
                AutoAttachType.Children => target.GetComponentInChildren(targetType),
                AutoAttachType.Parent => target.GetComponentInParent(targetType),
                _ => target.GetComponent(targetType)
            };

            bool set = referenceField.Obj;
            if (set)
                fieldInfo.SetValue(target, referenceField);

            return set;
        }
    }
#endif
}