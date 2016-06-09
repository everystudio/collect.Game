using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectMain : WindowBase {

	public enum STEP
	{
		NONE			= 0,
		IDLE			,
		DECIDE			,
		END				,
		MAX				,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public UIGrid m_gridSelectRoot;

	public string m_strSelectKey;
	public string m_strSelectValue;

	public List<SelectButton> m_selectButtonList = new List<SelectButton>();

	static public string GetSelectKey( string _strParam ){
		return string.Format ("select{0}", _strParam);//);
	}

	public void Initialize( ref List<CsvScriptParam> _scriptParamList ){

		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;

		// 内部から呼び出す
		WindowStart ();

		m_strSelectKey = GetSelectKey (_scriptParamList [0].param);
		int iSelectNum = int.Parse (_scriptParamList [0].option1);

		_scriptParamList.RemoveAt (0);

		for (int i = 0; i < iSelectNum; i++) {

			SelectButton script = PrefabManager.Instance.MakeScript<SelectButton> ("prefab/SelectButton", m_gridSelectRoot.gameObject);
			script.Initialize (_scriptParamList [0]);
			m_selectButtonList.Add (script);
			_scriptParamList.RemoveAt (0);
		}
		m_gridSelectRoot.enabled = true;
		return;

	}

	protected override void windowStart ()
	{
		// closeボタンいらない
		m_btnClose.gameObject.SetActive (false);

		m_ctrlMessage.Initialize ("", "");
	}

	protected override void windowEnd ()
	{
		
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
				foreach (SelectButton selectButton in m_selectButtonList) {
					selectButton.TriggerClear ();
				}
			}
			foreach (SelectButton selectButton in m_selectButtonList) {
				if (selectButton.ButtonPushed) {
					m_strSelectValue = selectButton.Index.ToString ();
					m_eStep = STEP.DECIDE;
				}
			}
			break;
		case STEP.DECIDE:
			if (bInit) {
				DataManager.Instance.kvs_data.Write (m_strSelectKey, m_strSelectValue);
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
			}
			m_eStep = STEP.END;
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




















