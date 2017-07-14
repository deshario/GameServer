using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using MyScript;
using System.Text.RegularExpressions;

public class NetworkManager : MonoBehaviour 
{
	private string User = "";
	private SocketIOComponent socket;
	private ectScript myscript;
	public string PlayerName;
	public bool xx;
	public StatusPlayer Player;
	public GameObject otherPlayer;
	private bool Login = true;
	private string MessLogin = " ";
	private string MessRegister = " ";
	public int selectColor = 0;
	private string[] OptionColor = new string[]{"red","green","blue"};
	string reName = "";
	void Start()
	{
		DontDestroyOnLoad (gameObject);
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		myscript = go.GetComponent<ectScript>();
		socket.On("open", ServerOpen);
		socket.On("error", ServerError);
		socket.On("close", ServerClose);
		socket.On("loginSuccess",loginSuccess);
		socket.On("loginUnsuccess",loginUnsuccess);
		socket.On("swapPlayer",swapPlayer);
		socket.On ("PlayerMove", PlayerMove);
		socket.On ("Playerdisconnected", Playerdisconnected);
		socket.On ("registersuccess", registersuccess);
		socket.On ("registerunsuccess", registerunsuccess);
	}

	public void registersuccess(SocketIOEvent e)
	{
		MessRegister = myscript.jsontoString (e.data [0].ToString (), "\"");
	}
	public void registerunsuccess(SocketIOEvent e)
	{
		MessRegister = myscript.jsontoString (e.data [0].ToString (), "\"");
	}
	public void Playerdisconnected(SocketIOEvent e)
	{
		Destroy(GameObject.Find (myscript.jsontoString (e.data [0].ToString (), "\"")));
	}

	public void ServerOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: " + e.name + ": " + e.data);


	}
	public void Playerconnected(SocketIOEvent e)
	{
		Debug.Log (myscript.jsontoString (e.data [0].ToString (), "\"")+ "has connected");
		//Debug.Log ("has connected");
	}

	public void PlayerMove(SocketIOEvent e)
	{
		//Debug.Log ("Move");
		Debug.Log("Player " + myscript.jsontoString (e.data [0].ToString (), "\"") + " : " + myscript.jsontoString (e.data [1].ToString (), "\"") );
		GameObject newObj = GameObject.Find (myscript.jsontoString (e.data [0].ToString (), "\""));
		newObj.GetComponent<OtherPlayer>().currentPosition = myscript.StringtoVector3(myscript.jsontoString(e.data [1].ToString (),"\""));
	}
	
	public void ServerError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " : " + e.data);
	}

	public void ServerClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " : " + e.data);
	}

	void loginSuccess(SocketIOEvent e)
	{
		PlayerName = myscript.jsontoString( e.data [0].ToString (),"\"");
		Player.Name = PlayerName;
		Player.ID = myscript.jsontoString(e.data [1].ToString (),"\"");
		Player.MyColor = myscript.jsontoString(e.data [3].ToString (),"\"");
		Debug.Log ("Your ID : " + PlayerName);
		Debug.Log ("Your Position : " + e.data [2].ToString ());

		if (myscript.jsontoString(e.data [2].ToString (),"\"") == "null") 
		{
			Player.startPosition = Vector3.zero;

		} else 
		{
			Player.startPosition = myscript.StringtoVector3(myscript.jsontoString(e.data [2].ToString (),"\""));
		}

		Player.gameObject.transform.position = Player.startPosition;

		Login = false;
		Application.LoadLevel(1);

                                     
	}

	void loginUnsuccess(SocketIOEvent e)
	{
		MessLogin = e.data[0].ToString();
	}

	void swapPlayer(SocketIOEvent e)
	{
		if(Application.loadedLevel != 0)
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
	void OnGUI()
	{
		if (Login) {
			User = GUI.TextField (new Rect (60, 10, 100, 20), User);

			if (GUI.Button (new Rect (10, 10, 50, 20), "Start")) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["name"] = User;

				socket.Emit ("login", new JSONObject (data));
			}

			GUI.Label (new Rect (10, 40, 100, 20), MessLogin);
			
			
			reName = GUI.TextField (new Rect (60, 200, 100, 20), reName);
			GUI.Label (new Rect (10, 200, 100, 20), "Name");
			GUI.Label (new Rect (10, 230, 100, 20), "Color");
			GUI.Label (new Rect (10, 170, 100, 20), "-----Register----");
			GUI.Label (new Rect (30, 300, 100, 20), MessRegister);
			selectColor = GUI.SelectionGrid (new Rect (60, 230, 160, 20), selectColor, OptionColor, OptionColor.Length, GUI.skin.toggle);
			
			if (GUI.Button (new Rect (10, 260, 100, 20), "Register")) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["name"] = reName;
				switch (selectColor) {
				case 0:
					data ["color"] = "red";
					break;
				case 1:
					data ["color"] = "green";
					break;
				case 2:
					data ["color"] = "blue";
					break;
				}
				
				socket.Emit ("register", new JSONObject (data));
			}
		}


	
	}
}
