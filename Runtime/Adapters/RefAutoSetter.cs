#if DYTHERVIN_AUTO_ATTACH
using System;
using Dythervin.AutoAttach;
using Dythervin.AutoAttach.Setters;
using Dythervin.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dythervin.SerializedReference.Adapters
{
    internal class RefSetter : SetterBase
    {
        public override int Order => 100;

        public override bool Compatible(Type value)
        {
            return value.IsGenericType && value.IsValueType && value.ImplementsOrInherits(typeof(IRef));
        }

        public override bool TrySetField(Component target, object context, object currentValue, Type fieldType, AttachAttribute attribute, out object newValue)
        {
            Type targetType = fieldType.GenericTypeArguments[0];
            var referenceField = (IRef)(newValue = currentValue ?? Activator.CreateInstance(fieldType));
            if (!attribute.readOnly && referenceField.Obj)
                return false;

            Object newObj = GetComponent(target, context, targetType, attribute);
            if (newObj == referenceField.Obj)
                return false;

            referenceField.Obj = newObj;
            return true;
        }
    }
}

#endif