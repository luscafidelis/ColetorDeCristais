using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Marca o objeto como capaz de coletar cristais (o jogador) e guarda a contagem de coletas.
    /// É só a ponte com a Unity: a contagem fica no <see cref="ContadorDeColeta"/>.
    /// </summary>
    public class ColetorDoJogador : MonoBehaviour
    {
        /// <summary>Cristais coletados por este jogador, no total e por tipo.</summary>
        public ContadorDeColeta Contador { get; } = new ContadorDeColeta();

        /// <summary>
        /// Registra a coleta de um cristal e aplica os efeitos da definição dele.
        /// Chamado pelo <see cref="CristalColetavel"/>.
        /// </summary>
        /// <param name="definicao">Definição do cristal coletado.</param>
        public void Registrar(DefinicaoCristal definicao)
        {
            if (definicao == null)
            {
                Debug.LogError($"{name}: cristal coletado sem definição; a coleta não foi contada.", this);
                return;
            }

            Contador.Registrar(definicao.Identificador);
            Debug.Log($"Cristal coletado: {definicao.NomeExibicao} ({definicao.Identificador}). Total: {Contador.Total}.", this);

            foreach (var efeito in definicao.Efeitos)
            {
                if (efeito != null)
                {
                    efeito.Aplicar(this);
                }
            }
        }
    }
}
