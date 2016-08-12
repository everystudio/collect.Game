using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkitRoot : Singleton<SkitRoot> {

	public enum STEP
	{
		NONE		,
		SETUP		,
		DISP		,
		STAND		,
		NAME		,
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
		STAND		,
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
	public GameObject m_goStand;
	public UILabel m_lbTextWindow;
	public UILabel m_lbTextAll;
	public UILabel m_lbTextStand;
	public UILabel m_lbTextStandName;
	public ButtonBase m_btnNextWindow;
	public ButtonBase m_btnPrevWindow;
	public ButtonBase m_btnEndWindow;
	public ButtonBase m_btnNextAll;
	public ButtonBase m_btnPrevAll;
	public ButtonBase m_btnEndAll;
	public ButtonBase m_btnNextStand;
	public ButtonBase m_btnPrevStand;
	public ButtonBase m_btnEndStand;

	public UILabel m_lbTextName;
	public UILabel m_lbText;
	public ButtonBase m_btnPrev;
	public ButtonBase m_btnNext;
	public ButtonBase m_btnEnd;

	public UI2DSprite m_sprStandLeft;
	public UI2DSprite m_sprStandRight;

	public string m_strMessage;
	public string m_strNameBuf;
	public string m_strMessageBuf;

	public List<CsvScriptParam> m_scriptParamList = new List<CsvScriptParam> ();
	public int m_iIndex;
	public int m_iMessageIndex;
	public float m_fTimer;
	public bool m_bClose;

	public void SkitStart( List<CsvScriptParam>  _scriptParamList , TYPE _eType ){
		// いったん閉じる
		Close ();
		m_eStep = STEP.CHECK;
		m_iIndex = 0;
		m_scriptParamList = _scriptParamList;
		m_goRoot.SetActive (true);

		m_goWindow.SetActive (false);
		m_goAll.SetActive (false);
		m_goStand.SetActive (false);

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

		case TYPE.STAND:
			AdManager.Instance.ShowIcon (GameMain.Instance.m_eAdType, false);
			m_goStand.SetActive (true);
			m_fMessageSpeed = DataManager.Instance.config.ReadFloat ("message_speed_window");
			m_lbText = m_lbTextStand;
			m_btnPrev = m_btnPrevStand;
			m_btnNext = m_btnNextStand;
			m_btnEnd = m_btnEndStand;
			break;

		default:
			break;
		}
		m_btnPrev.gameObject.SetActive (false);
		m_btnEnd.gameObject.SetActive (false);

		m_lbText.text = "";

		callStand (ref m_iIndex);
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

	private void callStand( ref int _iIndex ){
		if (_iIndex == m_scriptParamList.Count) {
			return;
		}
		if (!m_scriptParamList [m_iIndex].command.Equals ("stand")) {
			return;
		}

		if (m_scriptParamList [m_iIndex].param.Equals ("")) {
			if (m_scriptParamList [m_iIndex].option1.Equals ("left")) {
				Debug.LogError ("left hide");
				m_sprStandLeft.gameObject.SetActive (false);
			} else {
				m_sprStandRight.gameObject.SetActive (false);
				Debug.LogError ("right hide");
			}
		} else {
			// 強制でstandフォルダ以下とします
			string strLoadFilename = string.Format ("stand/{0}", m_scriptParamList [m_iIndex].param);
			Debug.LogError (strLoadFilename);
			if (m_scriptParamList [m_iIndex].option1.Equals ("left")) {
				m_sprStandLeft.gameObject.SetActive (true);
				m_sprStandLeft.sprite2D = SpriteManager.Instance.Load (strLoadFilename);
			} else {
				m_sprStandRight.gameObject.SetActive (true);
				m_sprStandRight.sprite2D = SpriteManager.Instance.Load (strLoadFilename);
			}
		}
		_iIndex += 1;
		callStand (ref _iIndex);
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
			m_eStep = STEP.CHECK;
			break;

		case STEP.CHECK:
			m_strNameBuf = "";
			if (m_iIndex == m_scriptParamList.Count) {
				m_eStep = STEP.END;
			} else {
				string command = m_scriptParamList [m_iIndex].command;
				if (command.Equals ("stand")) {
					//m_eStep = STEP.STAND;
					callStand (ref m_iIndex);
					//Debug.LogError ("goto stand");
				} else if (command.Equals ("name")) {
					m_eStep = STEP.NAME;
				} else {
					m_eStep = STEP.DISP;
				}
			}
			break;

		case STEP.STAND:
			m_eStep = STEP.CHECK;

			break;

		case STEP.NAME:
			m_strNameBuf = m_scriptParamList [m_iIndex].param;
			m_iIndex += 1;
			m_eStep = STEP.DISP;
			break;

		case STEP.DISP:
			
			if (bInit) {
				if (!m_strNameBuf.Equals ("") || !m_scriptParamList [m_iIndex].option1.Equals ("")) {
					m_strNameBuf = !m_strNameBuf.Equals ("") ? m_strNameBuf : m_scriptParamList [m_iIndex].option1;
				} else {
					m_strNameBuf = "";
				}

				m_lbTextName.text = m_strNameBuf;
				m_strMessageBuf = "";
				//m_strMessageBuf = m_scriptParamList [m_iIndex].param;
				string [] arr = m_scriptParamList [m_iIndex].param.Split ('|');
				foreach (string str in arr) {
					m_strMessageBuf += str;
					m_strMessageBuf += "\n";
				}

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

				if (m_scriptParamList.Count == m_iIndex + 1) {
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
				if (m_iIndex == m_scriptParamList.Count) {
					m_eStep = STEP.END;
				} else {
					m_eStep = STEP.CHECK;
				}
			} else if (m_btnPrev.ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_iIndex -= 1;
				m_eStep = STEP.CHECK;
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











