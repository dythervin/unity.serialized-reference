using Dythervin.Core.Editor.Drawers;
using Dythervin.Core.Extensions;
using Dythervin.SerializedReference.Refs;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Dythervin.SerializedReference.Editor
{
    [CustomPropertyDrawer(typeof(Ref<>))]
    public class RefDrawer : SimpleGenericDrawer
    {
        protected override bool Validate(Object obj)
        {
            return obj.GetType().ImplementsOrInherits(fieldInfo.FieldType.GenericTypeArguments[0]);
        }
    }
}