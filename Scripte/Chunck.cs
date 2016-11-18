using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunck : MonoBehaviour {

	public Block[, ,] map;
	public static int Width = 20, Height = 10;
    public static bool working = false;
	public static List<Chunck> Chuncks = new List<Chunck> ();
    bool ready = false;
	List<Vector3> vertices = new List<Vector3> ();
	List<int> triangulos = new List<int> ();
	List<Vector2> uvs = new List<Vector2>();
	float TextureOffset = 1F / 16F;
	Mesh mesh;

    void Start()
    {
        Chuncks.Add(this);
    }

	void Update()
	{
        if (working == false && ready == false)
        {
            ready = true;
            StartFunktion();
        }
	}

	public void StartFunktion()
	{
        working = true;
		mesh = new Mesh ();
		map = new Block[Width, Height, Width];
        StartCoroutine (CalculateMap ());
	}

	public IEnumerator CalculateMap()
	{
		for(int x = 0; x < Width; x++)
		{
			for(int y = 0; y < Height; y ++)
			{
				for(int z = 0; z < Width; z++)
				{
					if (y < 5) 
					{
						map [x, y, z] = Block.getBlock ("Dirt");
					}
					if(y == 5 && Random.Range(0, 5) == 1)
					{
						map[x, y, z] = Block.getBlock("Grass");
					}
				}
			}
		}
		yield return 0;
		StartCoroutine (CalculateMesh ());
	}
       
	public IEnumerator CalculateMesh()
	{
               
		for (int x = 0; x < Width; x++) 
		{
			for (int y = 0; y < Height; y++) 
			{
				for (int z = 0; z < Width; z++) 
				{
					if (map [x, y, z] != null) 
					{
                        if (isBlockTransparent(x, y, z + 1))
                        {
                            AddCubeFront(x, y, z, map[x, y, z]);
                        }
						if (isBlockTransparent (x, y, z - 1))
                        {
                            AddCubeBack (x, y, z, map [x, y, z]);
                        }
                        if (isBlockTransparent (x, y + 1, z))
                        {
                            AddCubeTop (x, y, z, map [x, y, z]);
                        }
                        if (isBlockTransparent (x, y - 1, z))
                        {
                            AddCubeBottom (x, y, z, map [x, y, z]);
                        }
                        if (isBlockTransparent (x + 1, y, z))
                        {
                            AddCubeRight (x, y, z, map [x, y, z]);
                        }
                        if (isBlockTransparent (x - 1, y, z))
                        {
                            AddCubeLeft (x, y, z, map [x, y, z]);
                        }
                    }
				}
			}
		}
  
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangulos.ToArray ();
		mesh.uv = uvs.ToArray ();
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		mesh.Optimize ();
		GetComponent<MeshCollider> ().sharedMesh = mesh;
		GetComponent<MeshFilter>().mesh = mesh;
		yield return 0;
        working = false;

        }
       
        public void AddCubeFront(int x, int y, int z, Block b)
        {              
                z++;
               
                int offset = 1;
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(2 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                triangulos.Add(4 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
                vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
                vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
        }
        public void AddCubeBack(int x, int y, int z, Block b)
        {               
                int offset = 1;
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(2 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
               
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(4 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
                vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
                vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
        }
        public void AddCubeTop(int x, int y, int z, Block b)
        {              
                int offset = 1;
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(2 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
               
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(4 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY));
                uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY));
                uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
                vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
                vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
        }
        public void AddCubeBottom(int x, int y, int z, Block b)
        {              
                y--;
               
                int offset = 1;
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(2 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                triangulos.Add(4 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom));
                uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom));
                uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
                vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
                vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
        }
        public void AddCubeRight(int x, int y, int z, Block b)
        {               
                int offset = 1;
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(2 - offset + vertices.Count);
               
                triangulos.Add(4 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
                vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
                vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
        }
        public void AddCubeLeft(int x, int y, int z, Block b)
        {              
                x--;
               
                int offset = 1;
                triangulos.Add(2 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(1 - offset + vertices.Count);
               
                triangulos.Add(1 - offset + vertices.Count);
                triangulos.Add(3 - offset + vertices.Count);
                triangulos.Add(4 - offset + vertices.Count);
               
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
               
                vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
                vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
                vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
        }
       
        bool isBlockTransparent(int x, int y, int z){
                if(x >= Width || y >= Height || z >= Width || x < 0 || y < 0 || z < 0)
                        return true;
               
                if(map[x, y, z] == null)
                        return true;
               
                return false;
        }
       
	public static Chunck GetChunck(int x, int y, int z)
	{
		for (int i = 0; i < Chuncks.Count; i++ )
		{
			Vector3 pos = new Vector3 (x, y, z);
			Vector3 cpos = Chuncks[i].transform.position;
			if (cpos.Equals(pos))
			{
				return Chuncks[i];
			}
			if(pos.x < cpos.x || pos.y < cpos.y || pos.z < cpos.z || pos.x >= cpos.x + Width || pos.y >= cpos.y + Height || pos.z >= cpos.z + Width)
			{
			    continue;
			}
			return Chuncks[i];
		}
		return null;
	}
}
 
 
public class Block
{
        public string BlockName;
        public int BlockID;
        public bool Trasnsparent = false;
        public int TextureX;
        public int TextureY;
       
        public int TextureXSide;
        public int TextureYSide;
       
        public int TextureXBottom;
        public int TextureYBottom;
       
        public bool BlockGlow;
        public Color BlockColor = Color.white;
       
        public Block()
        {
                BlockID = -1;
                Trasnsparent = true;
        }

        public Block(string name, bool transparent, int tX, int tY)
        {
                Trasnsparent = transparent;
                BlockName = name;
                BlockID = BlockManager.Blocks.Count;
                TextureX = tX;
                TextureY = tY;
                TextureXSide = tX;
                TextureYSide = tY;
                TextureXBottom = tX;
                TextureYBottom = tY;
        }

        public Block(string name, bool transparent, int tX, int tY, int sX, int sY)
        {
                Trasnsparent = transparent;
                BlockName = name;
				BlockID = BlockManager.Blocks.Count;
                TextureX = tX;
                TextureY = tY;
                TextureXSide = sX;
                TextureYSide = sY;
                TextureXBottom = tX;
                TextureYBottom = tY;
        }

        public Block(string name, bool transparent, int tX, int tY, int sX, int sY, int bX, int bY)
        {
                Trasnsparent = transparent;
                BlockName = name;
				BlockID = BlockManager.Blocks.Count;
                TextureX = tX;
                TextureY = tY;
                TextureXSide = sX;
                TextureYSide = sY;
                TextureXBottom = bX;
                TextureYBottom = bY;
        }
       
        public void SetColor(Color color, bool glow)
        {
                BlockGlow = glow;
                BlockColor = color;
        }
       
        public static Block getBlock(string name)
        {
				foreach(Block b in BlockManager.Blocks)
                {
                        if (b.BlockName == name)
                                return b;
                }
                return new Block();
        }
}
