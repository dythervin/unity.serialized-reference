using UnityEngine;

namespace Dythervin.SerializedReference.Demo
{
    public class RefDemo : MonoBehaviour
    {
        [SerializeField] private Ref<IDemoInterface> demoInterface;

        private void Awake()
        {
            //Usage
            demoInterface.Value.LogName();
        }
    }
}