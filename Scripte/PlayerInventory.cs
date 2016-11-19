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

        AddItem(Block.getBlock("Dirt"), 5);
        AddItem(Block.getBlock("Dirt"), 5);
        AddItem(Block.getBlock("Dirt"), 64);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            showInventory = ! showInventory;
    }

    void OnGUI()
    {
        if (showInventory == false) return;

        Event e = Event.current;
        float space = 5;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int InventoryWidth = Width * SlotBackground.width;
                int InventoryHeight = Height * SlotBackground.height;

                Rect offset = new Rect((Screen.width / 2) - (InventoryWidth / 2), (Screen.height / 2) - (InventoryHeight / 2), InventoryWidth, InventoryHeight);
                Rect slotPos = new Rect(offset.x + SlotBackground.width * x, offset.y + SlotBackground.height * y, SlotBackground.width, SlotBackground.height);
                GUI.DrawTexture(slotPos, SlotBackground);

                Block b = InventoryItems[x, y];
                int n = InventoryNum[x, y];
                if (b != null)
                {
                    Rect BlockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                    GUI.DrawTexture(BlockPos, b.ItemView);
                    GUI.Label(slotPos, n.ToString());

                    if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem == null)
                    {
                        DragItem(x, y);
                        break;
                    }
                }
                if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem != null)
                {
                    MoveItem(x, y);
                    break;
                }
                if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 1)
                {
                    SplitItem(x, y);
                    break;
                }
            }
        }
        ShowDraggingItem(e, space);
    }

    void ShowDraggingItem(Event e, float space)
    {
        if (draggingItem != null)
        {
            GUI.DrawTexture(new Rect(e.mousePosition.x, e.mousePosition.y, SlotBackground.width - space, SlotBackground.height - space), draggingItem.ItemView);
            GUI.Label(new Rect(e.mousePosition.x + (SlotBackground.width - space) / 2, e.mousePosition.y + (SlotBackground.height - space) / 2, SlotBackground.width - space, SlotBackground.height - space), draggingItemNum.ToString());
        }
    }

    Block draggingItem;
    int draggingItemNum;
    void DragItem(int x, int y)
    {
        if (draggingItem != null) return;
        draggingItem = InventoryItems[x, y];
        draggingItemNum = InventoryNum[x, y];

        InventoryItems[x, y] = null;
        InventoryNum[x, y] = 0;
    }

    void MoveItem(int x, int y)
    {
        if (draggingItem == null) return;
        Block bloco = InventoryItems[x, y];
        int blocoN = InventoryNum[x, y];

        if (bloco == null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y] = draggingItemNum;

            draggingItem = null;
            draggingItemNum = 0;
        }
        else if (bloco == draggingItem )
        {
            if(blocoN + draggingItemNum > bloco.BlockMaxStack)
            {
                int rest = InventoryNum[x, y] + draggingItemNum - bloco.BlockMaxStack;
                InventoryNum[x, y] = bloco.BlockMaxStack;
                draggingItemNum = rest;
            }
            else
            {
                InventoryNum[x, y] += draggingItemNum;
                draggingItem = null;
                draggingItemNum = 0;
            }
        }

    }

    void SplitItem(int x, int y)
    {
        Block bloco = InventoryItems[x, y];
        int blocoN = InventoryNum[x, y];

        if (draggingItem != null && bloco == draggingItem)
        {
            if (InventoryNum[x, y] + 1 > bloco.BlockMaxStack)
            {

            }
            else
            {
                InventoryNum[x, y]++;
                draggingItemNum--;
            }
        }
        else if (draggingItem == null)
        {
            if (blocoN / 2 < 0)
                return;
            draggingItem = bloco;
            draggingItemNum = blocoN / 2;
            InventoryNum[x, y] -= draggingItemNum;
        }
        else if (draggingItem != null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y]++;
            draggingItemNum--;
        }
        if (draggingItemNum <= 0)
        {
            draggingItem = null;
        }
    }
    
    

    public void AddItem(Block b, int num)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (InventoryItems[x,y] == null)
                {
                    InventoryItems[x, y] = b;
                    if (num > b.BlockMaxStack)
                        InventoryNum[x, y] = b.BlockMaxStack;
                    else
                        InventoryNum[x, y] = num;
                    return;
                    }
                    else if(InventoryItems[x,y] == b && InventoryNum[x, y] < b.BlockMaxStack)
                    {
                    if (num > b.BlockMaxStack)
                    {
                        int rest = num - b.BlockMaxStack;
                        InventoryItems[x, y] = b;
                        InventoryNum[x, y] = b.BlockMaxStack;
                        AddItem(b, rest);
                        return;
                    }
                    else
                    {
                        if (InventoryNum[x, y] + num > b.BlockMaxStack)
                        {
                            int rest = InventoryNum[x, y] + num - b.BlockMaxStack;
                            InventoryItems[x, y] = b;
                            InventoryNum[x, y] = b.BlockMaxStack;
                            AddItem(b, rest);
                        }
                        else
                        {
                            InventoryItems[x, y] = b;
                            InventoryNum[x, y] += num;
                        }
                        return;
                    }
                }
            }
        }
    }
}
