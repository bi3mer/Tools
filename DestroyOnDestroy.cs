using UnityEngine;

namespace UnityUtility
{
    public class DestroyOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}