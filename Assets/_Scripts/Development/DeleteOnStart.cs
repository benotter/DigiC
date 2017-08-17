using UnityEngine;
public class DeleteOnStart : MonoBehaviour 
{
	public GameObject[] objectsToDelete = new GameObject[1];
	void Start () 
	{
		foreach(GameObject g in objectsToDelete)
			Destroy(g);

		Destroy(gameObject);
	}
}
