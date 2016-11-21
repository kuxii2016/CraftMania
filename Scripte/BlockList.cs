using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockList : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();
    public Texture DirtT;
    public Texture StoneT;
    public Texture SandT;
    public Texture SandStoneT;


    public void Awake()
    {
        //Grass Block <- For Chunk Updates
        Blocks.Add(new Block("Grass", false, 0, 15, 3, 15, 2, 15));

        //Dirt Block
        Block dirt = new Block("Dirt", false, 2, 15);
        dirt.SetColor(Color.white, true);
        dirt.SetTexture(DirtT);
        Blocks.Add(dirt);

        //Sand Block
        Block sand = new Block("Sand", false, 2, 14);
        sand.SetColor(Color.white, true);
        sand.SetTexture(SandT);
        Blocks.Add(sand);
        //Sandstone Block

        Block Sandstone = new Block("SandStone", false, 2, 14);
        Sandstone.SetColor(Color.white, true);
        Sandstone.SetTexture(SandStoneT);
        Blocks.Add(Sandstone);


        //Blocks.Add(new Block("Stone", false, 1, 15));
        Block stone = new Block("Stone", false, 1, 15);
        stone.SetColor(Color.white, true);
        stone.SetTexture(StoneT);
        Blocks.Add(stone);

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
