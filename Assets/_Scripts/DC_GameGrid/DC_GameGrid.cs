using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid : NetworkBehaviour
{
	[System.Serializable]
	public enum GridSize 
	{
		One,
		Two,
		Three
	}

	[SyncVar]
	public GridSize gridSize = GridSize.One;

	[SyncVar]
	public float gridCellSize;

	[HideInInspector]
	public SyncListGridCell cells = new SyncListGridCell();

	public bool CheckPosition(int x, int y)
	{
		if(cells.Count < 1)
			return false;

		if(x < 1 || x > 3 || y < 1 || y > 3)
			return false;

		return (cells[(x * y) - 1].player != null);
	}
}

[System.Serializable]
public struct GameGridCell 
{
	public GameObject player;
	public Vector3 cellPos;
	public int x;
	public int y;
}

public class SyncListGridCell : SyncListStruct<GameGridCell>{}
