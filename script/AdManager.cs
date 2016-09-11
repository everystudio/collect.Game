using UnityEngine;
using System.Collections;
using NendUnityPlugin.AD;
using NendUnityPlugin.AD.Native;

public class AdManager : Singleton<AdManager> {

	#region IMobile
	/**
	 * IMOBILE用
	 * */
	const string IMOBILE_ICON_PID = "43442";
	const string IMOBILE_ICON_MID = "260633";
	const string IMOBILE_ICON_SID = "818133";

	const string IMOBILE_BANNER_PID = "43442";
	const string IMOBILE_BANNER_MID = "260633";
	const string IMOBILE_BANNER_SID = "818129";

	const string IMOBILE_RECTANGLE_PID = "43442";
	const string IMOBILE_RECTANGLE_MID = "260633";
	const string IMOBILE_RECTANGLE_SID = "818132";

	#endregion



	protected override bool GetDontDestroy()
	{
		return false;
	}

	public override void Initialize ()
	{
		base.Initialize ();
		m_goNendNativePanel.SetActive (false);
		m_nendBanner = m_goNendBanner.GetComponent<NendAdBanner> ();
		m_nendRectangle = m_goNendRectangle.GetComponent<NendAdBanner> ();

	}
	public GameObject m_goNendBanner;
	public GameObject m_goNendRectangle;
	public GameObject m_goNendNative;
	public GameObject m_goNendNativePanel;

	private NendAdBanner m_nendBanner;
	private NendAdBanner m_nendRectangle;
	private NendAdNative m_nendNative;

	public bool m_bAdDispIcon;
	public bool m_bAdDispBanner;
	public bool m_bAdDispRectangle;

	public int m_iIMobileIcon;
	public int m_iIMobileBanner;
	public int m_iIMobileRectangle;

	#if UNITY_ANDROID
	public bool m_bIsIcon;
	#endif


	public bool ShowIcon(DataManager.AD_TYPE _eAdType,bool _bDisp ){

		if (m_bAdDispIcon == _bDisp) {
			return false;
		}

		switch (_eAdType) {
		case DataManagerBase<DataManager>.AD_TYPE.NEND:
		#if UNITY_ANDROID
			m_goNendNativePanel.SetActive( _bDisp );
		#elif UNITY_IPHONE
			m_goNendNativePanel.SetActive( _bDisp );
		#endif
			break;
		case DataManagerBase<DataManager>.AD_TYPE.IMOBILE:
			#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
			#if USE_IMOBILE
			if( m_iIMobileIcon == 0 && _bDisp){
				// スポット情報を設定します
				IMobileSdkAdsUnityPlugin.registerInline(IMOBILE_ICON_PID, IMOBILE_ICON_MID, IMOBILE_ICON_SID);
				// 広告の取得を開始します
				IMobileSdkAdsUnityPlugin.start(IMOBILE_ICON_SID);
				// アイコン表示パラメータを作成します
				var iconParams = new IMobileIconParams();
				iconParams.iconNumber = 2;
				// 広告の表示位置を指定して表示します
				m_iIMobileIcon = IMobileSdkAdsUnityPlugin.show(IMOBILE_ICON_SID,
					IMobileSdkAdsUnityPlugin.AdType.ICON,
					IMobileSdkAdsUnityPlugin.AdAlignPosition.RIGHT,
					IMobileSdkAdsUnityPlugin.AdValignPosition.TOP,
					iconParams);
			}
			else {
				IMobileSdkAdsUnityPlugin.setVisibility(m_iIMobileIcon, _bDisp);
			}
			#endif
			#endif
			break;
		}
		m_bAdDispIcon = _bDisp;
		return true;
	}

	public bool ShowBanner(DataManager.AD_TYPE _eAdType,bool _bDisp){
		if (m_bAdDispBanner == _bDisp) {
			return false;
		}
		switch (_eAdType) {
		case DataManagerBase<DataManager>.AD_TYPE.NEND:
			if (_bDisp) {
				m_nendBanner.Show ();
			} else {
				m_nendBanner.Hide ();
			}
			break;
		case DataManagerBase<DataManager>.AD_TYPE.IMOBILE:
			#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
			#if USE_IMOBILE
			if( m_iIMobileBanner == 0 && _bDisp ){
			// スポット情報を設定します

			IMobileSdkAdsUnityPlugin.registerInline(IMOBILE_BANNER_PID, IMOBILE_BANNER_MID, IMOBILE_BANNER_SID);
			// 広告の取得を開始します
			IMobileSdkAdsUnityPlugin.start(IMOBILE_BANNER_SID);
			// 広告の表示位置を指定して表示します(画面中央下)
			m_iIMobileBanner = IMobileSdkAdsUnityPlugin.show(IMOBILE_BANNER_SID,
			IMobileSdkAdsUnityPlugin.AdType.BANNER,
			IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER,
			IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM);
			}
			else {
				IMobileSdkAdsUnityPlugin.setVisibility(m_iIMobileBanner, _bDisp);
			}
			#endif
			#endif
			break;
		}
		m_bAdDispBanner = _bDisp;
		return true;
	}


	public void ShowRectangle( DataManager.AD_TYPE _eAdType , bool _bDisp){

		if (m_bAdDispRectangle == _bDisp) {
			return;
		}		
		switch( _eAdType ){
		case DataManagerBase<DataManager>.AD_TYPE.NEND:
			if (_bDisp) {
				m_nendRectangle.Show ();
			} else {
				m_nendRectangle.Hide ();
			}
			break;

		case DataManagerBase<DataManager>.AD_TYPE.IMOBILE:
			#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
		#if USE_IMOBILE
			if (_bDisp && m_iIMobileRectangle == 0 ) {
			// スポット情報を設定します
			IMobileSdkAdsUnityPlugin.registerInline(IMOBILE_BANNER_PID, IMOBILE_BANNER_MID, IMOBILE_BANNER_SID);
			// 広告の取得を開始します
			IMobileSdkAdsUnityPlugin.start(IMOBILE_BANNER_SID);
			// 広告の表示位置を指定して表示します(画面中央)
			m_iIMobileRectangle = IMobileSdkAdsUnityPlugin.show(IMOBILE_BANNER_SID,
				IMobileSdkAdsUnityPlugin.AdType.MEDIUM_RECTANGLE,
				IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER,
				IMobileSdkAdsUnityPlugin.AdValignPosition.MIDDLE);
			}
			else {
				IMobileSdkAdsUnityPlugin.setVisibility(m_iIMobileRectangle, _bDisp);
			}
		#endif
			#endif
			break;
		case DataManagerBase<DataManager>.AD_TYPE.MAX:
		default:
			break;
		}
		m_bAdDispRectangle = _bDisp;
		return;
	}


}
