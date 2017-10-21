using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_HR_Equipment_Base : MonoBehaviour 
{
	public virtual void OnJoinGame(DC_Player player) {}
	public virtual void OnLeaveGame() {}

	public virtual void OnRoundStart() {}
	public virtual void OnRoundEnd() {}

	public virtual void OnGainAvatarSpawn(DC_Avatar_Spawn avatarS) {}
	public virtual void OnLoseAvatarSpawn() {}

	public virtual void OnGainAvatar(DC_Avatar avatar) {}
	public virtual void OnLoseAvatar() {}
	
}
