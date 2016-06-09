using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class TargetSpeedup : WindowBase {

	public enum STEP {
		NONE		= 0,
		IDLE		,
		SHOW_AD		,

		END			,
		MAX			,
	};
	public STEP m_eStep;
	public STEP m_eStepPre;

	public ButtonBase m_btnAd;
	public ButtonBase m_btnMovie;
	#if NO_USE_UNITY_ADS || !(UNITY_ANDROID || UNITY_IOS)
	public ShowResult m_eShowResult;
	#endif

	protected override void windowStart ()
	{
		m_ctrlMessage.Initialize ("ターゲット出現時間短縮！", "動画を視聴して5分間ターゲットの出現回数アップ！");

		GameObject obj = PrefabManager.Instance.MakeObject ("prefab/ShareButton", gameObject);
		obj.GetComponent<ShareButton> ().Initialize ("動画を見る");
		m_btnMovie = obj.GetComponent<ButtonBase> ();
		m_btnMovie.TriggerClear ();
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
	}

	protected override void windowEnd ()
	{
		m_bIsEnd = true;
		gameObject.SetActive (false);
	}

	// Update is called once per frame
	void Update () {

		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}

		switch (m_eStep) {
		case STEP.IDLE:
			if (bInit) {
				m_btnClose.TriggerClear ();
				m_btnMovie.TriggerClear ();
			}
			if (m_btnMovie.ButtonPushed) {
				m_eStep = STEP.SHOW_AD;

			} else if (m_btnClose.ButtonPushed) {
				m_eStep = STEP.END;
			} else {
			}
			break;

		case STEP.SHOW_AD:
			if (bInit) {
				if (false == UnityAdsSupporter.Instance.ShowRewardedAd ()) {
					m_eStep = STEP.IDLE;
				} else {
					string end_time = TimeManager.StrGetTime (60 * 5);
					Debug.Log (end_time);
					DataManager.Instance.kvs_data.Write ( DataManager.Instance.KEY_TARGET_SPEEDUP_END, end_time);
					DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
				}
			}
			#if NO_USE_UNITY_ADS || !(UNITY_ANDROID || UNITY_IOS)
			if (UnityAdsSupporter.Instance.IsShowed (out m_eShowResult)) {
				Debug.Log (m_eShowResult);
				m_eStep = STEP.END;
			}
			#else
			m_eStep = STEP.END;
			#endif
			break;

		case STEP.END:
			if (bInit) {
				WindowEnd ();
			}
			break;

		default:
			break;
		}

	}
}
