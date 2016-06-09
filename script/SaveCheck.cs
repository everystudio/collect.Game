using UnityEngine;
using System.Collections;

public class SaveCheck : PageBase {

	public enum STEP
	{
		NONE		= 0,
		OPEN		,
		IDLE		,
		CLOSE		,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public UILabel m_lbText;

	public GameObject m_goDispRoot;
	private SaveBanner m_saveBanner;

	public ButtonBase m_btnYes;
	public ButtonBase m_btnNo;

	public bool m_bDecide;		// 実行されたかどうか

	private SaveBanner.TYPE m_eType;
	private int m_iNo;

	private string m_strSaveTime;
	private bool m_bLoadOnly;

	public void Initialize( SaveBanner.TYPE _eType , int _iNo , string _strTime , bool _bLoadOnly){
		m_eStep = STEP.OPEN;
		m_eStepPre = STEP.MAX;

		m_strSaveTime = TimeManager.StrGetTime ();
		m_bLoadOnly = _bLoadOnly;

		m_eType = _eType;
		m_iNo = _iNo;
		m_bDecide = false;

		string strImage = "";
		switch (_eType) {
		case SaveBanner.TYPE.SAVE:
			m_lbText.text = "データを保存しますか？";
			strImage = SaveMain.TEMP_SCREENSHOT_NAME;
			break;
		case SaveBanner.TYPE.LOAD:
			m_lbText.text = "データをロードしますか？";
			m_strSaveTime = _strTime;
			break;
		case SaveBanner.TYPE.MAX:
		default:
			break;
		}

		m_goDispRoot.transform.localScale = Vector3.zero;
		m_saveBanner = PrefabManager.Instance.MakeScript<SaveBanner> ("prefab/SaveBanner", m_goDispRoot);
		m_saveBanner.Initialize (_eType, _iNo, strImage, m_strSaveTime);
		return;
		
	}

	// Update is called once per frame
	void Update () {

		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre = m_eStep;
			bInit = true;
		}
		switch (m_eStep) {
		case STEP.OPEN:
			if (bInit) {
				TweenScale ts = TweenScale.Begin (m_goDispRoot, 0.2f, Vector3.one);
				EventDelegate.Set (ts.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.IDLE:
			if (bInit) {
				m_btnYes.TriggerClear ();
				m_btnNo.TriggerClear ();
				m_bDecide = false;
			}
			if (m_btnNo.ButtonPushed) {
				m_eStep = STEP.CLOSE;
			}

			if (m_btnYes.ButtonPushed) {
				m_bDecide = true;
				if (m_eType == SaveBanner.TYPE.SAVE) {
					GoogleAnalytics.Instance.Log (DataManager.Instance.GA_SAVE);

					string filename = SaveMain.TEMP_SCREENSHOT_NAME;
					string save_filename = string.Format (CsvSave.PNG_NAME_FORMAT, m_iNo);
					string write_path = "";
					string path = "";

					switch (Application.platform) {
					case RuntimePlatform.IPhonePlayer:
						path = Application.persistentDataPath + "/" + filename;
						write_path = Application.persistentDataPath + "/" + save_filename;
						break;
					case RuntimePlatform.Android:
						path = Application.persistentDataPath + "/" + filename;
						write_path = Application.persistentDataPath + "/" + save_filename;
						break;
					default:
						path = Application.persistentDataPath + "/" + filename;
						write_path = Application.persistentDataPath + "/" + save_filename;
						break;
					}
					//Debug.LogError (write_path);
					if (System.IO.File.Exists (write_path)) {
						//Debug.LogError ("exist");
						System.IO.File.Delete (write_path);
						SpriteManager.Instance.Unload (save_filename);
					}
					if (System.IO.File.Exists (path)) {
						System.IO.File.Copy (path, write_path);
						//System.IO.File.Delete (path);
					}
					GameMain.Instance.Save (m_iNo , m_strSaveTime );
				} else if (m_eType == SaveBanner.TYPE.LOAD) {
					GoogleAnalytics.Instance.Log (DataManager.Instance.GA_LOAD);

					if (m_bLoadOnly) {
						GameMain.LoadOnly (m_iNo);
					} else {
						GameMain.Instance.Load (m_iNo, m_bLoadOnly);
					}
				} else {
				}
				m_eStep = STEP.CLOSE;
			}
			break;
		case STEP.CLOSE:
			if (bInit) {
				m_bEndTween = false;
				TweenScale ts = TweenScale.Begin (m_goDispRoot, 0.2f, Vector3.zero);
				EventDelegate.Set (ts.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_eStep = STEP.END;
			}
			break;
		case STEP.END:
			if (bInit) {
				m_bIsEnd = true;
			}
			break;

		case STEP.MAX:
		default:
			break;
		}
	
	}
}
