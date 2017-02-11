using UnityEngine;
using System.Collections;

public class StillMain : WindowBase {

	public enum STEP {
		NONE		= 0,
		APPEAR		,
		WAIT		,
		IDLE		,
		CLOSE		,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public bool m_bIsTap;
	public float m_fTimer;
	private bool m_bQuick;

	[SerializeField]
	ButtonBase m_btnSprite;
	[SerializeField]
	UI2DSprite m_sprite;

	/*
	void Start(){
		Initialize ("screenshot.png");
	}
	*/

	public void Initialize( string _strStillName , bool _bQuick = false){
		Debug.Log (string.Format ("still:[{0}]", _strStillName));

		CsvBook book = new CsvBook ();
		book.Load (CsvBook.FILE_NAME);
		foreach (CsvBookParam param in book.list) {
			if (_strStillName.Contains(param.name)) {
				param.status = 1;
				break;
			}
		}
		book.Save (CsvBook.FILE_NAME);
		m_eStepPre	= STEP.MAX;
		m_bQuick = _bQuick;
		m_sprite.sprite2D = SpriteManager.Instance.Load (_strStillName);
		m_sprite.width = (int)m_sprite.sprite2D.rect.width;
		m_sprite.height = (int)m_sprite.sprite2D.rect.height;
		if (m_bQuick) {
			m_eStep = STEP.IDLE;
		} else {
			m_eStep = STEP.APPEAR;
			TweenAlphaAll (m_sprite.gameObject, 0.0f, 0.0f);
		}
		WindowStart ();
		return;
	}

	protected override void windowStart ()
	{
		// closeボタンいらない
		//TweenAlphaAll( m_btnClose.gameObject , 0.0f , 0.0f );
		m_btnClose.gameObject.SetActive (false);
		m_ctrlMessage.Initialize ("", "");
		m_ctrlMessage.gameObject.SetActive (false);

		return;
	}

	protected override void windowEnd ()
	{
		return;
	}

	void Update () {
		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}
		switch (m_eStep) {
		case STEP.APPEAR:
			if (bInit) {
				TweenAlpha ta = TweenAlphaAll (m_sprite.gameObject, 1.0f, 1.0f);
				EventDelegate.Set (ta.onFinished, EndTween);
				m_btnSprite.TriggerClear ();
			}
			if (m_bEndTween) {
				m_eStep = STEP.WAIT;
			} else if (m_btnSprite.ButtonPushed) {
				m_eStep = STEP.CLOSE;
			} else {
			}
			break;

		case STEP.WAIT:
			if (bInit) {
				m_fTimer = 0.0f;
			}
			m_fTimer += Time.deltaTime;
			if (5.0f < m_fTimer) {
				m_eStep = STEP.IDLE;
			} else if (m_btnSprite.ButtonPushed) {
				m_eStep = STEP.CLOSE;
			} else {
			}
			break;
		case STEP.IDLE:
			if (bInit) {
				//TweenAlphaAll (m_btnClose.gameObject, 1.0f, 1.0f);
				m_btnClose.gameObject.SetActive(true);
				m_btnClose.TriggerClear ();
			}
			if (m_btnClose.ButtonPushed) {
				m_eStep = STEP.CLOSE;
			} else if (m_btnSprite.ButtonPushed) {
				m_eStep = STEP.CLOSE;
			} else {
			}
			break;
		case STEP.CLOSE:
			if (bInit) {
				TweenAlpha ta = TweenAlphaAll (gameObject, 0.5f, 0.0f);
				EventDelegate.Set (ta.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_eStep = STEP.END;
			}
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

	public void OnClickButton(){
		m_bIsTap = true;
	}


}
