using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DC_GameRoom : MonoBehaviour 
{
	public DynaRoom dRoom;

	public void SetRoomSize(float width, float length = 0)
	{
		bool isS = (length == 0 || width == length);

		dRoom.width = width;
		dRoom.length = length;
		dRoom.isSquare = isS;
	}
}
