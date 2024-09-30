using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _map;

    private ResourceSpawnerBase _foodSpawner;
    private ResourceSpawnerBase _timberSpawner;
    private ResourceSpawnerBase _marbleSpawner;

    private int _maxFoodOnMap;
    private int _maxTimberOnMap;
    private int _maxMarbleOnMap;
    private int _groundLayerMask;
    private int _idCounter;

    private float _mapX;
    private float _mapZ;
    private float _spawnDelay;

    private void Awake()
    {
        var foodPrefab = Resources.Load<Resource>("Prefabs/Food");
        _foodSpawner = new ResourceSpawnerBase(foodPrefab, CreateResource, transform);
        var timberPrefab = Resources.Load<Resource>("Prefabs/Timber");
        _timberSpawner = new ResourceSpawnerBase(timberPrefab, CreateResource, transform);
        var marblePrefab = Resources.Load<Resource>("Prefabs/Marble");
        _marbleSpawner = new ResourceSpawnerBase(marblePrefab, CreateResource, transform);
        _groundLayerMask = LayerMask.NameToLayer("Ground");
    }

    private void OnEnable()
    {
        _foodSpawner.Collected += OnFoodCollecting;
        _timberSpawner.Collected += OnTimberCollecting;
        _marbleSpawner.Collected += OnMarbleCollecting;
    }

    private void OnDisable()
    {
        _foodSpawner.Collected -= OnFoodCollecting;
        _timberSpawner.Collected -= OnTimberCollecting;
        _marbleSpawner.Collected -= OnMarbleCollecting;

        _foodSpawner.RemoveResourcesFromMap();
        _timberSpawner.RemoveResourcesFromMap();
        _marbleSpawner.RemoveResourcesFromMap();

        StopAllCoroutines();
    }

    private void Start()
    {
        const float OffsetFromTheEdgeOfTheMap = 1;
        const float Half = 0.5f;
        const float PlaneScale = 10;
        const float Area = PlaneScale * Half - OffsetFromTheEdgeOfTheMap;

        _maxFoodOnMap = 3;
        _maxMarbleOnMap = 3;
        _maxTimberOnMap = 5;
        _spawnDelay = 3f;

        _mapX = _map.localScale.x * Area;
        _mapZ = _map.localScale.z * Area;

        StartCoroutine(InitialResourceSpawn());
    }

    private Resource CreateResource(Resource prefab) => Instantiate<Resource>(prefab);

    private void OnFoodCollecting()
    {
        int numberOfFood = _maxFoodOnMap - _foodSpawner.NumberOfActiveResources;
        StartCoroutine(SpawnRes(_foodSpawner, numberOfFood));
    }

    private void OnTimberCollecting()
    {
        int numberOfTimber = _maxTimberOnMap - _timberSpawner.NumberOfActiveResources;
        StartCoroutine(SpawnRes(_timberSpawner, numberOfTimber));
    }

    private void OnMarbleCollecting()
    {
        int numberOfMarble = _maxMarbleOnMap - _marbleSpawner.NumberOfActiveResources;
        StartCoroutine(SpawnRes(_marbleSpawner, numberOfMarble));
    }

    private IEnumerator SpawnRes(ResourceSpawnerBase resourceSpawner, int numberOfResource)
    {
        Vector3 checkArea = new Vector3(3, 3, 3);
        Vector3 spawnPos = Vector3.zero;
        var delay = new WaitForSeconds(_spawnDelay);
        bool isWork = true;

        for (int i = 0; i < numberOfResource; i++)
        {
            yield return delay;
            isWork = true;

            while (isWork)
            {
                float posX = Random.Range(-_mapX, _mapX);
                float posZ = Random.Range(-_mapZ, _mapZ);
                spawnPos = new Vector3(posX, 0f, posZ);

                IList<Resource> list = new List<Resource>();
                Collider[] hits = Physics.OverlapBox(spawnPos, checkArea, Quaternion.identity, _groundLayerMask);

                if (hits.Length == 0)
                    isWork = false;
            }

            resourceSpawner.Spawn(spawnPos, _idCounter);
            _idCounter++;
        }
    }

    private IEnumerator InitialResourceSpawn()
    {
        float temp = _spawnDelay;
        _spawnDelay = 0;
        yield return StartCoroutine(SpawnRes(_foodSpawner, _maxFoodOnMap));
        yield return StartCoroutine(SpawnRes(_timberSpawner, _maxTimberOnMap));
        yield return StartCoroutine(SpawnRes(_marbleSpawner, _maxMarbleOnMap));
        _spawnDelay = temp;
    }
}