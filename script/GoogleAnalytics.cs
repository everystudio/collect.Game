using UnityEngine;
using System.Collections;

public class GoogleAnalytics : GoogleAnalyticsBase<GoogleAnalytics> {

	public override void Initialize ()
	{
		base.Initialize ();
		propertyID_Common  = "UA-77286676-7";
		propertyID_Android = "UA-77286676-12";
		propertyID_iOS     = "UA-77286676-13";

		bundleID ="jp.everystudio.collect.song";
		appName = "壊れた声でも希望を唄って";
		appVersion = "1";

		Invoke ("heartbeat", HEARTBEAT_INTERVAL);

	}

	public void LogScriptStart( int _iScriptId , int _iScriptIndex ){
		Log (string.Format (DataManager.Instance.GA_SCRIPT_START_FORMAT, _iScriptId, _iScriptIndex));
	}

}
