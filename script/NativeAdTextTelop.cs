using UnityEngine;
using System.Collections;
using NendUnityPlugin.AD.Native;
using UnityEngine.UI;

public class NativeAdTextTelop : MonoBehaviourEx {

	public enum STEP
	{
		NONE		= 0,
		SETUP		,
		MOVE		,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	public int FONT_SIZE = 17;
	//private float INTERVAL = 1.0f;
	public float BACK_WIDTH = 92.0f;
	public float BACK_HEIGHT= 10.0f;
	//private float m_fTimer;

	private string m_strPr;
	private string m_strShortText;
	private RectTransform rectTransform;

	[SerializeField]
	private Text m_textTelop;
	private float m_fTelopWidth;
	// Use this for initialization

	private int AdIndex;

	private NendAdNativeView nativeAdView;

	void Start(){
		/*
		m_strPr = "【PR】";
		m_strShortText = "広告準備中☆広告準備中";

		m_textTelop.text = string.Format ("{0}{1}", m_strPr, m_strShortText);
		m_fTelopWidth = m_textTelop.text.Length * FONT_SIZE;
		m_textTelop.rectTransform.sizeDelta = new Vector2 ( m_fTelopWidth,20.0f);
		*/
	}

	public void Loaded(NendAdNativeView view){
		myTransform.localPosition = new Vector3 (BACK_WIDTH - (0.5f*BACK_WIDTH) , BACK_HEIGHT*-0.25f, 0.0f);
		//Debug.LogError ("NativeAdTextTelop.Loaded");
		nativeAdView = view;
		//m_strPr = nativeAdView.GetPrText ();
		m_strShortText = nativeAdView.GetShortText ();
		m_strPr = "【PR】";
		//m_strShortText = "広告準備中☆広告準備中";
		m_textTelop.text = string.Format ("{0}{1}", m_strPr, m_strShortText);
		m_fTelopWidth = m_textTelop.text.Length * FONT_SIZE;
		m_textTelop.rectTransform.sizeDelta = new Vector2 (m_fTelopWidth,FONT_SIZE+2.0f);

		m_eStep = STEP.SETUP;
	}

	// Update is called once per frame
	void Update () {
		/*
		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
		}
		*/

		switch (m_eStep) {
		case STEP.SETUP:
			//m_eStep = STEP.MOVE;
			//myTransform.localPosition = new Vector3 (BACK_WIDTH, 0.0f, 0.0f);
			//m_textTelop.rectTransform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
			myTransform.localPosition = new Vector3 (BACK_WIDTH - (0.5f*BACK_WIDTH) , BACK_HEIGHT*-0.25f, 0.0f);
			//Debug.LogError (m_textTelop.rectTransform.position.x);
			m_eStep = STEP.MOVE;
			break;
		case STEP.MOVE:

			myTransform.localPosition = new Vector3 (myTransform.localPosition.x - (14.0f * Time.deltaTime), BACK_HEIGHT*-0.25f, 0.0f);

			if (myTransform.localPosition.x < -1.0f*m_fTelopWidth - (0.5f*BACK_WIDTH) ) {
				m_eStep = STEP.SETUP;
			}
/*
			m_fTimer += Time.deltaTime;
			if (INTERVAL < m_fTimer) {
				m_fTimer = 0.0f;
				Debug.LogError (nativeText.text.Substring (1, nativeText.text.Length - 1));
				m_strShortText = nativeText.text.Substring (1, nativeText.text.Length - 1);
			}
			nativeText.text = m_strShortText;
*/
			break;
		case STEP.MAX:
		default:
			break;
		}
	}
}



