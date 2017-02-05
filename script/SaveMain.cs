using UnityEngine;
using System.Collections;
using System.IO;
using NendUnityPlugin.AD;

public class SaveMain : PageBase {
	static public readonly string TEMP_SCREENSHOT_NAME = "save_screenshot_temp.png";

	public enum STEP {
		NONE		= 0,
		SCREENSHOT	,
		IDLE		,
		SAVE_PAGE	,
		SAVE_CHECK	,
		LOAD_PAGE	,
		LOAD_CHECK	,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;
	public GameObject m_goDispRoot;
	private bool m_bScreenshot;
	public ButtonBase m_closeButton;

	public SaveBanner.TYPE m_eType;
	public GameObject m_goUpLayer;
	private SaveBannerRoot m_saveBannerRoot;

	private SaveCheck m_saveCheck;
	private int m_iCheckNo;
	private string m_strSaveTime;

	public ButtonBase m_btnTabSave;
	public ButtonBase m_btnTabLoad;

	public bool m_bLoadOnly;
	public bool m_bDecide;

	public override void PageStart ()
	{
		m_bLoadOnly = false;
		base.PageStart ();
		m_goDispRoot.SetActive (false);
		m_eStep = STEP.SCREENSHOT;
		m_eStepPre = STEP.MAX;
		if (m_saveBannerRoot != null) {
			Destroy (m_saveBannerRoot.gameObject);
		}
		m_saveBannerRoot = null;

		m_bDecide = false;

		return;
	}

	public void PageStartSaveMain( bool _bLoadOnly ){
		PageStart ();

		m_bLoadOnly = _bLoadOnly;
	}

	// 呼ばれてないねぇ
	public override void PageEnd ()
	{
		base.PageEnd ();
		//Debug.Log (TEMP_SCREENSHOT_NAME);
		m_bIsEnd = true;
		return;
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
				StartCoroutine (startScreenshot ());
				m_bScreenshot = false;
				EditDirectory.MakeDirectory (CsvSave.FILE_DIRECTORY);
			}
			if (m_bScreenshot) {
				m_goDispRoot.SetActive (true);
				m_eType = SaveBanner.TYPE.SAVE;
				if (m_bLoadOnly) {
					m_btnTabSave.gameObject.SetActive (false);
					m_eType = SaveBanner.TYPE.LOAD;
				}
				m_closeButton = PrefabManager.Instance.MakeObject ("prefab/CloseButton", m_goDispRoot).GetComponent<ButtonBase> ();
				m_closeButton.transform.localPosition = new Vector3 (0.0f, -47.0f, 0.0f);
				m_eStep = STEP.IDLE;
				if (m_bLoadOnly == false) {
					//NendAdInterstitial.Instance.Show (DataManager.Instance.SPOTID_MENU);
						AdManager.Instance.CallInterstitial();
					}
				}
			break;
		case STEP.IDLE:
			if (m_eType == SaveBanner.TYPE.SAVE) {
				m_eStep = STEP.SAVE_PAGE;
			} else {
				m_eStep = STEP.LOAD_PAGE;
			}
			m_eStep = STEP.SAVE_PAGE;
			break;
		case STEP.SAVE_PAGE:
			if (bInit) {
				if (m_saveBannerRoot == null) {
					m_saveBannerRoot = PrefabManager.Instance.MakeScript<SaveBannerRoot> ("prefab/SaveBannerRoot", m_goDispRoot);
				}
				// ここの10は特に可変にはしてません
				m_saveBannerRoot.Initialize (m_eType, 10);

				m_saveBannerRoot.TriggerClearAll ();
				m_closeButton.TriggerClear ();

				m_btnTabSave.TriggerClear ();
				m_btnTabLoad.TriggerClear ();
			}
			if (m_saveBannerRoot.ButtonPushed) {
				m_iCheckNo = m_saveBannerRoot.Index + 1;	// あんまよくないけどここで補正
				m_saveBannerRoot.TriggerClearAll ();
				m_eStep = STEP.SAVE_CHECK;

				bool bSound = true;
				if (m_eType == SaveBanner.TYPE.LOAD) {
					CsvSave save = new CsvSave ();
					save.Load (m_iCheckNo);
					if (false == save.m_bExistData) {
						m_eStep = m_eStepPre;
						bSound = false;
					} else {
						m_strSaveTime = save.Read ("save_time");
					}
				}
				if (bSound) {
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CURSOR);
				} else {
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				}
			}
			if (m_btnTabLoad.ButtonPushed) {
				m_eType = SaveBanner.TYPE.LOAD;
				m_eStep = STEP.IDLE;
			}
			if (m_btnTabSave.ButtonPushed) {
				m_eType = SaveBanner.TYPE.SAVE;
				m_eStep = STEP.IDLE;
			}
			if (m_closeButton.ButtonPushed) {
				m_eStep = STEP.END;
			}
			break;
		case STEP.SAVE_CHECK:
			if (bInit) {
				m_saveCheck = PrefabManager.Instance.MakeScript<SaveCheck> ("prefab/SaveCheck", gameObject);
				m_saveCheck.Initialize (m_eType, m_iCheckNo , m_strSaveTime , m_bLoadOnly);
			}
			if (m_saveCheck.IsEnd ()) {
				m_bDecide = m_saveCheck.m_bDecide;
				m_eStep = STEP.IDLE;
				if ( m_bDecide) {
					if (m_bLoadOnly) {
						m_eStep = STEP.END;
					}
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_DECIDE);
				} else {
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				}
				Destroy (m_saveCheck.gameObject);
			}
			break;
		case STEP.LOAD_PAGE:
			if (m_closeButton.ButtonPushed) {
				m_eStep = STEP.END;
			}
			break;
		case STEP.LOAD_CHECK:
			break;
		case STEP.END:
			if (bInit) {
				SpriteManager.Instance.Unload (TEMP_SCREENSHOT_NAME);
				if (m_bLoadOnly == false) {
					//NendAdInterstitial.Instance.Show (DataManager.Instance.SPOTID_MENU);
				}
				m_bIsEnd = true;
			}
			break;
		case STEP.MAX:
		default:
			break;
		}
	}


	IEnumerator startScreenshot () {
		string filename = TEMP_SCREENSHOT_NAME;
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
		m_bScreenshot = true;
	}


}







