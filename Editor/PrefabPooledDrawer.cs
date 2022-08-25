using Dythervin.Core.Editor.Drawers;
using Dythervin.SerializedReference.Refs;
using UnityEditor;

namespace Dythervin.SerializedReference.Editor
{
    [CustomPropertyDrawer(typeof(ObjPtr<>))]
    public class PrefabPooledDrawer : SimpleGenericDrawer { }
}