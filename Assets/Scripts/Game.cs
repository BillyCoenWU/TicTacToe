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

[System.Serializable]
public struct Jogada
{
	public string player;

	public int x;
	public int y;
}

[System.Serializable]
public struct TicTacToe
{
	private bool m_playerOneJogada;

	public Queue<Jogada> jogadas;
	public LinkedList<Jogada> linkedJogadas;

	private Stack<string> m_playerOne;
	private Stack<string> m_playerTwo;

	public string[,] map;

	public void Start ()
	{
		m_playerOneJogada = true;

		jogadas = new Queue<Jogada>();
		m_playerOne = new Stack<string>();
		m_playerTwo = new Stack<string>();
		linkedJogadas = new LinkedList<Jogada>();

		for(int  i = 0; i < 5; i ++)
		{
			m_playerOne.Push(Game.X);
			m_playerTwo.Push(Game.O);
		}

		map = new string[3,3]	{	{ Game.NONE, Game.NONE, Game.NONE },
									{ Game.NONE, Game.NONE, Game.NONE },
									{ Game.NONE, Game.NONE, Game.NONE }	};
	}

	public int StackOneCount ()
	{
		return m_playerOne.Count;
	}

	public int StackTwoCount ()
	{
		return m_playerTwo.Count;
	}

	public string Jogada (int x, int y)
	{
		map[x, y] = m_playerOneJogada ? m_playerOne.Pop() : m_playerTwo.Pop();

		m_playerOneJogada = !m_playerOneJogada;

		Jogada j = new Jogada();
		j.player = map[x, y];
		j.x = x;
		j.y = y;

		jogadas.Enqueue(j);

		if(linkedJogadas.Count == 0)
		{
			linkedJogadas.AddFirst(j);
		}
		else linkedJogadas.AddLast(j);

		return map[x, y];
	}
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
	private const string FREE_HOUSES = "Casas livres: ";
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

	[SerializeField]
	private GameObject m_resetButton = null;

	[SerializeField]
	private ButtonData[] m_buttons = null;

	private List<int> m_indexs = new List<int>();
	private TicTacToe m_tictactoe = new TicTacToe();
	private List<Queue<Jogada>> m_partidas = new List<Queue<Jogada>>();

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
		m_tictactoe = new TicTacToe();
		m_tictactoe.Start();

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
			m_buttons[index].campo.text = m_tictactoe.Jogada(m_buttons[index].x, m_buttons[index].y);
		}

		CheckFreeHouses();

		if(m_tictactoe.StackOneCount() <= 2)
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

	private void CheckFreeHouses ()
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

		#if UNITY_EDITOR
		Debug.Log(FREE_HOUSES + count);
		#endif
	}

	private bool CheckFreeHouse (int x, int y)
	{
		return (m_tictactoe.map[x,y] == NONE);
	}

	private void CheckVictory ()
	{
		for(int x = 0; x < 3; x++)
		{
			if(m_tictactoe.map[x, 0] != NONE)
			{
				if(WonX (m_tictactoe.map[x, 0], x))
				{
					EndGame (m_tictactoe.map[x, 0]);
					return;
				}
			}
		}

		for(int y = 0; y < 3; y++)
		{
			if(m_tictactoe.map[0, y] != NONE)
			{
				if(WonY (m_tictactoe.map[0, y], y))
				{
					EndGame (m_tictactoe.map[0, y]);
					return;
				}
			}
		}

		if(m_tictactoe.map[0, 0] != NONE)
		{
			if(WonDiagonalOne(m_tictactoe.map[0, 0]))
			{
				EndGame (m_tictactoe.map[0, 0]);
				return;
			}
		}

		if(m_tictactoe.map[0, 2] != NONE)
		{
			if(WonDiagonalTwo(m_tictactoe.map[0, 2]))
			{
				EndGame (m_tictactoe.map[0, 2]);
				return;
			}
		}

		if(m_tictactoe.StackOneCount() == 0 && m_tictactoe.StackTwoCount() == 1)
		{
			Empate();
		}
	}

	private bool WonX (string player, int x)
	{
		for(int y = 1; y < 3; y++)
		{
			if(m_tictactoe.map[x, y] != player)
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
			if(m_tictactoe.map[x, y] != player)
			{
				return false;
			}
		}

		return true;
	}

	private bool WonDiagonalOne (string player)
	{
		return !(m_tictactoe.map[1, 1] != player || m_tictactoe.map[2, 2] != player);
	}

	private bool WonDiagonalTwo (string player)
	{
		return !(m_tictactoe.map[1, 1] != player || m_tictactoe.map[2, 0] != player);
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
		m_partidas.Add(m_tictactoe.jogadas);

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

		m_partidas = JsonConvert.DeserializeObject<List<Queue<Jogada>>>(File.ReadAllText(m_path));
	}

	#endregion
}
