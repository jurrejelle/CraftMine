using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Dictionary<int, int> blockToTexture;
    private World world;

    void Start()
    {
        blockToTexture = new Dictionary<int, int>();
        // Skipping 0 since it's air
        blockToTexture[1] = 15*64 + 0; // Dirt
        blockToTexture[2] = 15*64 + 2; // Cobble

        world = new World(this);
        world.initialWorldGen();
    }
}