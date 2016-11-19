using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    public Block[,] InventoryItems;
    public int[,] InventoryNum;
    public Texture2D SlotBackground;
    public bool showInventory = false;

    int Width = 9, Height = 5;

    void Start()
    {
        InventoryItems = new Block[Width, Height];
        InventoryNum = new int[Width, Height];
        InventoryItems[0, 0] = Block.getBlock("Dirt");
        InventoryNum[0, 0] = 56;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            showInventory = ! showInventory;
    }

    void OnGUI()
    {
        if (showInventory == false) return;
        for(int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int InventoryWidth = Width * SlotBackground.width;
                int InventoryHeight = Height * SlotBackground.height;

                Rect offset = new Rect((Screen.width / 2 ) - (InventoryWidth / 2), (Screen.height / 2) - (InventoryHeight / 2), InventoryWidth, InventoryHeight);
                Rect slotPos = new Rect(offset.x + SlotBackground.width * x, offset.y + SlotBackground.height * y, SlotBackground.width, SlotBackground.height);
                GUI.DrawTexture(slotPos, SlotBackground);

                Block b = InventoryItems[x, y];
                int n = InventoryNum[x, y];
                if(b != null)
                {
                    float space = 5;
                    Rect BlockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                    GUI.DrawTexture(BlockPos, b.ItemView);
                    GUI.Label(slotPos, n.ToString());
                }
            }

        }
    }

    void DragItem()
    {

    }

}
