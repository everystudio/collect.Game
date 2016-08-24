using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UI2DSprite))]
[RequireComponent(typeof(ButtonBase))]
public class CtrlCharacter : MonoBehaviourEx {

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

	float m_fTimer;
	float m_fTimerAnimation;
	int m_iAnimationFrame;
	Vector3 m_vecTarget;
	Vector3 m_vecPosition;
	Vector3 m_vecPositionStart;

	const float WAIT_INTERVAL = 3.0f;

	private UI2DSprite m_spriteChara;
	private ButtonBase m_button;

	public List<Sprite> m_sprIdleList = new List<Sprite> ();
	public List<Sprite> m_sprWalkList = new List<Sprite> ();
	public List<Sprite> m_sprHappyList = new List<Sprite> ();

	CsvKvs m_Config = new CsvKvs();

	private Vector3 getRandomPos(){
		Vector3 ret = new Vector3 ();

		float fX = UtilRand.GetRange (m_Config.ReadFloat("xmax"),m_Config.ReadFloat("xmin")) ;
		float fY = UtilRand.GetRange (m_Config.ReadFloat("ymax"),m_Config.ReadFloat("ymin"));

		ret = new Vector3 (fX, fY, 0.0f);

		return ret;
	}

	private void SetCharacter( string _strName ){
		// 要改良
		m_sprIdleList.Clear ();
		m_sprWalkList.Clear ();
		m_sprHappyList.Clear ();

		m_sprIdleList.Add( SpriteManager.Instance.Load( string.Format( _strName , 1)));
		m_sprIdleList.Add( SpriteManager.Instance.Load( string.Format( _strName , 2)));

		m_sprWalkList.Add( SpriteManager.Instance.Load( string.Format( _strName , 3)));
		m_sprWalkList.Add( SpriteManager.Instance.Load( string.Format( _strName , 4)));

		m_sprHappyList.Add( SpriteManager.Instance.Load( string.Format( _strName , 5)));
		m_sprHappyList.Add( SpriteManager.Instance.Load( string.Format( _strName , 6)));
		return;
	}

	int m_iNetworkSerial;

	public void Initialize( string _strName ) {
		SetCharacter( _strName );

		if (m_spriteChara == null) {
			m_spriteChara = GetComponent<UI2DSprite> ();
		}
		m_spriteChara.sprite2D = m_sprIdleList [0];

		if (m_button == null) {
			m_button = GetComponent<ButtonBase> ();
		}

		m_eStep = STEP.LOADING;
		m_eStepPre = STEP.MAX;
		m_Config.LoadResources ("csv/config_chara");
		if (_strName.Equals ("") || _strName.Equals(CsvKvs.READ_ERROR_STRING)) {
			Debug.Log ("empty");
			gameObject.SetActive (false);
		} else {
			Debug.Log ("fill");
			gameObject.SetActive (true);
		}
		return;
	}

	public int idepth;
	public int count;
	public bool m_bOpen;
	public string m_strLogs;

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
		case STEP.LOADING:
			m_eStep = STEP.IDLE;
			break;
		case STEP.IDLE:
			if (bInit) {
				m_fTimer = 0.0f;
				m_spriteChara.sprite2D = m_sprIdleList [0];
				m_button.TriggerClear();
			}
			m_fTimer += Time.deltaTime;
			if (WAIT_INTERVAL < m_fTimer) {
				m_eStep = STEP.MOVE;
				m_vecTarget = getRandomPos ();
			}
			else if( m_button.ButtonPushed ){
				m_eStep = STEP.HAPPY;
			}
			else {
			}
			break;

		case STEP.MOVE:
			if (bInit) {
				m_vecPositionStart = myTransform.localPosition;
				m_vecPosition = m_vecPositionStart;
				m_fTimer = 0.0f;
				m_fTimerAnimation = m_Config.ReadFloat ("move_interval");
				m_iAnimationFrame = -1;     // 最初のみ


				if (m_vecPosition.x < m_vecTarget.x) {
						m_spriteChara.flip = UIBasicSprite.Flip.Horizontally;
						//myTransform.localScale = new Vector3 (-1.0f, 1.0f, 1.0f);
				} else {
						m_spriteChara.flip = UIBasicSprite.Flip.Nothing;

						//myTransform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f);
				}
			}

			m_fTimerAnimation += Time.deltaTime;
			if ( m_Config.ReadFloat ("move_interval") < m_fTimerAnimation ) {

				m_iAnimationFrame += 1;
				m_iAnimationFrame %= m_sprWalkList.Count;
				m_fTimerAnimation -= m_Config.ReadFloat ("move_interval");
				m_spriteChara.sprite2D = m_sprWalkList [m_iAnimationFrame];
				
			}

			if (true == Linear (ref m_fTimer, Time.deltaTime, m_Config.ReadFloat ("move_time") , m_vecPositionStart, m_vecTarget, out m_vecPosition)) {
				m_eStep = STEP.IDLE;
			}
			else if( m_button.ButtonPushed ){
				m_eStep = STEP.HAPPY;
			}
			else {
			}

			myTransform.localPosition = m_vecPosition;
			break;

		case STEP.HAPPY:
			if (bInit) {
				m_fTimer = 0.0f;
				m_fTimerAnimation = 0.0f;
				m_iAnimationFrame = 0;		// 最初のみ
				m_spriteChara.sprite2D = m_sprHappyList [m_iAnimationFrame];
			}
			m_fTimer += Time.deltaTime;
			m_fTimerAnimation += Time.deltaTime;
			float fHappyAnimationInterval = m_Config.ReadFloat ("move_interval") * 2.0f;
			if ( fHappyAnimationInterval < m_fTimerAnimation) {

				m_iAnimationFrame += 1;
				m_iAnimationFrame %= m_sprWalkList.Count;
				m_fTimerAnimation -= fHappyAnimationInterval;
				m_spriteChara.sprite2D = m_sprHappyList [m_iAnimationFrame];
			}
			if (3.0f < m_fTimer) {
				m_eStep = STEP.IDLE;
			}
			break;

		case STEP.MAX:
		default:
			break;
		}
	}
}



