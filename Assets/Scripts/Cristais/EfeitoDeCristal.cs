using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Ponto de extensão para os efeitos de um cristal (ex.: velocidade, ímã, investida).
    /// Cada efeito é uma subclasse deste ScriptableObject, criada como asset e colocada na lista
    /// de efeitos da <see cref="DefinicaoCristal"/>. Exemplo: <see cref="EfeitoDeVelocidade"/>.
    /// </summary>
    public abstract class EfeitoDeCristal : ScriptableObject
    {
        /// <summary>Aplica o efeito no jogador que coletou o cristal.</summary>
        /// <param name="coletor">Componente de coleta do jogador; a partir dele dá para chegar aos outros componentes.</param>
        public abstract void Aplicar(ColetorDoJogador coletor);
    }
}
