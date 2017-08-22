using UnityEngine;
using UnityEngine.UI;

public class ButtonData : MonoBehaviour
{
	[SerializeField]
	private Text m_text = null;
	public Text campo
	{
		get
		{
			return m_text;
		}
	}

	[SerializeField]
	private int m_x = 0;
	public int x
	{
		get
		{
			return m_x;
		}
	}

	[SerializeField]
	private int m_y = 0;
	public int y
	{
		get
		{
			return m_y;
		}
	}
}
