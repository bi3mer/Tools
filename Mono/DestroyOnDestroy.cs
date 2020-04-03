using UnityEngine;

namespace Tools.Mono
{
    public class DestroyOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}