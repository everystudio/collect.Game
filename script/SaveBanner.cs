using UnityEngine;
using System.Collections;

public class SaveBanner : ButtonBase {

	public enum TYPE
	{
		NONE		= 0,
		SAVE		,
		LOAD		,
		MAX			,
	}
	public TYPE m_eType;
	public int m_iNo;

	public enum STEP
	{
		NONE		= 0,
		IDLE		,
		CHECKING	,
		REFREASH	,

		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public UILabel m_lbNo;
	public UILabel m_lbTime;
	public UI2DSprite m_sprBase;
	public UI2DSprite m_sprImage;
	public string m_strImage;
	private string m_strSaveTime;

	private CsvSave m_csvSave;

	public void CheckStart(SaveBanner.TYPE _eType){
		m_eStep = STEP.CHECKING;
	}
	void OnDestroy(){
		SpriteManager.Instance.Unload (m_strImage);
		return;
	}

	public void Initialize(SaveBanner.TYPE _eType , int _iNo , string _strImage , string _strTime ){
		if (_strImage.Equals ("") == true) {
			m_strImage = string.Format (CsvSave.PNG_NAME_FORMAT, _iNo);
		} else {
			m_strImage = _strImage;
		}

		m_csvSave = new CsvSave ();
		m_csvSave.Load (_iNo);
		if (_strTime.Equals ("") == true && m_csvSave.m_bExistData == true) {
			m_strSaveTime = m_csvSave.Read ("save_time");
		} else if (_strTime.Equals ("") == false) {
			m_strSaveTime = _strTime;
		} else {
			m_strSaveTime = "データがありません";
		}
		m_lbTime.text = m_strSaveTime;

		m_sprImage.sprite2D = SpriteManager.Instance.Load (m_strImage);

		m_lbNo.text = string.Format ("No.{0:D3}", _iNo);

		TriggerClear ();

		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;

		if (_eType == TYPE.SAVE) {
			m_sprBase.color = new Color ((188.0f / 255.0f), 1.0f, 0.0f);
		} else {
			m_sprBase.color = new Color (1.0f, (132.0f / 255.0f), 0.0f);
		}
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
		case STEP.IDLE:
			if (bInit) {
			}
			break;
		case STEP.CHECKING:
			if (bInit) {
			}
			break;
		case STEP.REFREASH:
			break;
		case STEP.END:
			break;
		case STEP.MAX:
		default:
			break;
		}
	
	}
}
