using UnityEngine;
using System.Collections;

public class ShareButton : ButtonBase {

	public UILabel m_lbText;

	public void Initialize( string _strText )
	{
		m_lbText.text = _strText;
		// ButtonManager側で初期化する
		//TriggerClear();
		return;
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
