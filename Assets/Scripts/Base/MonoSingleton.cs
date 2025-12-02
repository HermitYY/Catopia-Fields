using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static bool _alive = true;

    public static T Instance
    {
        get
        {
            if (!_alive) return null;

            if (_instance == null)
            {
                T[] objs = FindObjectsByType<T>(FindObjectsSortMode.None);
                if (objs.Length > 0)
                {
                    _instance = objs[0];
                }
                else
                {
                    // 没找到就自己创建一个
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }

                _instance.Init();
            }

            return _instance;
        }
    }

    private bool _inited = false;

    protected virtual void Init()
    {
        if (_inited) return;
        _inited = true;
        // 子类可 override Init 做初始化
    }

    protected virtual void OnApplicationQuit()
    {
        _alive = false;
    }
}
