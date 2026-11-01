using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class EventSystemFixer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void FixInputModule()
    {
        FixCurrentSceneEventSystem();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FixCurrentSceneEventSystem();
    }

    private static void FixCurrentSceneEventSystem()
    {
        EventSystem es = Object.FindFirstObjectByType<EventSystem>();
        if (es != null)
        {
            BaseInputModule inputModule = es.GetComponent<BaseInputModule>();
            if (inputModule != null && inputModule.GetType().Name.Contains("InputSystemUIInputModule"))
            {
                Debug.Log("EventSystemFixer: Replacing InputSystemUIInputModule with StandaloneInputModule to fix UI input.");
                Object.DestroyImmediate(inputModule);
                es.gameObject.AddComponent<StandaloneInputModule>();
            }
            else if (inputModule == null)
            {
                Debug.Log("EventSystemFixer: Adding missing StandaloneInputModule to fix UI input.");
                es.gameObject.AddComponent<StandaloneInputModule>();
            }
        }
    }
}
