public class SingletonData<T> where T : class, new()
{
    private static readonly object _lock = new object();

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}