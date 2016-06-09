using UnityEngine;
using System.Collections;

public class DataTargetParam : CsvDataParam{

	public enum STATUS
	{
		NONE		= 0,
		IDLE		,
		ACTIVE		,
		DESTROIED	,
		MAX			,
	}
	public int serial { get; set; }
	public int status { get; set; }
	public string time_destroy { get; set; }

}

public class DataTarget : CsvData<DataTargetParam>{

	public void Insert( DataTargetParam _param ){
		list.Add (_param);
		return;
	}

}




