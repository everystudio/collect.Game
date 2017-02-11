using UnityEngine;
using System.Collections;

public class BookIcon : MonoBehaviourEx {

	public enum STEP
	{
		IDLE		= 0,
		DISP		,
		MAX			,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;

	private GameObject m_goParent;
	private StillMain m_stillMain;
	private CsvBookParam m_bookParam;

	public ButtonBase m_button;

	public UI2DSprite m_sprImage;
	public UILabel m_lbText;

	public void Initialize(CsvBookParam _param , GameObject _goParent ){
		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;
		m_bookParam = _param;
		string strLabel = _param.text;
		string strImage = _param.local_path + "/" + _param.name;
		if (0 == _param.status) {
			// なんもしない
			m_eStep = STEP.MAX;
			strLabel = "????";
			strImage = "texture/still/secret";
		}
		m_sprImage.sprite2D = SpriteManager.Instance.Load (strImage);
		m_lbText.text = strLabel;

		m_goParent = _goParent;
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
				m_button.TriggerClear ();
			}
			if (m_button.ButtonPushed) {
				m_eStep = STEP.DISP;
			}
			break;
		case STEP.DISP:
			if (bInit) {
				string strImage = m_bookParam.local_path + "/" + m_bookParam.name;

				m_stillMain = PrefabManager.Instance.MakeScript<StillMain> ("prefab/StillMain", m_goParent);
					m_stillMain.transform.localPosition = new Vector3(-1.0f * m_goParent.transform.localPosition.x, 0.0f, 0.0f);
				m_stillMain.Initialize (strImage , true );
			}
			if (m_stillMain.IsEnd ()) {
				Release (m_stillMain.gameObject);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.MAX:
		default:
			break;
		}

	}


}





