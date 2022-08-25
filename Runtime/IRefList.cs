using UnityEngine;

namespace Dythervin.SerializedReference
{
    public interface IRefList
    {
        Object[] Objects { get; set; }
        Object GetAt(int index);
        bool SetAt(int index, Object obj);
    }
}