#if DYTHERVIN_AUTO_ATTACH && UNITY_EDITOR
using System;
using Dythervin.AutoAttach;
using Dythervin.AutoAttach.Setters;
using Dythervin.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dythervin.SerializedReference.Adapters
{
    internal class RefListAutoSetter : SetterBase
    {
        public override int Order => 99;

        public override bool Compatible(Type value)
        {
            return value.IsClass && value.IsGenericType && value.ImplementsOrInherits(typeof(IRefList));
        }

        public override bool TrySetField(Component target, object context, object currentValue, Type fieldType, AttachAttribute attribute, out object newValue)
        {
            Type targetType = fieldType.GenericTypeArguments[0];
            IRefList referenceField = (IRefList)currentValue ?? (IRefList)Activator.CreateInstance(fieldType);
            newValue = referenceField;

            var prevArray = referenceField.Objects;
            var componentArray = GetComponents(target, context, targetType, attribute);

            var array = prevArray != null && prevArray.Length == componentArray.Count
                ? prevArray
                : new Object [componentArray.Count];

            bool newValues = false;
            for (int i = 0; i < array.Length; i++)
            {
                Object value = componentArray[i];
                if (ReferenceEquals(array.GetValue(i), value))
                    continue;

                array.SetValue(componentArray[i], i);
                newValues = true;
            }

            if (newValues)
                referenceField.Objects = array;

            return newValues;
        }
    }
}
#endif