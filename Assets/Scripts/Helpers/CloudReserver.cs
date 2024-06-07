using System.Collections;
using Kimicu.YandexGames;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public class CloudReserver 
    {
        private static CoroutineRunner coroutineRunner;

        [MenuItem("Custom Tools/CloudReserver/Clear Data")]
        private static void ClearData()
        {
            GameObject tempGameObject = new GameObject("TempGameObject");
            coroutineRunner = tempGameObject.AddComponent<CoroutineRunner>();

            coroutineRunner.StartCoroutine(ClearDataCoroutine());
        }

        private static IEnumerator ClearDataCoroutine()
        {
            yield return Cloud.Initialize();

            Cloud.ClearCloudData(
                () =>
                {
                    Debug.Log("Success: Data cleared successfully");
                    Debug.Log("Player progress has been reset.");
                },
                (error) =>
                {
                    Debug.LogError($"Error: Failed to clear data - {error}");
                });

            if (coroutineRunner != null)
            {
                Object.DestroyImmediate(coroutineRunner.gameObject);
            }
        }
        
        private class CoroutineRunner : MonoBehaviour { }
    }
}