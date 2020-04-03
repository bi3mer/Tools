using UnityEngine;

namespace UnityUtility.Mono
{
    public class DestroyOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}