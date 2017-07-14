using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace MyScript
{
	public class ectScript: MonoBehaviour
	{
		public Vector3 StringtoVector3(string target)
		{
			Vector3 newVector;

			string[] newS = Regex.Split (target, ",");

			newVector = new Vector3 (float.Parse (newS [0]), float.Parse (newS [1]), float.Parse (newS [2]));

			return newVector;
		}

		public string jsontoString(string target,string s)
		{
			string[] newS = Regex.Split (target, s);

			return newS [1];
		}
	}
}
