using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GameObject chunckPrefab;
	public int viewRange = 30;

	void Start () {
	
	}
	

	void Update () 
	{
		for (float x = transform.position.z - viewRange; x < transform.position.x + viewRange; x += Chunck.Width) 
		{
			for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunck.Width) 
			{
				int xx = Mathf.FloorToInt (x / Chunck.Width) * Chunck.Width;
				int zz = Mathf.FloorToInt (z / Chunck.Width) * Chunck.Width;

				Chunck chunck = Chunck.GetChunck (Mathf.FloorToInt (xx), 0, Mathf.FloorToInt (zz));
				if (chunck == null) 
				{
					Instantiate (chunckPrefab, new Vector3 (xx, 0, zz), Quaternion.identity);
				}
			}
		}	
	}
}
