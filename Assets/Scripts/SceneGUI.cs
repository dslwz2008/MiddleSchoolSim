using UnityEngine;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System;

[ExecuteInEditMode]
public class SceneGUI : MonoBehaviour {

	public Texture2D texturePlay;
	public Texture2D textureStop;
	public Texture2D textureImportGPS;
	public Texture2D textureImportRFID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUILayout.BeginVertical("box");
		{
			//控制路径回放的功能
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(texturePlay, GUILayout.Width(48), GUILayout.Height(48))){

			}
			if(GUILayout.Button(textureStop, GUILayout.Width(48), GUILayout.Height(48))){

			}
			GUILayout.EndHorizontal();

			//导入数据文件功能
			GUILayout.BeginHorizontal();			
			if(GUILayout.Button(textureImportGPS, GUILayout.Width(48), GUILayout.Height(48))){
				//Camera.main.nearClipPlane = 1.0f;
				OpenFileDialog dlgOpenGPS = new OpenFileDialog();
				dlgOpenGPS.InitialDirectory = "URI=file:" + UnityEngine.Application.dataPath;
				dlgOpenGPS.Filter = "csv files (*.csv)|*.csv";
				dlgOpenGPS.RestoreDirectory = true ;				
				if (dlgOpenGPS.ShowDialog() == DialogResult.OK)
				{
					//打开文件，写到数据库
					try 
					{
						using (StreamReader sr = new StreamReader(dlgOpenGPS.FileName)) 
						{
							SqliteDB db = new SqliteDB();
							db.OpenDB("data.db");
							string tableName = Utilities.GetValidTableName(dlgOpenGPS.FileName);
							ArrayList cols = new ArrayList();
							cols.Add("id");
							cols.Add("lon");
							cols.Add("lat");
							cols.Add("alt");
							cols.Add("time");
							ArrayList types = new ArrayList();
							types.Add("INTEGER");
							types.Add("FLOAT");
							types.Add("FLOAT");
							types.Add("FLOAT");
							types.Add("DATETIME");
							db.CreateTable(tableName, cols, types);
							string line;
							while ((line = sr.ReadLine()) != null) 
							{
								string[] items = line.Split(new char[]{','});
								Debug.Log(items);
							}
							db.CloseDB();
						}
					}
					catch (Exception e) 
					{
						Debug.Log(e.Message);
					}
				}
			}
			if(GUILayout.Button(textureImportRFID, GUILayout.Width(48), GUILayout.Height(48))){
				
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
}
