using UnityEngine;
using System.Collections;

public class CtrlReview : MonoBehaviourEx {

	public enum STEP
	{
		IDLE		= 0,
		REVIEW		,
		SHINAI		,
		LATER		,
		END			,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public enum REVIEW_TYPE{
		NONE		= 0,
		REVIEW		,
		LATER		,
		MAX			,
	}

	[SerializeField]
	private ButtonBase m_btnReview;
	[SerializeField]
	private ButtonBase m_btnLater;

	private bool m_bIsEnd;
	private string m_strUrl;
	private REVIEW_TYPE m_eReviewType;
	public REVIEW_TYPE GetReviewType(){
		return m_eReviewType;
	}
	public void Initialize( string _strUrl ){
		m_btnReview.TriggerClear ();
		m_btnLater.TriggerClear ();
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
		m_bIsEnd = false;
		m_strUrl = _strUrl;
	}

	public bool IsEnd(){
		return m_bIsEnd;
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

			if (m_btnReview.ButtonPushed) {
				m_eStep = STEP.REVIEW;
			} else if (m_btnLater.ButtonPushed) {
				m_eStep = STEP.LATER;
			} else {
			}

			break;

		case STEP.REVIEW:
			if (bInit) {

				m_eReviewType = REVIEW_TYPE.REVIEW;
				PlayerPrefs.SetInt ("review", 1);

				Application.OpenURL (m_strUrl);
				//Application.OpenURL ("https://play.google.com/store/apps/details?id=jp.everystudio.equal");
			}
			m_eStep = STEP.END;

			break;
		case STEP.SHINAI:
			PlayerPrefs.SetInt ("review", 2);
			m_eStep = STEP.END;
			break;
		case STEP.LATER:
			m_eStep = STEP.END;
			m_eReviewType = REVIEW_TYPE.LATER;
			break;
		case STEP.END:
			if (bInit) {
				TweenAlpha ta = TweenAlphaAll (gameObject, 0.5f, 0.0f);
				EventDelegate.Set (ta.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_bIsEnd = true;
			}
			break;

		case STEP.MAX:
		default:
			break;
		}

	}
}

















