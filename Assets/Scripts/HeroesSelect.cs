using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public struct Heroe
{
	public string name;
}

public class HeroesSelect : MonoBehaviour
{
	[SerializeField]
	private Text m_outputText = null;

	[SerializeField]
	private Text m_generoText = null;

	[SerializeField]
	private Heroe[] m_heroes = null;

	private void Start ()
	{
		float forca = 1000.0f;

		for(int i = 0; i < 12; i++)
		{
			forca *= 1.01f;
		}

		Debug.Log(forca);
	}

	public void OnClick (int index)
	{
		m_outputText.text = string.Concat("O herói ", m_heroes[index].name, " foi selecionado");
	}

	public void Genero (int index)
	{
		m_generoText.text = index == 1 ? "Masculino" : "Feminimo";
	}

	public void OnClear ()
	{
		m_outputText.text = string.Empty;
		m_generoText.text = string.Empty;

		int i = 0;

		for(; i < 5 ;)
		{
			i++;
		}

		Debug.Log(i);
	}
}
