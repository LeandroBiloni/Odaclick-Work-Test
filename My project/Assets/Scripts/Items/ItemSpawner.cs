using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;
    
    private Transform _canvas;

    private float _timeSinceLastSpawn;
    
    private bool _spawnOnlyCoins;

    private int _spawnedCoinsCounter;

    private RouletteWheel _roulette;

    private Dictionary<Item, int> _itemChances = new Dictionary<Item, int>();
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
        _canvas = FindObjectOfType<Canvas>().transform;
    }

    private void Start()
    {
        
        _roulette = new RouletteWheel();
    }
    
    
    public void SpawnItem()
    {
        Item item = null;

        var minItem = GameManager.Instance.GetSelectedDifficultyData().minObjectsToSpawn;
        var maxItem = GameManager.Instance.GetSelectedDifficultyData().maxObjectsToSpawn;
        
        //Determines how many items will spawn.
        var itemsQuantity = Random.Range(minItem, maxItem + 1);

        for (int i = 0; i < itemsQuantity; i++)
        {
            
            //If target was clicked, only coins will spawn.
            if (_spawnOnlyCoins)
            {
                if (_spawnedCoinsCounter > 0)
                {
                    _spawnedCoinsCounter--;

                    if (_spawnedCoinsCounter <= 0)
                    {
                        _spawnOnlyCoins = false;
                    }
                    foreach (var pair in _itemChances)
                    {
                        if (pair.Key.GetItemName() != "Coin") continue;
            
                        item = pair.Key;
                        break;
                    }
                }
            }
            else
            {
                if (_roulette == null)
                {
                    _roulette = new RouletteWheel();
                }
                
                //Determines which item will spawn.
                item = _roulette.Calculate(_itemChances);
            }
        
            if (item)
                Spawn(item); 
        }
    }

    /// <summary>
    /// Spawns an item at a random screen position.
    /// </summary>
    /// <param name="item">The item to spawn.</param>
    private void Spawn(Item item)
    {
        var i = Instantiate(item, _canvas);
        
        //Calculates position to spawn relative to screen resolution
        var res = Screen.currentResolution;
        float xPostiion = Random.Range(-(res.width / 2) + 100, (res.width / 2) - 100);
        float yPostiion = Random.Range(-(res.height / 2) + 100, (res.height / 2) - 100);
        
        Vector3 randomPos = new Vector3(xPostiion, yPostiion, 0);
        
        i.transform.localPosition = randomPos;
    }

    /// <summary>
    /// Forces the spawn of coins for the next items.
    /// </summary>
    /// <param name="amount">The amount of coins that will spawn.</param>
    public void ForceCoinSpawn(int amount)
    {
        _spawnOnlyCoins = true;
        _spawnedCoinsCounter = amount;
    }

    /// <summary>
    /// Sets item spawn chances to ItemSpawner.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="weight">Higher weight in relation to other items gives more chance to spawn the item.</param>
    public void AddSpawnChances(Item item, int weight)
    {
        _itemChances.Add(item, weight);
    }
}
