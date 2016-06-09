using UnityEngine;
using System.Collections;

public class ChapterCheck : PageBase {

	public enum STEP
	{
		NONE		= 0,
		IDLE		,
		CHECK		,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public UILabel m_lbText;

	public GameObject m_goDispRoot;
	private ChapterBanner m_chapterBanner;

	public ButtonBase m_btnYes;
	public ButtonBase m_btnNo;

	private int m_iScriptId;

	public void Initialize(CsvChapterParam _param){
		m_iScriptId = _param.script_id;
		m_chapterBanner = PrefabManager.Instance.MakeScript<ChapterBanner> ("prefab/ChapterBanner", gameObject);
		m_chapterBanner.Initialize (_param);

		if (m_chapterBanner.m_bAble == false) {
			m_lbText.text = "まだこのチャプターは\n開始できません";
			m_btnYes.gameObject.SetActive (false);
		}
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
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
				m_btnYes.TriggerClear ();
				m_btnNo.TriggerClear ();
			}
			if (m_btnYes.ButtonPushed) {
				GameMain.Instance.ResetScriptId (m_iScriptId);
				m_eStep = STEP.END;
			}
			if (m_btnNo.ButtonPushed) {
				m_eStep = STEP.END;
			}
			break;
		case STEP.CHECK:
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














