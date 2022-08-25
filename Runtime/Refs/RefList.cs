using System;
using System.Collections.Generic;
using Dythervin.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.SerializedReference.Refs
{
    [Serializable]
    public class RefList<T> : ISerializationCallbackReceiver, IRefList
        where T : class
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [HideLabel]
#endif
        [SerializeField] private Object[] objects;

        public readonly List<T> values;

        public RefList()
        {
            values = new List<T>();
        }

        public RefList(IEnumerable<T> values)
        {
            this.values = new List<T>(values);
            objects = new Object[this.values.Count];
            for (int i = 0; i < this.values.Count; i++)
            {
                objects[i] = this.values[i] as Object;
            }
        }

        public int Lenght => objects.Length;

        public T this[int index]
        {
            get => values[index];
            set => values[index] = value;
        }

        Object[] IRefList.Objects
        {
            get => objects;
            set => objects = value;
        }

        public Object GetAt(int index)
        {
            return objects[index];
        }

        public bool SetAt(int index, Object obj)
        {
            bool valid = Validate(ref obj);
            if (valid)
                objects[index] = obj;
            return valid;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (objects == null)
                return;

            values.EnsureCapacity(objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                values.Add(objects[i] as T);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR && !ODIN_INSPECTOR
            for (int i = 0; i < objects.Length; i++)
            {
                Validate(ref objects[i]);
            }
#endif
        }

        public static implicit operator List<T>(RefList<T> a)
        {
            return a.values;
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }

        private bool Validate()
        {
            bool valid = true;
            for (int i = 0; i < objects.Length; i++)
            {
                valid &= Validate(ref objects[i]);
            }

            return valid;
        }

        private bool Validate(ref Object obj)
        {
            switch (obj)
            {
                case GameObject gameObject:
                {
                    if (!gameObject.TryGetComponent(out T component))
                        return false;

                    obj = component as Object;
                    return true;
                }
                default:
                    return obj is T;
            }
        }
    }
}