using UnityEngine;
using System.Collections;

public class CtrlMessage : MonoBehaviour {

	public UILabel m_lbTitle;
	public UILabel m_lbMessage;
	public void Initialize( string _strTitle , string _strMessage ){
		m_lbTitle.text = _strTitle;
		m_lbMessage.text = _strMessage;
	}

}
