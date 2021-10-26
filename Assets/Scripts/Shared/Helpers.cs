using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Shared
{
    public static class Helpers
    {
        public static IEnumerator DoAfterMilliseconds(int milliseconds, Action action)
        {
            yield return new WaitForSeconds(1f / milliseconds);
            action();
        }

        public static IEnumerator DoAfterMilliseconds<T>(int milliseconds, T param, Action<T> action)
        {
            yield return new WaitForSeconds(milliseconds / 1000f);
            action(param);
        }

        public static IEnumerator DoNextFrame<T>(T param, Action<T> action)
        {
            yield return 0;
            action(param);
        }

        public static IEnumerator DoNextPhysicsUpdate<T>(T param, Action<T> action)
        {
            yield return new WaitForFixedUpdate();
            action(param);
        }

        public static void AssertIsNotNullOrQuit<T>(T assert, string message)
            where T : class
        {
            try
            {
                Assert.IsNotNull(assert, message);
            }
            catch (AssertionException e)
            {
                Debug.LogError(e.Message);
                Quit();
            }
        }

        public static void AssertIsTrueOrQuit(bool condition, string message)
        {
            try
            {
                Assert.IsTrue(condition, message);
            }
            catch (AssertionException e)
            {
                Debug.LogError(e);
                Quit();
            }
        }
        
        public static void Quit()
        {
#if DEBUG
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}