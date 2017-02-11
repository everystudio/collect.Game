using UnityEngine;
using System.Collections;

public class GoogleAnalytics : GoogleAnalyticsBase<GoogleAnalytics> {

	public override void Initialize ()
	{
		base.Initialize ();
		propertyID_Android = "UA-77286676-3";
		propertyID_iOS = "UA-77286676-4";

		bundleID ="jp.everystudio.collect.apple";
		appName = "シークレットアップル";
		appVersion = "1";

	}

}
