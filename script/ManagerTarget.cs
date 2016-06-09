using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ManagerTarget: Singleton<ManagerTarget> {

	public enum STEP{
		NONE			= 0,
		IDLE			,
		GENERATE_WAIT	,
		GENERATE		,

		MAX				,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	CsvKvs m_Config = new CsvKvs();
	public List<CtrlTarget> m_targetList = new List<CtrlTarget>();

	public DataTarget m_dataTarget;

	public int m_iGetTarget;
	public int GetTarget( bool _bClear = false ){
		int iRet = m_iGetTarget;
		if (_bClear) {
			m_iGetTarget = 0;
		}
		return iRet;
	}

	public int m_iTargetInterval;
	public int m_iTargetSerial;
	public override void Initialize ()
	{
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
		m_iGetTarget = 0;
		m_iTargetSerial = 0;
		m_Config.Load(CsvConfig.FILE_NAME);
		Debug.Log (Application.persistentDataPath);

		m_iTargetInterval = m_Config.ReadInt ("target_interval");

		m_dataTarget = new DataTarget ();
		m_dataTarget.Load (DataManager.Instance.FILENAME_TARGET_DATA);

		int make_num = m_Config.ReadInt (DataManager.Instance.KEY_TARGET_DEFAULT_NUM);
		if (DataManager.Instance.data_kvs.HasKey (DataManager.Instance.KEY_ADD_TARGET)) {
			int add_num = DataManager.Instance.data_kvs.ReadInt (DataManager.Instance.KEY_ADD_TARGET);
			make_num += add_num;
		}
		//Debug.LogError (make_num);
		AddTargetTail (make_num);
		/*
		for (int i = 0; i < make_num ; i++) {
			int iSerial = i + 1;
			DataTargetParam targetParam = m_dataTarget.SelectOne (string.Format ("serial = {0}", iSerial));
			if (targetParam.serial == 0) {
				targetParam.serial = iSerial;
				targetParam.status = (int)DataTargetParam.STATUS.ACTIVE;
				targetParam.time_destroy = TimeManager.StrGetTime ();
				m_dataTarget.list.Add (targetParam);
			}
			m_targetList.Add (generateTarget (targetParam));
		}
		*/

		Debug.Log (m_dataTarget.list.Count);
		m_dataTarget.Save (DataManager.Instance.FILENAME_TARGET_DATA);
	}
	public CtrlTarget generateTarget( DataTargetParam _param ){
		GameObject objTarget = PrefabManager.Instance.MakeObject ("prefab/objTarget", gameObject);
		CtrlTarget script = objTarget.GetComponent<CtrlTarget> ();

		script.initialize (_param.serial,
			m_Config.ReadFloat ("xmin"),
			m_Config.ReadFloat ("xmax"),
			m_Config.ReadFloat ("ymin"),
			m_Config.ReadFloat ("ymax"),
			m_Config.ReadFloat ("target_move_interval"));
		return script;
	}

	public void AddTargetTail( int _iCount ){

		for (int i = 0; i < _iCount; i++) {
			int iSerial = m_targetList.Count + 1;
			// 一応確認はする
			DataTargetParam targetParam = m_dataTarget.SelectOne (string.Format ("serial = {0}", iSerial));
			if (targetParam.serial == 0) {
				targetParam.serial = iSerial;
				targetParam.status = (int)DataTargetParam.STATUS.ACTIVE;
				targetParam.time_destroy = TimeManager.StrGetTime ();
				m_dataTarget.list.Add (targetParam);
			}
			m_targetList.Add (generateTarget (targetParam));
		}
	}

	public int AppearAll(){
		int iRet = 0;
		string rebuild_time = TimeManager.StrGetTime (m_iTargetInterval * -1);
		foreach (CtrlTarget script in m_targetList) {
			DataTargetParam param = m_dataTarget.SelectOne ( string.Format( "serial = {0}" , script.m_iSerial ));
			if (param.status == (int)DataTargetParam.STATUS.DESTROIED) {
				param.time_destroy = rebuild_time;
				param.status = (int)DataTargetParam.STATUS.ACTIVE;
				script.SetActive ();
				iRet += 1;
			}
		}
		if (0 < iRet) {
			m_dataTarget.Save (DataManager.Instance.FILENAME_TARGET_DATA);
		}
		return iRet;
	}

	public int TotalNum(){
		return m_targetList.Count;
	}

	public int GetActiveNum(){
		int iNum = 0;
		foreach (CtrlTarget target in m_targetList) {
			DataTargetParam param = m_dataTarget.SelectOne ( string.Format( "serial = {0}" , target.m_iSerial ));
			if ((DataTargetParam.STATUS)param.status == DataTargetParam.STATUS.ACTIVE) {
				iNum += 1;
			}
		}
		return iNum;
	}

	void Update(){

		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}
		switch (m_eStep) {
		case STEP.IDLE:
			if (bInit) {
				foreach (CtrlTarget script in m_targetList) {
					script.TriggerClear ();
				}
			}
			foreach (CtrlTarget script in m_targetList) {
				DataTargetParam param = m_dataTarget.SelectOne ( string.Format( "serial = {0}" , script.m_iSerial ));
				int iCheckInterval = m_iTargetInterval;
				if (DataManager.Instance.kvs_data.HasKey (DataManager.Instance.KEY_TARGET_SPEEDUP_END)) {
					if (0 < TimeManager.Instance.GetDiffNow (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_TARGET_SPEEDUP_END)).TotalSeconds) {
						iCheckInterval /= 10;
					}
				}
				if (param.status == (int)DataTargetParam.STATUS.DESTROIED) {
					TimeSpan span = TimeManager.Instance.GetDiff (param.time_destroy , TimeManager.StrGetTime());


					if (iCheckInterval < span.TotalSeconds) {
						param.status = (int)DataTargetParam.STATUS.ACTIVE;
						script.SetActive ();
						m_dataTarget.Save (DataManager.Instance.FILENAME_TARGET_DATA);
					}
					//span.TotalSeconds
				} else if (script.ButtonPushed) {

					SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_TARGETGET);
					script.TriggerClear ();
					param.status = (int)DataTargetParam.STATUS.DESTROIED;
					param.time_destroy = TimeManager.StrGetTime ();
					script.SetDelete ();
					m_dataTarget.Save (DataManager.Instance.FILENAME_TARGET_DATA);
					m_iGetTarget += 1;
				} else {
				}
			}
			break;

		case STEP.GENERATE_WAIT:
			break;

		case STEP.GENERATE:
			break;

		case STEP.MAX:
		default:
			break;
			
		}

	}


}
