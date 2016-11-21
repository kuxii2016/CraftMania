using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject chunckPrefab;
    public static GameObject ChunckPrefab;
    public static int viewRange = 100;
    public GameObject BlockHighlight;
    public LayerMask layerMask;
    public CharacterController characterController;
    bool once = true;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8);

        characterController.enabled = false;

        ChunckPrefab = chunckPrefab;

        for (float x = transform.position.x - (Chunck.Width * 3); x < transform.position.x + (Chunck.Width * 3); x += Chunck.Width)
        {
            for (float y = transform.position.y - (Chunck.Height * 3); y < transform.position.y + (Chunck.Height * 3); y += Chunck.Height)
            {
                for (float z = transform.position.z - (Chunck.Width * 3); z < transform.position.z + (Chunck.Width * 3); z += Chunck.Width)
                {
                    int xx = Mathf.FloorToInt(x / Chunck.Width) * Chunck.Width;
                    int zz = Mathf.FloorToInt(z / Chunck.Width) * Chunck.Width;
                    int yy = Mathf.FloorToInt(y / Chunck.Height) * Chunck.Height;

                    if (!Chunck.ChunckExists(xx, yy, zz))
                    {
                        Instantiate(chunckPrefab, new Vector3(xx, yy, zz), Quaternion.identity);

                    }
                }
            }
        }
    }

    void Update()
    {
        if (Time.time > 5)
            characterController.enabled = true;
        BlockController();
        ChunkSpawn();
    }

    void ChunkSpawn()
    {
        if (Chunck.working) return;

        Chunck c = null;
        float lastDistance = 9999999f;
        for (int i = 0; i < Chunck.Chuncks.Count; i++)
        {
            float d = Vector3.Distance(this.transform.position, Chunck.Chuncks[i].transform.position);
            if (d < lastDistance)
            {
                Chunck cc = Chunck.Chuncks[i].GetComponent<Chunck>();
                if (cc.ready == false)
                {
                    lastDistance = d;
                    c = cc;
                }
            }
        }
        if (c != null)
        {
            c.StartFunction();
        }
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
