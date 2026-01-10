using UnityEngine;
using UnityEngine.Events;

public class LevelInitialization : MonoBehaviour
{
    public UnityEvent initialization;

    void Start()
    {
        initialization.Invoke();
    }
}
