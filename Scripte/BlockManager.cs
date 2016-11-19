using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockList : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();
    public Texture DirtT;
    public Texture StoneT;
    public Texture GrassT;


    public void Awake()
    {
        //Dirt Block
        Block dirt = new Block("Dirt", false, 2, 15);
        dirt.SetColor(Color.white, true);
        dirt.SetTexture(DirtT);
        Blocks.Add(dirt);

        //Grass Block
        Blocks.Add(new Block("Grass", false, 0, 15, 3, 15, 2, 15));
        Blocks.Add(new Block("Stone", false, 1, 15));
    }

    public static Block GetBlock(string Name)
    {
        foreach(Block b in Blocks)
        {
            if(b.BlockName == Name)
                return b;
        }
        return null;
    }
}
