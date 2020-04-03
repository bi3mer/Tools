using UnityEngine;

namespace UnityUtility.Mono
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static object _lock = new object();
        private static bool appIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (appIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (instance == null)
                    {
                        //Link to potential issue/solution: http://forum.unity3d.com/threads/findobjectoftype-not-detecting-an-object.48026/
                        instance = (T)FindObjectOfType(typeof(T));
                    }

                    return instance;
                }
            }
        }

        public void OnApplicationQuit()
        {
            appIsQuitting = true;
        }
    }
}