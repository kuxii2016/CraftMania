using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject chunckPrefab;
    public int viewRange = 30;
    int collumnHeight = 60;

    public GameObject BlockHighlight;

    public LayerMask layerMask;
    int gridSize = 1;

    bool once = true;
    void Update () 
    {
        if (Mathf.FloorToInt(Time.time) % 5 == 0 && once)
        {
            for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunck.Width * gridSize)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunck.Width * gridSize)
                {
                    int xx = Mathf.FloorToInt(x / Chunck.Width) * Chunck.Width;
                    int zz = Mathf.FloorToInt(z / Chunck.Width) * Chunck.Width;

                    Chunck chunck = Chunck.GetChunck(Mathf.FloorToInt(xx), 0, Mathf.FloorToInt(zz));
                    if (chunck == null)
                    {
                        for (int y = 0; y < collumnHeight; y++)
                        {
                            int yr = (y * Chunck.Height) /*- (y)*/;

                            for (int ix = 0; ix < gridSize; ix++)
                            {
                                for (int iz = 0; iz < gridSize; iz++)
                                {
                                    Instantiate(chunckPrefab, new Vector3(xx + (Chunck.Width * ix), yr, zz + (Chunck.Width * iz)), Quaternion.identity);
                                }
                            }
                        }
                    }
                }
            }
            once = false;
        }
        else
        {
            once = true;
        }

        BlockController();

        Chunck c = Chunck.GetChunck(10,90,10);
        if (c == null)
            return;
    }

    void BlockController()
    {
        if (Chunck.Blockworking) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7f, layerMask))
        {
            Vector3 p = hit.point - hit.normal / 2;
            BlockHighlight.transform.position = new Vector3(Mathf.Floor(p.x), Mathf.Floor(p.y), Mathf.Floor(p.z));

            if (Input.GetMouseButtonDown(0))
            {
                SetBlock(p, null);
            }
            if (Input.GetMouseButtonDown(1))
            {
                p = hit.point + hit.normal / 2;
                SetBlock(p, Block.getBlock("Stone"));
            }
        }
        else
        {
            BlockHighlight.transform.position = new Vector3(0, -1000, 0);
        }
    }

    void SetBlock(Vector3 p, Block b)
    {
        Chunck chunck = Chunck.GetChunck(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z));
        Vector3 localPos = chunck.transform.position - p;

        if ((Mathf.FloorToInt(localPos.x) * -1) == (Chunck.Width))
        {
            Chunck c = Chunck.GetChunck(Mathf.FloorToInt(p.x + 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }
        else
        {
            Chunck c = Chunck.GetChunck(Mathf.FloorToInt(p.x - 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }

        chunck.SetBlock(p + new Vector3(+1, 0, 0), b);
    }
}
