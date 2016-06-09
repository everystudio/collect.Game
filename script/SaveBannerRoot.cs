using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveBannerRoot : ButtonManager {

	public UIGrid m_grid;

	private List<SaveBanner> m_bannerList = new List<SaveBanner>();
	public void Initialize( SaveBanner.TYPE _eType , int _iNum ){
		bool bCreate = false;
		if (m_bannerList.Count == 0) {
			ButtonRefresh ();
			bCreate = true;
		}
		for (int i = 0; i < _iNum; i++) {
			SaveBanner script = null;
			if (bCreate == true ) {
				script = PrefabManager.Instance.MakeScript<SaveBanner> ("prefab/SaveBanner", m_grid.gameObject);
				AddButtonBaseList (script.gameObject);
				m_bannerList.Add (script);
			} else {
				script = m_bannerList [i];
			}
			script.Initialize (_eType, i + 1 , "" , "");
		}
		m_grid.enabled = true;
	}

}
