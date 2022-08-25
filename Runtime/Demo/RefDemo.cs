#if DYTHERVIN_AUTO_ATTACH
using Dythervin.AutoAttach;
#endif
using Dythervin.SerializedReference.Refs;
using UnityEngine;

namespace Dythervin.SerializedReference.Demo
{
    public class RefDemo : MonoBehaviour
    {
        [SerializeField] private Ref<IDemoInterface> demoInterface;
#if DYTHERVIN_AUTO_ATTACH
        [Attach(Attach.Child)]
#endif
        [SerializeField] private ObjPtr<RefDemoMonobehaviour> objPtrDemoInterface;

        private void Awake()
        {
            //Usage
            demoInterface.Value.LogName();
        }
    }
}