using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class StatusPlayer : MonoBehaviour 
{
	public string Name = " ";
	public string ID;
	public string MyColor;
	public Vector3 startPosition;
	private SocketIOComponent socket;
	public Renderer rend;

	public float timeUpPosition;
	void Start () 
	{
		Name = " ";
		DontDestroyOnLoad (gameObject);
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On("error", ServerError);
		socket.On("close", ServerClose);
		transform.position = startPosition;
		rend = GetComponent<Renderer>();
	}



	void Update () 
	{
		if (Application.loadedLevel != 0) {
			if (timeUpPosition < 60) {

				timeUpPosition += Time.deltaTime;
			} else {
				socket.Emit ("UpdatePosition");
				timeUpPosition = 0;
			}
		}

		switch (MyColor) 
		{
		case "red": { rend.material.color = Color.red;
		}break;
		case "blue": { rend.material.color = Color.blue;
		}break;
		case "green": { rend.material.color = Color.green;
		}break;
		}

		if (Input.GetAxis ("Vertical")!= 0||Input.GetAxis ("Horizontal")!= 0)
		{
			transform.Translate(new Vector3(Input.GetAxis ("Vertical") * 3 * Time.deltaTime,0,Input.GetAxis ("Horizontal") * 3 * Time.deltaTime));
			Dictionary<string, string> data = new Dictionary<string, string>();
			data["position"] = transform.position.x.ToString()+","+transform.position.y.ToString()+","+transform.position.z.ToString() ;
			socket.Emit("move",new JSONObject(data));
		}

	}

	public void ServerError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " : " + e.data);
	}
	
	public void ServerClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " : " + e.data);
	}
}
