using UnityEngine;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Mostra o visual do chefe na cena durante a vez dele (<see cref="CicloDaPartida.EmChefe"/>) e o esconde no
    /// resto da partida. É só o visual: o chefe não anda, não tem colisão nem dano, e qualquer tecla continua
    /// contando como chefe derrotado (pelo <see cref="ControladorDaPartida"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Fica no mesmo objeto do <see cref="ControladorDaPartida"/> (o <c>Partida</c> da cena). O chefe é um
    /// objeto da cena, guardado desligado: ligar o objeto reinicia o <c>Animator</c> no estado padrão
    /// (<c>Parado</c>).</item>
    /// <item>Sem chefe no campo <i>Chefe</i>, o componente registra um erro e se desliga.</item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(ControladorDaPartida))]
    public class ApresentacaoDoChefe : MonoBehaviour
    {
        [Tooltip("Objeto do chefe na cena (ex.: uma instância de VisualChefeTutorial), guardado desligado.")]
        [SerializeField] private GameObject chefe;

        private ControladorDaPartida _controlador;

        /// <summary>Indica se o chefe aparece: só na vez de um chefe (da onda ou da arena).</summary>
        /// <param name="ciclo">Estado da partida (nulo: a partida não funciona, e o chefe não aparece).</param>
        public static bool DeveAparecer(CicloDaPartida ciclo) => ciclo != null && ciclo.EmChefe;

        private void Awake()
        {
            _controlador = GetComponent<ControladorDaPartida>();
            if (chefe == null)
            {
                Debug.LogError($"{name}: a apresentação do chefe não tem o objeto do chefe; ele não aparece.", this);
                enabled = false;
                return;
            }

            chefe.SetActive(false);
        }

        private void LateUpdate()
        {
            var mostrar = DeveAparecer(_controlador.Ciclo);
            if (chefe.activeSelf != mostrar)
            {
                chefe.SetActive(mostrar);
            }
        }
    }
}
