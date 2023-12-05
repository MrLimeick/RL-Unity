using UnityEngine;

public static class InstanceObject
{
    public static bool SetInstance<T>(this T @this, ref T instance) where T : MonoBehaviour
    {
        GameObject gameObject = @this.gameObject;

        if (instance != null)
        {
            Object.Destroy(gameObject);
            return false;
        }

        instance = @this;

        gameObject.hideFlags = HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resource">Name of prefab in "Assets/Resources/Prefabs/"</param>
    public static void InstantiatePrefab(string resource)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{resource}");
        Object.Instantiate(prefab);
    }
}

public static class MonoBehaviourExtension
{
    
}
