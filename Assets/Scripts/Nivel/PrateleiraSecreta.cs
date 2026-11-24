using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Prateleira com o segredo: uma investida contra ela derruba um produto sorteado, uma vez só
    /// (<see cref="EstadoDaPrateleira"/>). A batida chega pelo <see cref="ImpactoDaInvestida"/> do jogador.
    /// </summary>
    /// <remarks>
    /// Os produtos, o afastamento e a altura vêm do <see cref="DefinicaoNivel"/> do <see cref="ControladorDoNivel"/> no pai.
    /// O produto cai na frente da face batida, para um lado sorteado.
    /// </remarks>
    [RequireComponent(typeof(Collider))]
    public class PrateleiraSecreta : MonoBehaviour
    {
        private readonly EstadoDaPrateleira _estado = new EstadoDaPrateleira();
        private ControladorDoNivel _nivel;
        private Collider _colisor;

        /// <summary>Indica se a prateleira já derrubou o produto dela.</summary>
        public bool Derrubou => _estado.Derrubou;

        private void Awake()
        {
            _nivel = GetComponentInParent<ControladorDoNivel>();
            _colisor = GetComponent<Collider>();
        }

        /// <summary>Registra uma batida do personagem. Público para o <see cref="ImpactoDaInvestida"/> e testes pela CLI.</summary>
        /// <param name="personagem">Posição do personagem no mundo.</param>
        /// <param name="emInvestida">Se a batida foi durante uma investida.</param>
        /// <returns>O produto que caiu, ou nulo.</returns>
        public GameObject ReceberBatida(Vector3 personagem, bool emInvestida)
        {
            if (_nivel == null || _nivel.Definicao == null || !_estado.TentarDerrubar(emInvestida))
            {
                return null;
            }

            var definicao = _nivel.Definicao;
            var indice = EstadoDaPrateleira.SortearProduto(definicao.Produtos.Count, _nivel.Sorteio);
            if (indice < 0 || definicao.Produtos[indice] == null)
            {
                Debug.LogWarning($"{name}: o nível não tem produtos para derrubar.", this);
                return null;
            }

            var limites = _colisor.bounds;
            var baseDaPrateleira = new Retangulo(new Vector2(limites.center.x, limites.center.z), new Vector2(limites.size.x, limites.size.z));
            var lado = _nivel.Sorteio.Next(2) == 0 ? -1f : 1f;
            var noChao = EstadoDaPrateleira.PosicaoDoProduto(
                baseDaPrateleira, new Vector2(personagem.x, personagem.z),
                definicao.AfastamentoDoProduto, lado * definicao.DeslocamentoLateralDoProduto);
            var posicao = new Vector3(noChao.x, definicao.AlturaDoProduto, noChao.y);
            return Instantiate(definicao.Produtos[indice], posicao, Quaternion.identity, _nivel.GrupoDeItens);
        }
    }
}
