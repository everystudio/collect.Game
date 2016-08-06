using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using NendUnityPlugin.AD;

public class TitleMain : MonoBehaviourEx {

	public enum STEP{
		NONE			= 0,
		APPEAR			,
		REVIEW			,
		IDLE			,

		START			,
		CONTINUE		,
		SAVEDATA		,
		SAVEDATA_SELECT	,

		GOTO_GAME		,

		MAX				,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public GameObject m_goSaveRoot;
	public GameObject m_goContinue;

	[SerializeField]
	private GameObject m_goDispRoot;
	private CtrlReview m_ctrlReview;

	[SerializeField]
	ButtonManager m_ButtonManager;

	public PageBase m_pageActive;
	// Use this for initialization
	void Start () {
		m_eStep = STEP.APPEAR;
		m_eStepPre = STEP.MAX;

		m_ButtonManager.ButtonInit ();
		m_ButtonManager.TriggerClearAll ();

		if (!DataManager.Instance.kvs_data.HasKey (DataManager.Instance.KEY_SCRIPT_ID) || DataManager.Instance.kvs_data.ReadInt (DataManager.Instance.KEY_SCRIPT_ID) == 0 ) {
			m_goContinue.SetActive (false);
		}
		NendAdInterstitial.Instance.Show(DataManager.Instance.SPOTID_GAMESTART);
	}
	
	// Update is called once per frame
	void Update () {
		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}
		switch (m_eStep) {
		case STEP.APPEAR:
			m_eStep = STEP.IDLE;
			if (DataManager.Instance.kvs_data.ReadInt ("review") == 0) {
				int iReviewCount = DataManager.Instance.kvs_data.ReadInt ("review_count");
				iReviewCount += 1;
				if (DataManager.Instance.config.ReadInt ("review_interval") < iReviewCount) {
					iReviewCount = 0;
					m_eStep = STEP.REVIEW;
				}
				DataManager.Instance.kvs_data.WriteInt ("review_count", iReviewCount);
			}
			break;

		case STEP.REVIEW:
			if (bInit) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_POPUP);
				m_ctrlReview = PrefabManager.Instance.MakeScript<CtrlReview> ("prefab/Review", m_goDispRoot);
				string strReviewUrl = "";
				#if UNITY_ANDROID
				strReviewUrl = DataManager.Instance.config.Read ("review_url_android");
				#elif UNITY_IOS
				strReviewUrl = DataManager.Instance.config.Read( "review_url_ios" );
				#endif
				m_ctrlReview.Initialize (strReviewUrl);
			}
			if (m_ctrlReview.IsEnd ()) {
				if (m_ctrlReview.GetReviewType () == CtrlReview.REVIEW_TYPE.REVIEW) {
					DataManager.Instance.kvs_data.WriteInt ("review", 2); 
				}
				m_eStep = STEP.IDLE;
			}
			break;

		case STEP.IDLE:
			if (bInit) {
				m_ButtonManager.TriggerClearAll ();
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
			}
			if (m_ButtonManager.ButtonPushed) {
				int index = m_ButtonManager.Index;
				m_eStep = STEP.START;
				if (index == 0) {
					m_eStep = STEP.START;
					GoogleAnalytics.Instance.Log (DataManager.Instance.GA_GAMESTART_START);
				} else if (index == 1) {
					m_eStep = STEP.CONTINUE;
					GoogleAnalytics.Instance.Log (DataManager.Instance.GA_GAMESTART_CONTINUE);
				} else if (index == 2) {
					m_eStep = STEP.SAVEDATA;
					GoogleAnalytics.Instance.Log (DataManager.Instance.GA_GAMESTART_SAVELOAD);
				} else {
				}
			}
			break;
		case STEP.START:
			if (bInit) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_DECIDE);

				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_SCRIPT_ID, 0);
				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_SCRIPT_INDEX, 0);
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
			}
			m_eStep = STEP.GOTO_GAME;
			break;

		case STEP.CONTINUE:
			if (bInit) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_DECIDE);
			}
			m_eStep = STEP.GOTO_GAME;
			break;
		case STEP.SAVEDATA:
			if (bInit) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CURSOR);
			}
			m_eStep = STEP.SAVEDATA_SELECT;
			break;
		case STEP.SAVEDATA_SELECT:
			if (bInit) {
				//AdManager.Instance.ShowBanner( m_eAdType, false);
				m_pageActive = PrefabManager.Instance.MakeObject ("prefab/SaveMain", m_goSaveRoot).GetComponent<PageBase> ();
				m_pageActive.gameObject.GetComponent<SaveMain> ().PageStartSaveMain (true);
			}
			if (m_pageActive.IsEnd ()) {
				if (m_pageActive.gameObject.GetComponent<SaveMain> ().m_bDecide) {
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_DECIDE);
					m_eStep = STEP.GOTO_GAME;
				} else {
					m_eStep = STEP.IDLE;
					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				}
				Release (m_pageActive.gameObject);
			}
			break;

		case STEP.GOTO_GAME:
			if (bInit) {
				Debug.LogError ("here");
				SceneManager.LoadScene ("game");
			}
			break;
		case STEP.MAX:
		default:
			break;
		}
	
	}
}






