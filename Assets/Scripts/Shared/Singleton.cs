using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Object
    {
        private static T _instance;

        public static T Instance =>
            _instance ??= FindObjectOfType<T>()
                          ?? throw new InvalidOperationException($"No {typeof(T).Name} found in scene");

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}