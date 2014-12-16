using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Utilities : MonoBehaviour {
	
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//	
	public static List<GameObject> GetChildren(GameObject go)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform tran in go.transform)
        {
            children.Add(tran.gameObject);
        }
        return children;
    }

	public static string GetValidTableName(string raw){
		string filename = Path.GetFileNameWithoutExtension(raw);
		filename = filename.Replace(' ', '-').Replace('.', '-');
		return filename;
	}

}
