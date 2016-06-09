using UnityEngine;
using System.Collections;

public class ChapterBanner : ButtonBase {

	public bool m_bAble;
	public int m_iScriptId;
	public UILabel m_lbTitle;
	public UILabel m_lbName;

	public void Initialize( int _iChapterId , int _iScriptId , string _strTitle , string _strName ){
		m_bAble = DataManager.Instance.kvs_data.HasKey (CsvChapter.GetChapterKey (_iChapterId));
		m_iScriptId = _iScriptId;
		if (m_bAble == false) {
			_strTitle = "？？？";
			_strName = "？？？";
		}
		m_lbTitle.text = _strTitle;
		m_lbName.text = _strName;
		return;
	}

	public void Initialize( CsvChapterParam _param ){
		Initialize ( _param.chapter_id , _param.script_id, _param.title, _param.name);
		return;
	}

	void Update () {
	
	}
}
