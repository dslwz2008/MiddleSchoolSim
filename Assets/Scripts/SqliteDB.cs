using UnityEngine;
using System;
using System.Collections;
using System.Data;
//using Mono.Data.Sqlite;
using Mono.Data.SqliteClient;

//C# class for accessing SQLite objects.
public class SqliteDB {
	// variables for basic query access
	private string connection;
	private IDbConnection dbcon;
	private IDbCommand dbcmd;
	private IDataReader reader;

	public void OpenDB(string path){
		connection = "URI=file:" + path; // we set the connection to our database
		dbcon = (IDbConnection)new SqliteConnection(connection);
		dbcon.Open();
	}
	
	public ArrayList ReadLimitLines(string tableName, int startLine, int lineNumbers){
		string query = "SELECT * FROM " + tableName + " LIMIT " + startLine + "," + lineNumbers;
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
		ArrayList readArray = new ArrayList();
		while(reader.Read()){
			var lineArray = new ArrayList();
			for (int i = 0; i < reader.FieldCount; i++){
				lineArray.Add(reader.GetValue(i)); // This reads the entries in a row
			}
			readArray.Add(lineArray); // This makes an array of all the rows
		}
		return readArray; // return matches
	}
		
	public void UpdateSpecificValue(string tableName, string valueColName, string value,
	string clauseColName, string clauseColValue ){
		string query;
		query = "UPDATE " + tableName + " SET " + valueColName + " = " + value +
			" WHERE " + clauseColName + " = \"" + clauseColValue + "\"";
			Debug.Log(query);
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
	}

	public IDataReader BasicQuery(string q, bool r){ // run a baic Sqlite query
		dbcmd = dbcon.CreateCommand(); // create empty command
		dbcmd.CommandText = q; // fill the command
		reader = dbcmd.ExecuteReader(); // execute command which returns a reader
		if(r){ // if we want to return the reader
			return reader; // return the reader
		}else{
			return null;
		}
	}

	public ArrayList ReadFullTable(string tableName){
		string query;
		query = "SELECT * FROM " + tableName;
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
		ArrayList readArray = new ArrayList();
		while(reader.Read()){
			var lineArray = new ArrayList();
			for (int i = 0; i < reader.FieldCount; i++){
				lineArray.Add(reader.GetValue(i)); // This reads the entries in a row
			}
			readArray.Add(lineArray); // This makes an array of all the rows
		}
		return readArray; // return matches
	}

	// This function deletes all the data in the given table.  Forever.  WATCH OUT! Use sparingly, if at all
	public void DeleteTableContents(string tableName){
		string query;
		query = "DELETE FROM " + tableName;
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
	}

	public void CreateTable(string name, ArrayList col, ArrayList colType){ // Create a table, name, column array, column type array
		string query = "CREATE TABLE " + name + "(" + col[0] + " " + colType[0];
		for(int i = 1; i < col.Count; i++){
			query += ", " + col[i] + " " + colType[i];
		}
		query += ")";
		dbcmd = dbcon.CreateCommand(); // create empty command
		dbcmd.CommandText = query; // fill the command
		reader = dbcmd.ExecuteReader(); // execute command which returns a reader
	}

	public void InsertIntoSingle(string tableName, 
		string colName, string value){ // single insert
		string query = "INSERT INTO " + tableName + "(" + colName + ") " + "VALUES (" + value + ")";
		dbcmd = dbcon.CreateCommand(); // create empty command
		dbcmd.CommandText = query; // fill the command
		reader = dbcmd.ExecuteReader(); // execute command which returns a reader
	}

	public void InsertIntoSpecific(string tableName, 
		ArrayList col, ArrayList values){ // Specific insert with col and values
		string query;
		query = "INSERT INTO " + tableName + "(" + col[0];
		for(int i = 1; i < col.Count; i++){
			query += ", " + col[i];
		}
		query += ") VALUES (" + values[0];
		for(int i = 1; i< values.Count; i++){
			query += ", " + values[i];
		}
		query += ")";
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
	}

	public void InsertInto(string tableName, ArrayList values){ // basic Insert with just values
		string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
		for(int i = 1; i<values.Count; i++){
			query += ", " + values[i];
		}
		query += ")";
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
	}
	
	// This function reads a single column
	//  wCol is the WHERE column, wPar is the operator you want to use to compare with,
	//  and wValue is the value you want to compare against.
	//  Ex. - SingleSelectWhere("puppies", "breed", "earType", "=", "floppy")
	//  returns an array of matches from the command: SELECT breed FROM puppies WHERE earType = floppy;
	public ArrayList SingleSelectWhere(string tableName, string itemToSelect,
		string wCol, string wPar, string wValue){ // Selects a single Item
		string query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + wCol + wPar + wValue;
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
		ArrayList readArray = new ArrayList();
		while(reader.Read()){
			readArray.Add(reader.GetString(0)); // Fill array with all matches
		}
		return readArray; // return matches
	}

	public void CloseDB(){
		reader.Close(); // clean everything up
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbcon.Close();
		dbcon = null;
	}
}