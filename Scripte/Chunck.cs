using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using System.Threading;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunck : MonoBehaviour
{
    public Block[,,] map;

    public bool[,,] lightMap;
    public int[,,] lightValue; //0 - 16

    public static int Width = 20, Height = 20;

    public static List<Transform> Chuncks = new List<Transform>();

    Chunck left;
    Chunck right;
    Chunck back;
    Chunck front;
    Chunck bottom;
    Chunck top;

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

    public bool iamWorking = false;

    void Start()
    {

        Chuncks.Add(this.transform);
        pos = transform.position;

        if (Time.time < 1)
        {
            StartFunction();
        }
    }

    GameObject pl;  
    Vector3 pos;
    public static bool spawningChuncksS = false;
    public bool IamspawningChuncksS = false;

    void Update()
    {
        if (pl == null)
            pl = GameObject.FindGameObjectWithTag("Player");

        if (pl != null)
        {
            if (Vector3.Distance(this.transform.position, pl.transform.position) > PlayerController.viewRange)
            {
                Chuncks.Remove(this.transform);
                if (iamWorking == true)
                    working = false;
                if (IamspawningChuncksS == true)
                    spawningChuncks = false;
                Destroy(this.gameObject);
            }
        }

        if (Time.time < 1f || spawningChuncksS == true || spawningChuncks == true) return;
        if (Vector3.Distance(pos, pl.transform.position) > PlayerController.viewRange - Width) return;

        if (left == null || right == null || front == null || back == null ||
        bottom == null || top == null)
        {
            if (left == null)
            {
                int x = Mathf.FloorToInt((transform.position.x - Chunck.Width) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        left = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (right == null)
            {
                int x = Mathf.FloorToInt((transform.position.x + Chunck.Width) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        right = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (top == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y + Chunck.Height) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        top = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (bottom == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y - Chunck.Height) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        bottom = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (back == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z - Chunck.Width) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        back = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (front == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z + Chunck.Width) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(x, y, z))
                    {
                        front = GetChunck(x, y, z);
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
        }
    }

    public void StartFunction()
    {
        mesh = new Mesh();
        StartCoroutine(CalculateMap());
    }

    public static bool spawningChuncks = false;

    public IEnumerator SpawnChunck(Vector3 pos)
    {
        spawningChuncks = true;

        GameObject.Instantiate(PlayerController.ChunckPrefab, pos, Quaternion.identity);

        yield return 0;

        spawningChuncks = false;
    }

    public static Thread tmap;

    public IEnumerator CalculateMap()
    {
        if (System.IO.Directory.Exists(Application.dataPath + "\\saves" ))
        {
            Directory.CreateDirectory(Application.dataPath + "\\saves\\");
        }

        if (tmap != null && tmap.IsAlive)
        {

        }
        else
        {
            bool createFile = true;
            if (System.IO.File.Exists(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString()))
            {
                createFile = false;
            }
            else
            {

            }

            working = true;
            iamWorking = true;

            if (createFile == false)
            {
                lightMap = new bool[Width, Height, Width];
                map = new Block[Width, Height, Width];

                string[] linhas = File.ReadAllLines(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString());

                int i = 0;
                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            int blockID = -1;

                            blockID = int.Parse(linhas[i]);

                            if (blockID == -1)
                                map[x, y, z] = null;
                            else
                                map[x, y, z] = Block.getBlock(blockID);

                            i++;
                        }
                    }
                }
            }
            else if (createFile == true)
            {
                tmap = new Thread(CMap);
                tmap.Start();

                while (tmap.IsAlive)
                {
                    yield return 0;
                }

                if (!Directory.Exists(Application.dataPath + "\\saves\\" + GameManager.worldName))
                {
                    Directory.CreateDirectory(Application.dataPath + "\\saves\\" + GameManager.worldName);
                }

                if (!System.IO.File.Exists(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + "seed"))
                {
                    StreamWriter f = File.CreateText(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + "seed");
                    f.Write(GameManager.seed);
                    f.Close();
                }

                if (!System.IO.File.Exists(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + "gameversion"))
                {
                    StreamWriter f = File.CreateText(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + "gameversion");
                    f.Write("1.0.0.1");
                    f.Close();
                }

                File.Create(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString()).Close();
                List<string> linhas = new List<string>();

                TextWriter c = new StreamWriter(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString());

                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (map[x, y, z] == null)
                                c.WriteLine("-1");
                            //linhas.Add(-1 + ";");
                            else
                                c.WriteLine(map[x, y, z].BlockID);
                            //linhas.Add(map[x,y,z].BlockID + ";");
                        }
                    }
                }

                c.Close();
                //File.WriteAllLines(Application.dataPath + "\\" + pos.ToString(), linhas.ToArray());
            }

            tmap = null;

            generatedMap = true;

            yield return 0;

            StartCoroutine(CalculateMesh());
        }
    }

    public void CMap()
    {
        System.Random r = new System.Random();
        working = true;

        lightMap = new bool[Width, Height, Width];
        map = new Block[Width, Height, Width];

        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Block bloco = GetTheoreticalBlock(pos + new Vector3(x, y, z));
                    map[x, y, z] = bloco;

                    if (map[x, y, z] == Block.getBlock("Dirt") && GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                    {
                        map[x, y, z] = Block.getBlock("Grass");
                    }
                }
            }
        }
    }

    public IEnumerator CalculateMesh()
    {
        mesh = new Mesh();

        tmap = new Thread(CMesh);
        tmap.Start();

        while (tmap.IsAlive)
        {
            yield return 0;
        }

        tmap = null;

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(triangulos.ToArray(), 0);
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        yield return new WaitForEndOfFrame();

        ready = true;
        working = false;
        iamWorking = false;
    }

    public void CMesh()
    {
        vertices.Clear();
        triangulos.Clear();
        colors.Clear();
        uvs.Clear();

        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    if (map[x, y, z] != null)
                    {
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
        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    if (map[x, y, z] != null)
                    {
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
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        yield return 0;

        Blockworking = false;
        yield return 0;
    }

    LibNoise.Generator.Perlin noise = new LibNoise.Generator.Perlin(1f, 1f, 1f, 8, GameManager.seed, QualityMode.High);

    LibNoise.Generator.Perlin biome = new LibNoise.Generator.Perlin(0.1f, 2f, 0.5f, 1, GameManager.seed, QualityMode.High);

    public Block GetTheoreticalBlock(Vector3 pos)
    {
        System.Random r = new System.Random(GameManager.seed);

        Vector3 offset = new Vector3((float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000);

        double noiseX = (double)Mathf.Abs((float)(pos.x + offset.x) / 20);
        double noiseY = (double)Mathf.Abs((float)(pos.y + offset.y) / 20);
        double noiseZ = (double)Mathf.Abs((float)(pos.z + offset.z) / 20);

        double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);
        double biomeValue = biome.GetValue(noiseX, 50, noiseZ);

        noiseValue += (200 - (float)pos.y) / 18f;
        noiseValue /= (float)pos.y / 8f;
        if (noiseValue > 0.5f)
        {
            if (noiseValue > 0.6f)
                return Block.getBlock("Stone");
            //if (biomeValue > 0.6f)
            //    return Block.getBlock("SandStone");

            if (biomeValue > 0.1f)
                return Block.getBlock("Sand");
            else
                return Block.getBlock("Dirt");
        }

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

        CalculateLightFront(x, y, z - 1, b);

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
        CalculateLightTop(x, y - 1, z, b);
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
        CalculateLightLeft(x + 1, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    void CalculateLightTop(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        Color colorToAdd = new Color(0, 0, 0);
        bool blockLight = true;
        if (y < 0)
            blockLight = !lightMap[x, 0, z];
        else
            blockLight = !lightMap[x, y, z];

        if (!blockLight)
        {
            colorToAdd = Color.black;
        }

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] = shadowColors + colorToAdd;
                colors[index + 1] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 0] = shadowColors + colorToAdd;
                colors[index + 3] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors + colorToAdd;
                colors[index + 0] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors + colorToAdd;
                colors[index + 3] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z + 1))
            {
                colors[index + 3] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors + colorToAdd;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z - 1))
            {
                colors[index + 0] = shadowColors + colorToAdd;
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
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y, z + 1) && InitialisBlockTransparent(x + 1, y, z))
            {
                colors[index + 1] *= shadowColors;
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y, z - 1) && InitialisBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z + 1) &&
                InitialisBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z - 1) &&
                InitialisBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y + 1, z + 1) &&
                InitialisBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y + 1, z - 1) &&
                InitialisBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
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
            if (!InitialisBlockTransparent(x - 1, y - 1, z) && InitialisBlockTransparent(x - 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y, z + 1) && InitialisBlockTransparent(x - 1, y, z))
            {
                colors[index + 1] *= shadowColors;
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y, z - 1) && InitialisBlockTransparent(x - 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!InitialisBlockTransparent(x - 1, y - 1, z + 1) &&
                InitialisBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y - 1, z - 1) &&
                InitialisBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z + 1) &&
                InitialisBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z - 1) &&
                InitialisBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
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
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y, z - 1) && InitialisBlockTransparent(x, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y, z - 1) && InitialisBlockTransparent(x, y, z - 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z - 1) &&
                InitialisBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y - 1, z - 1) &&
                InitialisBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y + 1, z - 1) &&
                InitialisBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z - 1) &&
                InitialisBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 2] *= shadowColors;
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
            if (!InitialisBlockTransparent(x, y - 1, z + 1) && InitialisBlockTransparent(x, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y, z + 1) && InitialisBlockTransparent(x, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y, z + 1) && InitialisBlockTransparent(x, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z + 1) &&
                InitialisBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y - 1, z + 1) &&
                InitialisBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x + 1, y + 1, z + 1) &&
                InitialisBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z + 1) &&
                InitialisBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
    }

    bool isBlockTransparent(int x, int y, int z)
    {
        if (x >= Width)
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
        if (y >= Height)
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
            if (GetTheoreticalBlock(pos + new Vector3(x, y, z)) == null)
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
        for (int i = 0; i < Chuncks.Count; i++)
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

    public static bool ChunckExists(int x, int y, int z)
    {
        for (int i = 0; i < Chuncks.Count; i++)
        {
            Transform t = Chuncks[i];
            Vector3 pos = new Vector3(x, y, z);
            if (t.position == null)
                return true;
            Vector3 cpos = t.position;

            if (pos.x < cpos.x || pos.y < cpos.y || pos.z < cpos.z || pos.x >= cpos.x + Width || pos.y >= cpos.y + Height || pos.z >= cpos.z + Width)
            {
                continue;
            }

            return true;
        }
        return false;
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
            if (b == null)
            {
                Block mb = map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
                if (mb != null)
                {
                    GameObject g = new GameObject("DropedI" + mb.BlockName) as GameObject;
                    CubeDropGenerator cdg = g.gameObject.AddComponent<CubeDropGenerator>();
                    cdg.StartCube(mb);

                    cdg.transform.position = worldPos - new Vector3(1, 0, 0);
                    cdg.gameObject.AddComponent<Rigidbody>();
                    cdg.GetComponent<Renderer>().material = this.gameObject.GetComponent<Renderer>().material;

                    cdg.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    cdg.gameObject.layer = 8;
                }
            }
            map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = b;

            string[] blocosNoC = File.ReadAllLines(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString());
            int i = 0;
            for (int z = 0; z < Width; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (x == Mathf.FloorToInt(localPos.x) && y == Mathf.FloorToInt(localPos.y) && z == Mathf.FloorToInt(localPos.z))
                        {
                            if (b == null)
                                blocosNoC[i] = "-1";
                            else
                                blocosNoC[i] = b.BlockID.ToString();
                        }
                        i++;
                    }
                }
            }
            File.WriteAllLines(Application.dataPath + "\\saves\\" + GameManager.worldName + "\\" + pos.ToString(), blocosNoC);
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
    public string BlockName;
    public Texture ItemView;
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

        ItemView = Resources.Load<Texture>(name);
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
        ItemView = Resources.Load<Texture>(name);
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
        ItemView = Resources.Load<Texture>(name);
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
        foreach (Block b in BlockList.Blocks)
        {
            if (b.BlockName == name)
                return b;
        }
        return new Block();
    }

    public static Block getBlock(int id)
    {
        foreach (Block b in BlockList.Blocks)
        {
            if (b.BlockID == id)
                return b;
        }
        return new Block();
    }

    public void SetMaxStack(int maxStack)
    {
        BlockMaxStack = maxStack;
    }
}
