#if AUTO_ATTACH
using System;
using System.Reflection;
using Dythervin.AutoAttach;
using Dythervin.SerializedReference;
using UnityEngine;

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