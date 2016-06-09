using UnityEngine;
using System.Collections;

public class BookMain : PageBase {

	public UIGrid m_goGrid;
	public CsvBook m_Book;

	public ButtonBase m_closeButton;

	public override void PageStart ()
	{
		base.PageStart ();
		m_bIsEnd = false;

		gameObject.SetActive (true);

		if (m_Book == null) {
			m_Book = new CsvBook ();
			m_Book.Load (CsvBook.FILE_NAME);
		}

		GameObject goIconGrid = null;
		int count = 0;
		foreach (CsvBookParam param in m_Book.list) {
			if (count % 4 == 0) {
				goIconGrid = PrefabManager.Instance.MakeObject ("prefab/GridBookIcon", m_goGrid.gameObject);
			}
			BookIcon bookIcon = PrefabManager.Instance.MakeObject ("prefab/BookIcon", goIconGrid).GetComponent<BookIcon> ();
			bookIcon.Initialize (param,m_goGrid.gameObject.transform.parent.gameObject);
			count += 1;
		}
		m_goGrid.enabled = true;
		m_closeButton.TriggerClear ();
	}

	public override void PageEnd ()
	{
		base.PageEnd ();
		gameObject.SetActive (false);
		m_bIsEnd = true;
		return;
	}

	// Update is called once per frame
	void Update () {
		if (m_closeButton.ButtonPushed) {
			m_closeButton.TriggerClear ();
			PageEnd ();
		}
	}
}
