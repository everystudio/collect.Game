using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UI2DSprite))]
public class CtrlTarget : ButtonBase {

	public enum STEP {
		NONE		= 0,
		LOADING		,
		IDLE		,
		WALK		,
		MOVE		,
		HAPPY		,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public int m_iSerial;

	float m_fTimer;
	float m_fTimerAnimation;
	int m_iAnimationFrame;
	public Vector3 m_vecTarget;
	public Vector3 m_vecPosition;
	public Vector3 m_vecPositionStart;
	const float WAIT_INTERVAL = 3.0f;

	UI2DSprite sprite;

	public float m_fRangeMinX;
	public float m_fRangeMaxX;
	public float m_fRangeMinY;
	public float m_fRangeMaxY;
	public float m_fMoveInterval;

	//public DataTargetParam m_targetParam;

	private Vector3 getRandomPos( float _fRangeMinX , float _fRangeMaxX , float _fRangeMinY , float _fRangeMaxY ){
		Vector3 ret = new Vector3 ();

		float fX = UtilRand.GetRange (_fRangeMaxX,_fRangeMinX);
		float fY = UtilRand.GetRange (_fRangeMaxY, _fRangeMinY);

		ret = new Vector3 (fX, fY, 0.0f);
		return ret;
	}

	public void initialize ( int _iSerial , float _fRangeMinX , float _fRangeMaxX , float _fRangeMinY , float _fRangeMaxY , float _fMoveInterval ) {
		m_iSerial = _iSerial;

		DataTargetParam param = ManagerTarget.Instance.m_dataTarget.SelectOne ( string.Format( "serial = {0}" , m_iSerial));

		m_eStep = STEP.NONE;
		if (param.status == (int)DataTargetParam.STATUS.ACTIVE) {
			m_eStep = STEP.IDLE;
		}
		m_eStepPre = STEP.MAX;

		m_fRangeMinX = _fRangeMinX;
		m_fRangeMaxX = _fRangeMaxX;
		m_fRangeMinY = _fRangeMinY;
		m_fRangeMaxY = _fRangeMaxY;
		m_fMoveInterval = _fMoveInterval;

		myTransform.localPosition = getRandomPos (m_fRangeMinX, m_fRangeMaxX, m_fRangeMinY, m_fRangeMaxY);


		sprite = GetComponent<UI2DSprite> ();
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
			if (bInit) {
				sprite.enabled = false;
			}
			break;

		case STEP.IDLE:
			if (bInit) {
				sprite.enabled = true;
				m_fTimer = 0.0f;
			}
			m_fTimer += Time.deltaTime;
			if (WAIT_INTERVAL < m_fTimer) {
				m_eStep = STEP.MOVE;
				m_vecTarget = getRandomPos (m_fRangeMinX, m_fRangeMaxX, m_fRangeMinY, m_fRangeMaxY);
			}
			break;

		case STEP.MOVE:
			if (bInit) {
				m_vecPositionStart = myTransform.localPosition;
				m_vecPosition = m_vecPositionStart;
				m_fTimer = 0.0f;
				m_fTimerAnimation = m_fMoveInterval;
				m_iAnimationFrame = -1;		// 最初のみ

				if (m_vecPosition.x < m_vecTarget.x) {
					myTransform.localScale = new Vector3 (-1.0f, 1.0f, 1.0f);
				} else {
					myTransform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f);
				}
			}

			m_fTimerAnimation += Time.deltaTime;
			if ( m_fMoveInterval < m_fTimerAnimation ) {

				m_iAnimationFrame += 1;
				m_fTimerAnimation -= m_fMoveInterval;
			}

			if (true == Linear (ref m_fTimer, Time.deltaTime, m_fMoveInterval, m_vecPositionStart, m_vecTarget, out m_vecPosition)) {
				m_eStep = STEP.IDLE;
			}
			myTransform.localPosition = m_vecPosition;
			break;
		}
	
	}
	public void SetDelete(){
		m_eStep = STEP.NONE;
	}
	public void SetActive(){
		m_eStep = STEP.IDLE;
	}
}
