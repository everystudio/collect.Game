using UnityEngine;
using System.Collections;

public class SelectButton : ButtonBase {

	public UILabel m_lbText;

	public void Initialize( CsvScriptParam _param ){

		m_lbText.text = _param.param;

		// データはint型
		int iParam = int.Parse(_param.option1);

		Index = iParam;

		TriggerClear ();

		return;
	}


}









