using UnityEngine;
using System.Collections;

public class GoogleAnalytics : GoogleAnalyticsBase<GoogleAnalytics> {

	public override void Initialize ()
	{
		base.Initialize ();
		propertyID_Common = "UA-77286676-7";
		propertyID_Android = "UA-77286676-14";
		propertyID_iOS = "UA-77286676-15";

		bundleID ="jp.everystudio.collect.mister";
		appName = "壊すミスターラビットの夢";
		appVersion = "1";

		Invoke ("heartbeat", HEARTBEAT_INTERVAL);

	}

	public void LogScriptStart( int _iScriptId , int _iScriptIndex ){
		Log (string.Format (DataManager.Instance.GA_SCRIPT_START_FORMAT, _iScriptId, _iScriptIndex));
	}

}
