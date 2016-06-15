using UnityEngine;
using System.Collections;

public class GoogleAnalytics : GoogleAnalyticsBase<GoogleAnalytics> {

	public override void Initialize ()
	{
		base.Initialize ();
		propertyID_Common = "UA-77286676-7";
		propertyID_Android = "UA-77286676-5";
		propertyID_iOS = "UA-77286676-6";

		bundleID ="jp.everystudio.collect.summer";
		appName = "ナツヤスミ、いりませんか？";
		appVersion = "1";

	}

	public void LogScriptStart( int _iScriptId , int _iScriptIndex ){
		Log (string.Format (DataManager.Instance.GA_SCRIPT_START_FORMAT, _iScriptId, _iScriptIndex));
	}

}
