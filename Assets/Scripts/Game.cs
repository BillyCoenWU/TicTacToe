#region Namespaces
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
#endregion

public struct Jogada
{
	public string player;

	public int x;
	public int y;
}

public class Game : MonoBehaviour
{
	#region Variables & Instance

	public const string X = "X";
	public const string O = "O";
	public const string NONE = "-";

	#if UNITY_EDITOR
	public const string REFRESH = "Assets/Refresh";
	#endif

	private const string DRAW = "Empatou!";
	private const string PLAY_ALONE = "PlayAlone";
	private const string PLAYER_ONE_WIN = "Player 1 venceu!";
	private const string PLAYER_TWO_WIN = "Player 2 venceu!";

	private string m_path = string.Empty;

    [SerializeField]
	private bool m_save = false;
	[SerializeField]
	private bool m_geraJson = false;
	[SerializeField]
	private bool m_startAlone = false;
	[SerializeField]
	private bool m_resetAlone = false;
    private bool m_playerOneJogada = true;

    [SerializeField]
	private GameObject m_resetButton = null;

	[SerializeField]
	private ButtonData[] m_buttons = null;

    private string[,] m_map = null;

    private List<int> m_indexs = new List<int>();

    private Stack<string> m_playerOne = new Stack<string>();
    private Stack<string> m_playerTwo = new Stack<string>();

    private LinkedList<Jogada> m_linkedJogadas = new LinkedList<Jogada>();

    private List<LinkedList<Jogada>> m_partidas = new List<LinkedList<Jogada>>();

	private static Game s_instance = null;
	public static Game Instance
	{
		get
		{
			return s_instance;
		}
	}

	#endregion

	#region Basic_Methods

	private void Awake()
	{
		s_instance = this;

		m_path = string.Concat(Application.dataPath, "/TicTacToe.json");

		NovaPartida();
	}

	#endregion

	#region Other_Methods

	public void NovaPartida ()
	{
        m_playerOneJogada = true;

        for (int i = 0; i < 5; i++)
        {
            m_playerOne.Push(X);
            m_playerTwo.Push(O);
        }

        m_map = new string[3, 3]    {   { NONE, NONE, NONE },
                                        { NONE, NONE, NONE },
                                        { NONE, NONE, NONE } };

        m_resetButton.SetActive(false);

		for(int i = 0; i < m_buttons.Length; i++)
		{
			m_buttons[i].campo.text = NONE;
		}

		if(m_startAlone)
		{
			m_indexs.Clear();

			for(int i = 0; i < 9; i++)
			{
				m_indexs.Add(i);
			}

			InvokeRepeating(PLAY_ALONE, 0.5f, 0.5f);
		}
	}

	public void NovaJogada (int index)
	{
		if(CheckFreeHouse (m_buttons[index].x, m_buttons[index].y))
		{
            m_map[m_buttons[index].x, m_buttons[index].y] = m_playerOneJogada ? m_playerOne.Pop() : m_playerTwo.Pop();
            m_buttons[index].campo.text = m_map[m_buttons[index].x, m_buttons[index].y];

            m_playerOneJogada = !m_playerOneJogada;

            Jogada j = new Jogada();
            j.player = m_map[m_buttons[index].x, m_buttons[index].y];
            j.x = m_buttons[index].x;
            j.y = m_buttons[index].y;
            
            if (m_linkedJogadas.Count == 0)
            {
                m_linkedJogadas.AddFirst(j);
            }
            else m_linkedJogadas.AddLast(j);
		}

		CheckFreeHouses();

		if(m_playerOne.Count <= 2)
		{
			CheckVictory();
		}
	}

	private void PlayAlone ()
	{
		int index = Random.Range(0, m_indexs.Count);
		int valor = m_indexs[index];
		m_indexs.RemoveAt(index);
		NovaJogada(valor);
	}

	private int CheckFreeHouses ()
	{
		int count = 0;

		for(int x = 0; x < 3; x++)
		{
			for(int y = 0; y < 3; y++)
			{
				if(CheckFreeHouse(x, y))
				{
					count++;
				}
			}
		}

        return count;
	}

	private bool CheckFreeHouse (int x, int y)
	{
		return (m_map[x,y] == NONE);
	}

	private void CheckVictory ()
	{
		for(int x = 0; x < 3; x++)
		{
			if(m_map[x, 0] != NONE)
			{
				if(WonX (m_map[x, 0], x))
				{
					EndGame (m_map[x, 0]);
					return;
				}
			}
		}

		for(int y = 0; y < 3; y++)
		{
			if(m_map[0, y] != NONE)
			{
				if(WonY (m_map[0, y], y))
				{
					EndGame (m_map[0, y]);
					return;
				}
			}
		}

		if(m_map[0, 0] != NONE)
		{
			if(WonDiagonalOne(m_map[0, 0]))
			{
				EndGame (m_map[0, 0]);
				return;
			}
		}

		if(m_map[0, 2] != NONE)
		{
			if(WonDiagonalTwo(m_map[0, 2]))
			{
				EndGame (m_map[0, 2]);
				return;
			}
		}

		if(CheckFreeHouses() == 0)
		{
			Empate();
		}
	}

	private bool WonX (string player, int x)
	{
		for(int y = 1; y < 3; y++)
		{
			if(m_map[x, y] != player)
			{
				return false;
			}
		}

		return true;
	}

	private bool WonY (string player, int y)
	{
		for(int x = 1; x < 3; x++)
		{
			if(m_map[x, y] != player)
			{
				return false;
			}
		}

		return true;
	}

	private bool WonDiagonalOne (string player)
	{
		return !(m_map[1, 1] != player || m_map[2, 2] != player);
	}

	private bool WonDiagonalTwo (string player)
	{
		return !(m_map[1, 1] != player || m_map[2, 0] != player);
	}

	private void EndGame (string player)
	{
		if(m_startAlone)
		{
			CancelInvoke(PLAY_ALONE);
		}

		switch (player)
		{
			case O:
				Debug.Log(PLAYER_TWO_WIN);
				break;

			case X:
				Debug.Log(PLAYER_ONE_WIN);
				break;
		}

		if(m_save)
		{
			m_savePartida();
		}

		if(m_resetAlone)
		{
			NovaPartida();
		}
		else m_resetButton.SetActive(true);
	}

	private void Empate ()
	{
		if(m_startAlone)
		{
			CancelInvoke(PLAY_ALONE);
		}

		Debug.Log(DRAW);

		if(m_resetAlone)
		{
			NovaPartida();
		}
		else m_resetButton.SetActive(true);
	}

	private void m_savePartida ()
	{
		m_partidas.Add(m_linkedJogadas);

		if(m_geraJson)
		{
			File.WriteAllText(m_path, JsonConvert.SerializeObject(m_partidas));

			#if UNITY_EDITOR
			EditorApplication.ExecuteMenuItem(REFRESH);
			#endif
		}
	}

	public void LoadPartidas ()
	{
		m_partidas.Clear();

		m_partidas = JsonConvert.DeserializeObject<List<LinkedList<Jogada>>>(File.ReadAllText(m_path));
	}

	#endregion
}
