using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkitRoot : Singleton<SkitRoot> {

	public enum STEP
	{
		NONE		,
		SETUP		,
		DISP		,
		IDLE		,
		CHECK		,
		END			,
		MAX			,
	};
	public STEP m_eStep;
	public STEP m_eStepPre;

	public float m_fMessageSpeed;

	public enum TYPE
	{
		NONE		= 0,
		WINDOW		,
		ALL			,
		MAX			,
	}
	public TYPE m_eType;

	public override void Initialize ()
	{
		base.Initialize ();
	}


	public GameObject m_goRoot;

	public GameObject m_goAll;
	public GameObject m_goWindow;
	public UILabel m_lbTextWindow;
	public UILabel m_lbTextAll;
	public ButtonBase m_btnNextWindow;
	public ButtonBase m_btnPrevWindow;
	public ButtonBase m_btnEndWindow;
	public ButtonBase m_btnNextAll;
	public ButtonBase m_btnPrevAll;
	public ButtonBase m_btnEndAll;

	public UILabel m_lbText;
	public ButtonBase m_btnPrev;
	public ButtonBase m_btnNext;
	public ButtonBase m_btnEnd;

	public string m_strMessage;
	public string m_strMessageBuf;
	public List<string> m_strMessageList = new List<string> ();
	public int m_iIndex;
	public int m_iMessageIndex;
	public float m_fTimer;
	public bool m_bClose;

	public void SkitStart( List<string> _strMessageList , TYPE _eType ){
		// いったん閉じる
		Close ();
		m_eStep = STEP.DISP;
		m_iIndex = 0;
		m_strMessageList = _strMessageList;
		m_goRoot.SetActive (true);

		m_eType = _eType;
		switch (m_eType) {
		case TYPE.WINDOW:
			m_goWindow.SetActive (true);
			m_lbText = m_lbTextWindow;
			m_fMessageSpeed = DataManager.Instance.config.ReadFloat ("message_speed_window");
			m_btnPrev = m_btnPrevWindow;
			m_btnNext = m_btnNextWindow;
			m_btnEnd = m_btnEndWindow;

			break;
		case TYPE.ALL:
			AdManager.Instance.ShowIcon ( GameMain.Instance.m_eAdType , false);
			m_goAll.SetActive (true);
			m_lbText = m_lbTextAll;
			m_fMessageSpeed = DataManager.Instance.config.ReadFloat ("message_speed_all");
			m_btnPrev = m_btnPrevAll;
			m_btnNext = m_btnNextAll;
			m_btnEnd = m_btnEndAll;
			break;
		default:
			break;
		}
		m_btnPrev.gameObject.SetActive (false);
		m_btnEnd.gameObject.SetActive (false);

		m_lbText.text = "";
	}

	public void Close(){
		m_goRoot.SetActive (false);
		m_goWindow.SetActive (false);
		m_goAll.SetActive (false);


	}

	public bool EndSkit(){
		return m_eStep == STEP.END;
	}

	private bool pushedNext(){
		bool bRet = false;
		if (m_btnNextWindow.ButtonPushed) {
			bRet = true;
		} else if (m_btnNextAll.ButtonPushed) {
			bRet = true;
		} else {
		}
		if (bRet == true) {
			SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CURSOR);
		}
		return bRet;
	}
	private bool pushedPrev(){
		bool bRet = false;
		if (m_btnPrevWindow.ButtonPushed) {
			bRet = true;
		} else if (m_btnPrevAll.ButtonPushed) {
			bRet = true;
		} else {
		}
		if (bRet == true) {
			SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
		}
		return bRet;
	}
	private bool pushedEnd(){
		if (m_btnEndWindow.ButtonPushed) {
			return true;
		} else if (m_btnEndAll.ButtonPushed) {
			return true;
		} else {
		}
		return false;
	}
	private void TriggerClearAll(){
		m_btnNextWindow.TriggerClear();
		m_btnPrevWindow.TriggerClear();
		m_btnEndWindow.TriggerClear();
		m_btnNextAll.TriggerClear();
		m_btnPrevAll.TriggerClear();
		m_btnEndAll.TriggerClear();
	}
	// Update is called once per frame
	void Update () {

		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}

		switch (m_eStep) {
		case STEP.NONE:
			break;
		case STEP.SETUP:
			m_eStep = STEP.DISP;
			break;

		case STEP.DISP:
			if (bInit) {
				m_strMessageBuf = m_strMessageList [m_iIndex];
				m_iMessageIndex = 0;
				m_strMessage = "";
				m_fTimer = 0.0f;

				m_btnNext.TriggerClear ();
				m_btnPrev.TriggerClear ();
				m_btnEnd.TriggerClear ();
			}

			bool bToIdle = false;

			m_fTimer += Time.deltaTime;
			if (m_fMessageSpeed < m_fTimer) {
				m_fTimer -= m_fMessageSpeed;
				m_iMessageIndex += 1;
				m_strMessage = m_strMessageBuf.Substring (0, m_iMessageIndex);

				m_lbText.text = m_strMessage;
				if (m_strMessage.Length == m_strMessageBuf.Length) {
					bToIdle = true;
				}
			}

			if (m_btnNext.ButtonPushed) {
				bToIdle = true;
			}
			if (m_btnPrev.ButtonPushed) {
				bToIdle = true;
			}
			if (m_btnEnd.ButtonPushed) {
				bToIdle = true;
			}
			if (bToIdle) {
				m_eStep = STEP.IDLE;
			}
			break;

		case STEP.IDLE:
			if (bInit) {
				if (m_iIndex != 0) {
					m_btnPrev.gameObject.SetActive (true);
				} else {
					m_btnPrev.gameObject.SetActive (false);
				}

				if (m_strMessageList.Count == m_iIndex + 1) {
					m_btnEnd.gameObject.SetActive (true);
				} else {
					m_btnEnd.gameObject.SetActive (false);
				}
				m_strMessage = m_strMessageBuf;
				m_lbText.text = m_strMessage;
				m_btnNext.TriggerClear ();
				m_btnPrev.TriggerClear ();
				m_btnEnd.TriggerClear ();
			}

			if (m_btnNext.ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CURSOR);
				m_iIndex += 1;
				if (m_iIndex == m_strMessageList.Count) {
					m_eStep = STEP.END;
				} else {
					m_eStep = STEP.DISP;
				}
			} else if (m_btnPrev.ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_iIndex -= 1;
				m_eStep = STEP.DISP;
			} else if (m_btnEnd.ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CURSOR);
				m_eStep = STEP.END;
			} else {
			}
			break;
		case STEP.END:
			if (bInit) {
				Close ();
			}
			break;

		case STEP.MAX:
		default:
			break;
		}
		return;
	}
}











