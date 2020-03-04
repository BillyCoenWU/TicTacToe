namespace IBMR
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class GameUI : MonoBehaviour
    {
        // velocidade da transição que abertura da barra
        [SerializeField]
        private float m_transitionSpeed = 1.0f;

        // variavel para ter acesso a barra
        // que é ativada ao fim do jogo
        [SerializeField]
        private RectTransform m_bar = null;

        // texto que vai indicar se houve empate
        // ou vitoria entre os jogadores
        [SerializeField]
        private TextMeshProUGUI m_text = null;

        // botoes que vão se desativados ao fim do jogo
        [SerializeField]
        private GameObject[] m_gameObjects = null;

        // botoes que seram ativados quando a transição acabar
        [SerializeField]
        private Button[] m_buttons = null;

        [SerializeField]
        private GameObject[] m_playersTurn = null;

        // informação do tamanho da barra para quando ela estiver fechada
        private readonly Vector3 CLOSED_SIZE = new Vector3(0.0f, 1.0f, 1.0f);
        // informação do tamanho total da barra para quando ela terminar de abrir
        private readonly Vector3 FULL_SIZE = Vector3.one;

        // ativa e desativa o indicador de turno dos jogadores
        public void SetPlayerTurn (int turn)
        {
            // passa por todos os indicadores
            // setando true ou false em cada um
            // de acordo com o valor do tuno atual
            for (int i = 0; i < m_playersTurn.Length; i++)
            {
                m_playersTurn[i].SetActive(turn == i);
            }
        }

        // metodo para setar as informações na tela de fim de jogo e chamar a transição da UI
        public void OpenUI (CONDITION finalGameCondition, int player)
        {
            // desativa os indicadores de turnos de ambos os jogadores
            // já que passa um valor que não é referente a turno nenhum
            SetPlayerTurn(-1);

            // coloca no texto da barra de final de jogo se os jogadores empataram ou qual jogador ganhou
            m_text.text = finalGameCondition == CONDITION.DRAW ? "EMPATE!" : $"VITÓRIA DO JOGADOR {player}!";

            // faz cada botao que ainda não desativou ficar desatiado
            foreach (Button b in m_buttons)
            {
                b.interactable = false;
            }

            // inicia a transição que vai abrir a tela de fim de jogo
            StartCoroutine(OpenTransition());
        }

        // Coroutina que vai abrir a tela quando acaba o jogo
        private IEnumerator OpenTransition ()
        {
            // variavel de tempo decorrido, em segundos
            // 1.0f = 1 segundo
            float elapsedTime = 0.0f;

            // enquanto o valor do tempo decorrido for menor que 1.0f
            while (elapsedTime < 1.0f)
            {
                // soma na variavel de tempo decorrido
                // o intervalo entre frames multiplicado
                // pela velocidade da transição
                elapsedTime += Time.deltaTime * m_transitionSpeed;

                // usa do metodo Lerp da estrutura Vector3 para
                // fazer uma transição do tamanho da tela
                // de quando ela está fechada para quando ela está aberta
                m_bar.localScale = Vector3.Lerp(CLOSED_SIZE, FULL_SIZE, elapsedTime);

                //indica que esse recurso deve esperar até o próximo frame para continuar
                yield return null;
            }

            // quando termina a transição:

            // ativa o texto
            m_text.gameObject.SetActive(true);

            // ativa os botões de resetar ou fechar o jogo
            foreach (GameObject go in m_gameObjects)
            {
                // ativa o GameObject atualda lista de botões
                go.SetActive(true);
            }
        }
    }
}
