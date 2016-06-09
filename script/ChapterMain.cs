using UnityEngine;
using System.Collections;
using NendUnityPlugin.AD;

public class ChapterMain : PageBase {

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

	private ButtonBase m_closeButton;

	public UILabel m_lbTitle;

	public UIGrid m_grid;

	private ButtonManager m_btnManager;

	private ChapterCheck m_chapterCheck;

	public override void PageStart ()
	{
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
		base.PageStart ();
		m_lbTitle.text = "チャプター選択";
		m_closeButton = PrefabManager.Instance.MakeObject ("prefab/CloseButton" , gameObject ).GetComponent<ButtonBase>();
		m_closeButton.TriggerClear ();
		if (m_btnManager == null) {
			m_btnManager = gameObject.AddComponent<ButtonManager> ();
		}
		m_btnManager.ButtonRefresh ();

		foreach (CsvChapterParam param in DataManager.Instance.csv_chapter.list) {
			ChapterBanner script = PrefabManager.Instance.MakeScript<ChapterBanner> ("prefab/ChapterBanner", m_grid.gameObject);
			script.Initialize (param);
			m_btnManager.AddButtonBase (script.gameObject);
		}
		m_btnManager.ButtonInit ();
		NendAdInterstitial.Instance.Show();

	}

	public override void PageEnd ()
	{
		base.PageEnd ();
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
				m_closeButton.TriggerClear ();
				m_btnManager.TriggerClearAll ();
			}
			if (m_btnManager.ButtonPushed) {
				m_eStep = STEP.CHECK;
				//m_btnManager.Index;
			}
			if (m_closeButton.ButtonPushed) {
				m_eStep = STEP.END;
			}
			break;

		case STEP.CHECK:
			if (bInit) {
				CsvChapterParam param = DataManager.Instance.csv_chapter.list [m_btnManager.Index];
				m_chapterCheck = PrefabManager.Instance.MakeScript<ChapterCheck> ("prefab/ChapterCheck", gameObject);
				m_chapterCheck.Initialize (param);
			}
			if (m_chapterCheck.IsEnd ()) {
				Destroy (m_chapterCheck.gameObject);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.END:
			if (bInit) {
				m_bIsEnd = true;
				NendAdInterstitial.Instance.Show();
			}
			break;

		case STEP.MAX:
		default:
			break;
		}
	}
}











