using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    [SerializeField] private Transform _map;

    public Transform Map { get { return _map; } }
    public ResourceScaner ResourceScaner { get; private set; }
    public ResourceSpawner ResourceSpawner { get; private set; }

    private void Awake()
    {
        ResourceScaner = GetComponentInChildren<ResourceScaner>();
        ResourceSpawner = GetComponentInChildren<ResourceSpawner>();
    }
}
