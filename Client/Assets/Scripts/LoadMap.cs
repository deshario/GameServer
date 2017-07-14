using UnityEngine;
using System.Collections;
using SocketIO;
using MyScript;

public class LoadMap : MonoBehaviour 
{
	public GameObject otherPlayer;
	private SocketIOComponent socket;
	private ectScript myscript;

	void Awake()
	{

	}

	void Start () 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		myscript = go.GetComponent<ectScript>();
		socket.Emit("LoadMap");
		socket.On("swapAllPlayer",swapAllPlayer);

	}

	public void swapAllPlayer(SocketIOEvent e)
	{
		if(Application.loadedLevel == 1)
		{
			otherPlayer.GetComponent<OtherPlayer>().Name = myscript.jsontoString( e.data [0].ToString (),"\"");
			otherPlayer.GetComponent<OtherPlayer>().ID = myscript.jsontoString( e.data [1].ToString (),"\"");
			otherPlayer.GetComponent<OtherPlayer>().MyColor = myscript.jsontoString( e.data [3].ToString (),"\"");
			if (myscript.jsontoString(e.data [2].ToString (),"\"") == "null") 
			{
				otherPlayer.GetComponent<OtherPlayer>().startPosition = Vector3.zero;
				
			} else 
			{
				otherPlayer.GetComponent<OtherPlayer>().startPosition = myscript.StringtoVector3(myscript.jsontoString(e.data [2].ToString (),"\""));
			}
			
			Instantiate(otherPlayer,otherPlayer.GetComponent<OtherPlayer>().startPosition,Quaternion.identity);
		}
	}

	void Update () 
	{
	
	}
}
