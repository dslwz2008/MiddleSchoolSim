using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
		//数据库表名中不能含有数字
		string alphabeta = "abcdefghij";//0123456789
		string filename = Path.GetFileNameWithoutExtension(raw);
		StringBuilder result = new StringBuilder(filename);
		//去掉特殊字符
		for(int i=0; i < filename.Length; i++){
			if(char.IsNumber(filename[i])){
				result[i] = alphabeta[int.Parse(filename[i].ToString())];
			}else if(char.IsLetter(filename[i])){
				result[i] = filename[i];
			}else{
				result[i] = 'z';
			}
		}
		return result.ToString();
	}

}
