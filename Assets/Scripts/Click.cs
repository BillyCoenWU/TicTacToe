using UnityEngine;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
	private int m_int = 1;

	public Text text = null; 

	public void ShowValue ()
	{
		text.text = string.Concat("O número ", m_int.ToString(), (m_int % 2 == 0 ? " é par!" : " é ímpar!"));
		m_int++;
	}
}
