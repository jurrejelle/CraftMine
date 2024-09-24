using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Dictionary<int, BlockTexture> blockToTexture;
    public World world;
    private static WorldManager instance;

    void Start()
    {
        instance = this;
        blockToTexture = new Dictionary<int, BlockTexture>();
        // Skipping 0 since it's air
        blockToTexture[1] = new(1, 15*64 + 0, 17*64 + 7, 9*64+12); // Grass
        blockToTexture[2] = new(2, 9*64  + 12); // dirt
        blockToTexture[3] = new(3, 15*64 + 2); // Cobble

        world = new World(this);
        world.InitialWorldGen();
        world.UpdateAllChunks();
    }
    public static WorldManager GetManager() {
        return instance;
    }
}