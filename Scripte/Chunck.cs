using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunck : MonoBehaviour {

	public Block[, ,] map;
	public static int Width = 35, Height = 10;
    public static bool working = false;
	public static List<Chunck> Chuncks = new List<Chunck> ();
    List<Color> colors = new List<Color>();
    public static int seed = 505;
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
                    map[x, y, z] = GetTheoreticalBlock(transform.position + new Vector3(x, y, z));
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
                        if (y < 1) continue;
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
        mesh.colors = colors.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		mesh.Optimize ();
		GetComponent<MeshCollider> ().sharedMesh = mesh;
		GetComponent<MeshFilter>().mesh = mesh;
		yield return 0;
        working = false;

        }

    public Block GetTheoreticalBlock(Vector3 pos)
    {
        Random.seed = seed;
        Vector3 offset = new Vector3(Random.value * 100000, Random.value * 100000, Random.value * 100000);
        float noiseX = Mathf.Abs((float)(pos.x +  offset.x) / 20);
        float noiseY = Mathf.Abs((float)(pos.y +  offset.y) / 20);
        float noiseZ = Mathf.Abs((float)(pos.z +  offset.z) / 20);

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        noiseValue += (8 - (float)pos.y) / 5;
        noiseValue /= (float)pos.y / 4f;

        if (noiseValue > 0.2F)
            return Block.getBlock("Dirt");
        return null;
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

                //CalculateLightFront(x, y, z);

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

                //CalculateLightBack(x, y, z);

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

                //CalculateLightTop(x, y, z);

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

                //CalculateLightTop(x, y, z);

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

                //CalculateLightRight(x, y, z);

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

                //CalculateLightLeft(x, y, z);

                vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
                vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
                vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
                vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
        }

  /*  void CalculateLightTop(int x, int y, int z)
    {
        int index = colors.Count;

        Block b = getBlock(x, y, z);
        if (b == null)
            b = new Block();

        colors.Add(Color.white);
        colors.Add(Color.white);
        colors.Add(Color.white);
        colors.Add(Color.white);

        {
            if (!isBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] = Color.gray;
                colors[index + 1] = Color.gray;
            }

            if (!isBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 0] = Color.gray;
                colors[index + 3] = Color.gray;
            }

            if (!isBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 1] = Color.gray;
                colors[index + 0] = Color.gray;
            }

            if (!isBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] = Color.gray;
                colors[index + 3] = Color.gray;
            }

            if (!isBlockTransparent(x + 1, y + 1, z + 1))
            {
                colors[index + 3] = Color.gray;
            }

            if (!isBlockTransparent(x - 1, y + 1, z - 1))
            {
                colors[index + 1] = Color.gray;
            }

            if (!isBlockTransparent(x - 1, y + 1, z + 1))
            {
                colors[index + 2] = Color.gray;
            }

            if (!isBlockTransparent(x + 1, y + 1, z - 1))
            {
                colors[index + 0] = Color.gray;
            }
        }
    }
    void CalculateLightRight(int x, int y, int z)
    {
        Block b = getBlock(x, y, z);
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x + 1, y - 1, z) && isBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] = Color.gray;
                colors[index + 1] = Color.gray;
            }
        }
    }
    void CalculateLightLeft(int x, int y, int z)
    {
        Block b = getBlock(x, y, z);
        int index = colors.Count;

        colors.Add(Color.white);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x - 1, y - 1, z) && isBlockTransparent(x, y, z))
            {
                colors[index + 0] = Color.gray;
                colors[index + 1] = Color.gray;
            }
        }
    }
    void CalculateLightBack(int x, int y, int z)
    {
        Block b = getBlock(x, y, z);
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x, y - 1, z - 1) && isBlockTransparent(x, y, z - 1))
            {
                colors[index + 0] = Color.gray;
                colors[index + 1] = Color.gray;
            }
        }
    }
    void CalculateLightFront(int x, int y, int z)
    {
        Block b = getBlock(x, y, z);
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x, y - 1, z + 1) && isBlockTransparent(x, y, z))
            {
                colors[index + 0] = Color.gray;
                colors[index + 1] = Color.gray;
            }
        }
    } */

    bool isBlockTransparent(int x, int y, int z){
                if(x >= Width || y >= Height || z >= Width || x < 0 || y < 0 || z < 0)
                        return true;
               
                if(map[x, y, z] == null)
                        return true;
               
                return false;
        }

    Block getBlock(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width || x < 0 || y < 0 || z < 0)
            return null;

            return map[x, y, z];
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
