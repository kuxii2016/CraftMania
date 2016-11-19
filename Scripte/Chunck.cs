using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunck : MonoBehaviour
{
    public Block[, ,] map;
    public static int Width = 30, Height = 7;

    public static List<Transform> Chuncks = new List<Transform>();

    Chunck left;
    Chunck right;
    Chunck back;
    Chunck front;
    Chunck bottom;
    Chunck bottom2;
    Chunck top;

    public GameObject chunckPrefab;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangulos = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public Color shadowColors;

    float TextureOffset = 1F / 16F;
    Mesh mesh;

    public static bool working = false;
    public static bool Blockworking = false;
    public bool ready = false;
    public bool generatedMap = false;

    bool once = true;
    void Start()
    {
        Chuncks.Add(this.transform);

        if (Time.time < 1)
        {
            StartFunction();
        }
    }
    void Update()
    {
        if (working == false && ready == false)
        {
            ready = true;
            StartFunction();
        }
    }
    public void StartFunction()
    {
        mesh = new Mesh();
        map = new Block[Width, Height, Width];

        StartCoroutine(CalculateMap());
    }

    public static int seed = 505;

    public IEnumerator CalculateMap()
    {
        working = true;

        Block b = null;
        bool render = false;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    Block bloco = GetTheoreticalBlock(transform.position + new Vector3(x, y, z));
                    if( x == 0 && y == 0 && z == 0)
                        b = bloco;

                    if (bloco != null && bloco.BlockName == "Dirt")
                    {
                        if(GetTheoreticalBlock(transform.position + new Vector3(x, y + 1, z)) == null)
                            map[x, y, z] = BlockList.GetBlock("Grass");
                        else
                            map[x, y, z] = bloco;
                    }
                    else
                    {
                        map[x, y, z] = bloco;
                    }

                    if (render == false)
                    if (map[x, y, z] != b)
                        render = true;
                }
            }
        }
        generatedMap = true;

        yield return 0;

        if (render)
            StartCoroutine(CalculateMesh());
        else
        {
            ready = true;
            working = false;
        }
    }
    public IEnumerator CalculateMesh()
    {
        working = true;
        mesh = new Mesh();
        vertices.Clear();
        triangulos.Clear();
        colors.Clear();
        uvs.Clear();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    if (map[x, y, z] != null)
                    {
                        Vector3 worldPosition = new Vector3(x, y, z) + transform.position;
                        if (Mathf.FloorToInt(worldPosition.y) < 1) continue;

                        if (InitialisBlockTransparent(x, y, z + 1))
                            AddCubeFront(x, y, z, map[x, y, z]);
                        if (InitialisBlockTransparent(x, y, z - 1))
                            AddCubeBack(x, y, z, map[x, y, z]);
                        if (InitialisBlockTransparent(x, y + 1, z))
                            AddCubeTop(x, y, z, map[x, y, z]);
                        if (InitialisBlockTransparent(x, y - 1, z))
                            AddCubeBottom(x, y, z, map[x, y, z]);
                        if (InitialisBlockTransparent(x + 1, y, z))
                            AddCubeRight(x, y, z, map[x, y, z]);
                        if (InitialisBlockTransparent(x - 1, y, z))
                            AddCubeLeft(x, y, z, map[x, y, z]);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        yield return 0;
        ready = true;
        working = false;
    }

    public IEnumerator RecalculateMesh()
    {
        Blockworking = true;
        ready = true;

        mesh = new Mesh();
        vertices.Clear();
        triangulos.Clear();
        colors.Clear();
        uvs.Clear();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    if (map[x, y, z] != null)
                    {
                        Vector3 worldPosition = new Vector3(x, y, z) + transform.position;
                        if (Mathf.FloorToInt(worldPosition.y) < 1) continue;
                        if (isBlockTransparent(x, y, z + 1))
                            AddCubeFront(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y, z - 1))
                            AddCubeBack(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y + 1, z))
                            AddCubeTop(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y - 1, z))
                            AddCubeBottom(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x + 1, y, z))
                            AddCubeRight(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x - 1, y, z))
                            AddCubeLeft(x, y, z, map[x, y, z]);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        yield return 0;

        Blockworking = false;
        yield return 0;
    }

    public Block GetTheoreticalBlock(Vector3 pos)
    {
        Random.seed = seed;

        Vector3 offset = new Vector3(Random.value * 100000, Random.value * 100000, Random.value * 100000);

        float noiseX = Mathf.Abs((float)(pos.x + offset.x) / 20);
        float noiseY = Mathf.Abs((float)(pos.y + offset.y) / 20);
        float noiseZ = Mathf.Abs((float)(pos.z + offset.z) / 20);

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
        float cavernas = SimplexNoise.Noise.Generate(noiseX, Mathf.Abs((float)(pos.y + offset.y - 10) / 20), noiseZ);

        noiseValue += (200 - (float)pos.y) / 18f;
        noiseValue /= (float)pos.y / 8f;

        cavernas /= (float)pos.y / 19f;
        cavernas /= 2;

        if (noiseValue > 0.2f)
            return Block.getBlock("Dirt");

        if (cavernas > 0.2f)
            return null;

        return null;
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        //x = x /*+ Mathf.FloorToInt(transform.position.x)*/;
        //y = y /*+ Mathf.FloorToInt(transform.position.y)*/;
        //z = z /*+ Mathf.FloorToInt(transform.position.z*/);

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

        CalculateLightFront(x, y, z, b);

        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    public void AddCubeBack(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

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
        CalculateLightBack(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    public void AddCubeTop(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

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
        CalculateLightTop(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }

    public void AddCubeBottom(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

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
        CalculateLightTop(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }

    public void AddCubeRight(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

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
        CalculateLightRight(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    public void AddCubeLeft(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
       // y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

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
        CalculateLightLeft(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    void CalculateLightTop(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] = shadowColors;;
                colors[index + 1] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 0] = shadowColors;;
                colors[index + 3] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors;;
                colors[index + 0] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors;;
                colors[index + 3] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z + 1))
            {
                colors[index + 3] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors;;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z - 1))
            {
                colors[index + 0] = shadowColors;;
            }
        }
    }

    void CalculateLightRight(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z) && InitialisBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] = shadowColors;;
                colors[index + 1] = shadowColors;;
            }
        }
    }

    void CalculateLightLeft(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z) && InitialisBlockTransparent(x, y, z))
            {
                colors[index + 0] = shadowColors;;
                colors[index + 1] = shadowColors;;
            }
        }
    }

    void CalculateLightBack(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z - 1) && InitialisBlockTransparent(x, y, z - 1))
            {
                colors[index + 0] = shadowColors;;
                colors[index + 1] = shadowColors;;
            }
        }
    }

    void CalculateLightFront(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z) && InitialisBlockTransparent(x, y, z))
            {
                colors[index + 0] = shadowColors;;
                colors[index + 1] = shadowColors;;
            }
        }
    }

    bool isBlockTransparent(int x, int y, int z)
    {
        if( x >= Width)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (right == null)
            {
                right = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (right != this && right != null && right.generatedMap == true)
            {
                if (right.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if (x < 0)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (left == null)
            {
                left = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (left != this && left != null && left.generatedMap == true)
            {
                if (left.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if (z >= Width)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (front == null)
            {
                front = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (front != this && front != null && front.generatedMap == true)
            {
                if (front.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if (z < 0)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (back == null)
            {
                back = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (back != this && back != null && back.generatedMap == true)
            {
                if (back.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if( y >= Height)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (top == null)
            {
                top = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (top != this && top != null && top.generatedMap == true)
            {
                if (top.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if (y < 0)
        {
            Vector3 worldPosition = new Vector3(x, y, z) + transform.position;

            if (bottom == null)
            {
                bottom = GetChunck(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
            }
            if (bottom != this && bottom != null && bottom.generatedMap == true)
            {
                if (bottom.GetBlock(worldPosition) == null)
                    return true;
                else
                    return false;
            }
            return true;
        }

        if (map[x, y, z] == null)
            return true;
        if (map[x, y, z].Trasnsparent)
            return true;

        return false;
    }

    bool InitialisBlockTransparent(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width
            || x < 0 || y < 0 || z < 0)
        {
            if (GetTheoreticalBlock(transform.position + new Vector3(x, y, z)) == null)
                return true;
            else
                return false;
        }

        if (map[x, y, z] == null)
            return true;

        if (map[x, y, z].Trasnsparent)
            return true;

        return false;
    }

    public static Chunck GetChunck(int x, int y, int z)
    {
        for (int i = 0; i < Chuncks.Count; i ++ )
        {
            Transform t = Chuncks[i];
            Vector3 pos = new Vector3(x, y, z);
            Vector3 cpos = t.position;

            if (pos.x < cpos.x || pos.y < cpos.y || pos.z < cpos.z || pos.x >= cpos.x + Width || pos.y >= cpos.y + Height || pos.z >= cpos.z + Width)
            {
                continue;
            }

            return t.gameObject.GetComponent<Chunck>();
        }
        return null;
    }

    public void SetBlock(Vector3 worldPos, Block b)
    {
        Vector3 localPos;
        localPos = worldPos - transform.position;

        if (localPos.x > (Width))
        {
            return;
        }

        if (Mathf.FloorToInt(localPos.x) >= Width || Mathf.FloorToInt(localPos.y) >= Height || Mathf.FloorToInt(localPos.z) >= Width
            || Mathf.FloorToInt(localPos.x) < 0 || Mathf.FloorToInt(localPos.y) < 0 || Mathf.FloorToInt(localPos.z) < 0)
        {
        }
        else
        {
            map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = b;
        }

        StartCoroutine(RecalculateMesh());

        if (b != null) return;
        if (Mathf.FloorToInt(localPos.x) >= Width - 1)
        {
            if (right == null)
            {
                right = GetChunck(Mathf.FloorToInt(worldPos.x + 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
            }
            StartCoroutine(right.RecalculateMesh());
        }

        if (Mathf.FloorToInt(localPos.x) <= 1)
        {
            if (left == null)
            {
                left = GetChunck(Mathf.FloorToInt(worldPos.x - 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
            }
            StartCoroutine(left.RecalculateMesh());
        }

        if (Mathf.FloorToInt(localPos.z) >= Width - 1)
        {
            if (front == null)
            {
                front = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z + 1));
            }
            StartCoroutine(front.RecalculateMesh());
        }

        if (Mathf.FloorToInt(localPos.z) <= 1)
        {
            if (back == null)
            {
                back = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z - 1));
            }
            StartCoroutine(back.RecalculateMesh());
        }

        if (Mathf.FloorToInt(localPos.y) >= Height - 1)
        {
            if (top == null)
            {
                top = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y + 1), Mathf.FloorToInt(worldPos.z - 1));
            }
            StartCoroutine(top.RecalculateMesh());
        }

        if (Mathf.FloorToInt(localPos.y) <= 1)
        {
            if (bottom == null)
            {
                bottom = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y - 1), Mathf.FloorToInt(worldPos.z - 1));
                /*bottom2 = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y - 4), Mathf.FloorToInt(worldPos.z - 1));
                if (bottom2 == null)
                {

                }
                else
                {
                    int zCoordinate = Mathf.FloorToInt((worldPos.z / Width)) * Width;
                    int xCoordinate = Mathf.FloorToInt((worldPos.x / Width)) * Width;
                    int yCoordinate = Mathf.FloorToInt((worldPos.y - 4 / Height)) * Height;
                    GameObject go = Instantiate(chunckPrefab, new Vector3(xCoordinate, yCoordinate, zCoordinate), Quaternion.identity) as GameObject;
                    go.GetComponent<Chunck>().StartFunction();
                }*/
            }
            StartCoroutine(bottom.RecalculateMesh());
        }
    }

    public Block GetBlock(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
    }
}

public class Block
{
    public Texture ItemView;
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

    public int BlockMaxStack = 64;
    
    public Block()
    {
        BlockID = -1;
        Trasnsparent = true;
    }

    public Block(string name, bool transparent, int tX, int tY)
    {
        Trasnsparent = transparent;
        BlockName = name;
        BlockID = BlockList.Blocks.Count;
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
        BlockID = BlockList.Blocks.Count;
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
        BlockID = BlockList.Blocks.Count;
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

    public void SetTexture(Texture i)
    {
        ItemView = i;
    }

    public static Block getBlock(string name)
    {
        foreach(Block b in BlockList.Blocks)
        {
            if (b.BlockName == name)
                return b;
        }
        return new Block();
    }

    public void SetMaxStack(int maxStack)
    {
        BlockMaxStack = maxStack;
    }
}
