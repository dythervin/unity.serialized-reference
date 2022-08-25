using UnityEngine;

namespace Dythervin.SerializedReference.Demo
{
    public class RefDemoMonobehaviour : MonoBehaviour, IDemoInterface
    {
        public void LogName()
        {
            Debug.Log(name);
        }
    }
}