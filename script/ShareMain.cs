using UnityEngine;
using System.Collections;
using System.IO;
using NendUnityPlugin.AD;

public class ShareMain : PageBase {

	public enum STEP
	{
		NONE		= 0,
		SCREENSHOT	,
		IDLE		,
		SHARE		,
		TWITTER		,
		FACEBOOK	,
		LINE		,
		OTHER		,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;
	public ButtonBase m_closeButton;

	public GameObject m_goDispRoot;

	public UILabel m_lbText;
	public UILabel m_lbTextDesc;
	public UIGrid m_gridButton;
	public ButtonManager m_btnManager;

	private bool m_bScreenshot;

	//private SocialConnector.ServiceType m_eServiceType;
	private bool m_bShareEnd;

	public readonly string[] BUTTON_LIST = new string[]{
		"シェアする",
		/*
		"Twitter",
		"Facebook",
		"Line",
		"Other",
		*/
	};

	public override void PageStart ()
	{
		m_eStep = STEP.SCREENSHOT;
		m_eStepPre = STEP.MAX;
		base.PageStart ();

		m_lbText.text = "シェアしてリンゴ出現！";
		if (DataManager.Instance.config.HasKey ("share_title")) {
			m_lbText.text = DataManager.Instance.config.Read ("share_title");
		}
		m_lbTextDesc.text = "シェアするとリンゴが出現してストーリーを進めることができます";
		if (DataManager.Instance.config.HasKey ("share_text")) {
			m_lbTextDesc.text = DataManager.Instance.config.Read ("share_text");
		}
		m_closeButton = PrefabManager.Instance.MakeObject ("prefab/CloseButton" , m_goDispRoot ).GetComponent<ButtonBase>();
		m_btnManager.ButtonRefresh ();
		foreach( string strText in BUTTON_LIST ){
			GameObject obj = PrefabManager.Instance.MakeObject ("prefab/ShareButton", m_gridButton.gameObject);
			obj.GetComponent<ShareButton> ().Initialize (strText);
			m_btnManager.AddButtonBase (obj);
		}
		m_gridButton.enabled = true;
		m_btnManager.ButtonInit ();
	}

	public override void PageEnd ()
	{
		base.PageEnd ();
		m_bIsEnd = true;
	}
	
	// Update is called once per frame
	void Update () {
		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}

		switch (m_eStep) {
		case STEP.SCREENSHOT:
			if (bInit) {
				m_bScreenshot = false;
				StartCoroutine (startScreenshot ());
			}
			if (m_bScreenshot) {
				m_eStep = STEP.IDLE;
				//NendAdInterstitial.Instance.Show(DataManager.Instance.SPOTID_MENU);
					AdManager.Instance.CallInterstitial();
					m_goDispRoot.SetActive (true);
			}
			break;

		case STEP.IDLE:
			if (bInit) {
				m_closeButton.TriggerClear ();
				m_btnManager.TriggerClearAll ();
			}
			if (m_closeButton.ButtonPushed) {
				//NendAdInterstitial.Instance.Show(DataManager.Instance.SPOTID_MENU);

				m_eStep = STEP.END;
			} else if (m_btnManager.ButtonPushed) {

				m_eStep = STEP.SHARE;
				m_btnManager.TriggerClearAll ();
			} else {
				;//nothing
			}
			break;

		case STEP.SHARE:
			if (bInit) {
				m_bShareEnd = false;
				#if UNITY_EDITOR
				m_bShareEnd = true;
				#endif

				string strMessage = "【泣けるアプリ】切なくて泣ける、青春ラブストーリー！\n\n思いを打ち明けれない青年「鈴木」\nその彼に厚意を寄せる「野田」\n\n二人の行く末はどうなってしまうのか。 #シークレットアップル";
				string strImage = "screenshot.png";
				#if UNITY_ANDROID
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_MESSAGE_ANDROID )){
					strMessage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_MESSAGE_ANDROID);
				}
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_IMAGE_ANDROID )){
					strImage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_IMAGE_ANDROID);
				}
				#elif UNITY_IPHONE
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_MESSAGE_IPHONE )){
					strMessage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_MESSAGE_IPHONE);
				}
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_IMAGE_IPHONE )){
					strImage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_IMAGE_IPHONE);
				}
				#endif
				StartCoroutine (startShare (strMessage , strImage));
			}
			if (m_bShareEnd == true) {
				ManagerTarget.Instance.AppearAll ();
				m_eStep = STEP.IDLE;
			}
			break;

		case STEP.END:
			if (bInit) {
				m_closeButton.TriggerClear ();
				PageEnd ();
			}
			break;

		case STEP.MAX:
		default:
			break;

		}

	}

	IEnumerator startScreenshot () {
		string filename = "screenshot.png";
		string write_path = "";
		string path = "";
		string capture_path = "";

		switch (Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			capture_path = filename;
			path = Application.persistentDataPath + "/" + filename;
			write_path = path;
			break;
		case RuntimePlatform.Android:
			capture_path = filename;
			path = Application.persistentDataPath + "/" + filename;
			write_path = path;
			break;
		default:
			capture_path = Application.persistentDataPath + "/" + filename;
			path = Application.persistentDataPath + "/" + filename;
			write_path = Application.persistentDataPath + "/" + filename;
			break;
		}

		// 存在する場合は一度削除
		if (System.IO.File.Exists (path)) {
			System.IO.File.Delete (path);
		}
		//Application.CaptureScreenshot( write_path );
		Application.CaptureScreenshot( capture_path );
		do {
			yield return new WaitForSeconds (0.5f);
		} while (false == System.IO.File.Exists (path));
		// スクリーンショットの読み込み
		byte[] image = File.ReadAllBytes(path);
		//string path = Application.persistentDataPath + "/image.png";
		File.WriteAllBytes(write_path, image);
		// Texture2D を作成して読み込み
		Texture2D tex = new Texture2D(0, 0);
		tex.LoadImage(image);
		m_bScreenshot = true;
	}

	IEnumerator startShare ( string _strMessage , string _strImage ) {
		//string filename = "screenshot.png";
		string path = "";

		switch (Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			path = Application.persistentDataPath + "/" + _strImage;
			break;
		case RuntimePlatform.Android:
			path = Application.persistentDataPath + "/" + _strImage;
			break;
		default:
			path = Application.persistentDataPath + "/" + _strImage;
			break;
		}
		SocialConnector.Share (
			_strMessage,
			null, 
			path
		);
		m_bShareEnd = true;
		yield return 0;
	}

}
