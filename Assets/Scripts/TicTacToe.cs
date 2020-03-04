namespace IBMR
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    // informações de possiveis
    // condições do jogo
    public enum CONDITION
    {
        CHANGE_TURN = 0,
        WIN,
        DRAW
    }

    public class TicTacToe : MonoBehaviour
    {
        // código que controla a tela do fim de jogo
        [SerializeField]
        private GameUI m_ui = null;

        // turno que indica qual jogador deve jogar agora
        private int m_turn = 0;

        // infromacao apenas para o sistema saber o tamanho maximo do board
        private const int BOARD_SIZE = 3;

        // valor do simbolo do jogador 1 que será desenhado no board
        private const string PLAYER_ONE_SYMBOL = "X";
        // valor do simbolo do jogador 2 que será desenhado no board
        private const string PLAYER_TWO_SYMBOL = "0";

        // cor do simbolo do jogador 1 -> vermelho
        private readonly Color PLAYER_ONE_COLOR = Color.red;
        // cor do simbolo do jogador 2 -> azul
        private readonly Color PLAYER_TWO_COLOR = Color.blue;

        // pega a cor do jogador de acordo com o valor do turno
        private Color GetPlayerColor => m_turn == 0 ? PLAYER_ONE_COLOR : PLAYER_TWO_COLOR;

        // pega o simbolo do jogador de acordo com o valor do turno
        private string GetPlayerSymbol => m_turn == 0 ? PLAYER_ONE_SYMBOL : PLAYER_TWO_SYMBOL;

        // matrix de duas dimensões que informa o estado inicial do board
        // sera alterada pelo sistema e usada para verificar as condições de
        // vitoria ou empate
        private int[,] m_board = new int[BOARD_SIZE, BOARD_SIZE] {  { -1, -1, -1 },
                                                                    { -1, -1, -1 },
                                                                    { -1, -1, -1 }};

        // textos dos campos que os jogadores selecionam
        [SerializeField]
        private TextMeshProUGUI[] m_texts = null;

        // metodo que é chamado quando um dos botões é clicado
        // ele recebe o id do botão
        public void OnClickButton (int buttonID)
        {
            // seta na informaçaõ do sistema qual campo foi marcado pro quando jogador
            m_board[GetRoll(buttonID), GetColumn(buttonID)] = m_turn;

            // seta o simbolo do jogador no campo
            m_texts[buttonID].text = GetPlayerSymbol;
            // seta a cor no texto do simbolo do jogador
            m_texts[buttonID].color = GetPlayerColor;

            // recebe a condição atual do jogo que foi verificada
            CONDITION currentCondition = GameCondition(buttonID);

            // de acordo com a condição atual do jogo
            switch (currentCondition)
            {
                // caso seja uma vitoria ou um empate
                case CONDITION.DRAW:
                case CONDITION.WIN:
                    // abre a tela do fim de jogo
                    m_ui.OpenUI(currentCondition, m_turn + 1);
                    break;

                    // caso seja para trocar de turno
                case CONDITION.CHANGE_TURN:
                    // troca qual jogador deve jogar agora
                    m_turn = m_turn == 0 ? 1 : 0;
                    m_ui.SetPlayerTurn(m_turn);
                    break;
            }
        }

        // metodo que de acordo com o id do botão
        // pega em qual linha os jogadores clicaram
        private int GetRoll (int index) => (index / BOARD_SIZE);

        // metodo que de acordo com o id do botão
        // pega em qual coluna os jogadores clicaram
        private int GetColumn (int index) => (index % BOARD_SIZE);

        // metodo que de acordo com o id do botão
        // fala se o jogador clicou em uma das casas
        // que formam uma diagonal
        private bool IsDiagonal (int index) => (index % 2 == 0);

        // metodo que verifica se um dos jogadores ganhou a partida
        // ou se houve um empate
        private CONDITION GameCondition (int index)
        {
            // verifica se a coluna em que o jogador atual
            // selecionou um dos campos forma uma sequencia
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                if (m_board[x, GetColumn(index)] != m_turn)
                {
                    break;
                }

                if (x == 2)
                {
                    return CONDITION.WIN;
                }
            }

            // verifica se a linha em que o jogador atual
            // selecionou um dos campos forma uma sequencia
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                if (m_board[GetRoll(index), y] != m_turn)
                {
                    break;
                }

                if (y == 2)
                {
                    return CONDITION.WIN;
                }
            }

            // caso seja uma das casas na diagonal
            if (IsDiagonal(index))
            {
                // verifica a primeira diagonal
                for (int i = 0; i < BOARD_SIZE; i++)
                {
                    if (m_board[i, i] != m_turn)
                    {
                        break;
                    }

                    if (i == 2)
                    {
                        return CONDITION.WIN;
                    }
                }

                // verifica a segunda diagonal
                for (int i = 0; i < BOARD_SIZE; i++)
                {
                    if (m_board[i, (2-i)] != m_turn)
                    {
                        break;
                    }

                    if (i == 2)
                    {
                        return CONDITION.WIN;
                    }
                }
            }

            bool breakAll = false;

            // caso não tenha ocorrido uma vitória
            // verifica se houve empate
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    // faz uma verificação em cada uma das 9 casas
                    // para ver se ainda tem algum campo que não foi escolhido
                    if (m_board[x, y] == -1)
                    {
                        // caso ainda haja campos para escolher para a verificação
                        breakAll = true;
                        break;
                    }
                }

                if (breakAll)
                {
                    break;
                }

                if (x == 2)
                {
                    return CONDITION.DRAW;
                }
            }
            
            // caso n]ao tenha ocorrido empate
            // então troca o turno dos jogadores
            return CONDITION.CHANGE_TURN;
        }

        // Metodo responsável por fechar o jogo
        public void CloseGame ()
        {
            // caso esteja sendo executando dentro da Unity
#if UNITY_EDITOR
            // só faz a ferramente parar de simular o jogo
            EditorApplication.isPlaying = false;
#else
            // caso esteja sendo executado na build
            // fechar a aplicação
            Application.Quit();
#endif
        }

        // Metodo responsável por recarregar a cena e resetar o jogo
        public void ReloadScene () => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}