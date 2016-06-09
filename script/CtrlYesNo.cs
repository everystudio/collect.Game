using UnityEngine;
using System.Collections;

public class CtrlYesNo : MonoBehaviour {

	[SerializeField]
	private UILabel m_lbMessage;

	[SerializeField]
	private ButtonBase m_btnYes;
	[SerializeField]
	private ButtonBase m_btnNo;

	public void Init( string _strMessage ){
		m_lbMessage.text = _strMessage;
		m_btnYes.TriggerClear ();
		m_btnNo.TriggerClear ();
	}

	public bool IsYes(){
		return m_btnYes.ButtonPushed;
	}
	public bool IsNo(){
		return m_btnNo.ButtonPushed;
	}
}
