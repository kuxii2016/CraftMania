using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class BlockManager : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();
 
    public void Awake()
    {
        //Dirt Block
        Block Dirt = new Block("Dirt", false, 2, 15);
        Blocks.Add(Dirt);
		Blocks.Add(Dirt = new Block("Dirt", false, 2, 15));
 
        //Grass Block
        Blocks.Add(new Block("Grass", false, 0, 15, 3, 15, 2, 15));
    }
}
