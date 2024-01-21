using UnityEngine;

public static class InstanceObject
{
    public static void SetInstance<T>(this T @this, ref T instance, bool active = false, bool hide = true, bool dontDestroy = true) where T : MonoBehaviour
    {
        if (instance != null) Object.Destroy(instance.gameObject);
        instance = @this;

        GameObject gameObject = @this.gameObject;
        if (hide) gameObject.hideFlags = HideFlags.DontSave;
        if (dontDestroy) Object.DontDestroyOnLoad(gameObject);
        gameObject.SetActive(active);
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
