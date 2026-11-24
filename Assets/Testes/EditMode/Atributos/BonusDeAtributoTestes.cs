using System;
using ColetorDeCristais.Atributos;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Atributos
{
    /// <summary>
    /// Testes do <see cref="BonusDeAtributo"/>. O cenário "coxa" usa os valores do asset do jogo
    /// (3,5 m/s de base, +30% por 5 s e +0,3 m/s permanente ao fim) para documentar o comportamento;
    /// os outros números são arbitrários.
    /// </summary>
    public class BonusDeAtributoTestes
    {
        private const float Tolerancia = 0.0001f;
        private const string Grupo = "coxa";

        private static BonusDeAtributo Coxa(MomentoDoPermanente momento = MomentoDoPermanente.AoFimDoTemporario) =>
            new BonusDeAtributo(TipoModificador.PercentualAditivo, 0.3f, 5f, Grupo, TipoModificador.Plano, 0.3f, momento);

        // --- Cenário da coxa ---

        [Test]
        public void Coxa_DuranteOEfeito_SoOTemporarioVale()
        {
            var velocidade = new Atributo(3.5f, valorMinimo: 0f);

            Coxa().AplicarEm(velocidade);

            Assert.AreEqual(3.5f * 1.3f, velocidade.ValorFinal, Tolerancia);
        }

        [Test]
        public void Coxa_AoFimDoEfeito_FicaOPermanente()
        {
            var velocidade = new Atributo(3.5f, valorMinimo: 0f);
            Coxa().AplicarEm(velocidade);

            velocidade.Atualizar(4.5f);
            Assert.AreEqual(3.5f * 1.3f, velocidade.ValorFinal, Tolerancia, "ainda no efeito");

            velocidade.Atualizar(0.5f);
            Assert.AreEqual(3.8f, velocidade.ValorFinal, Tolerancia);
        }

        [Test]
        public void Coxa_ColetarDeNovoDuranteOEfeito_SomaReiniciaEOsPermanentesEntramJuntos()
        {
            var velocidade = new Atributo(3.5f, valorMinimo: 0f);
            Coxa().AplicarEm(velocidade);
            velocidade.Atualizar(3f);

            Coxa().AplicarEm(velocidade);
            Assert.AreEqual(3.5f * 1.6f, velocidade.ValorFinal, Tolerancia, "+30% + 30%");

            velocidade.Atualizar(4.5f);
            Assert.AreEqual(3.5f * 1.6f, velocidade.ValorFinal, Tolerancia, "o tempo reiniciou na segunda coleta");

            velocidade.Atualizar(0.5f);
            Assert.AreEqual(4.1f, velocidade.ValorFinal, Tolerancia, "3,5 + 0,3 + 0,3");
        }

        [Test]
        public void Coxa_ColetarDepoisDoEfeito_OPermanenteAnteriorEntraNoCalculo()
        {
            var velocidade = new Atributo(3.5f, valorMinimo: 0f);
            Coxa().AplicarEm(velocidade);
            velocidade.Atualizar(5f);

            Coxa().AplicarEm(velocidade);

            Assert.AreEqual(3.8f * 1.3f, velocidade.ValorFinal, Tolerancia);
        }

        // --- Outras combinações ---

        [Test]
        public void PermanenteAoAplicar_EntraJuntoComOTemporario()
        {
            var atributo = new Atributo(10f);

            Coxa(MomentoDoPermanente.AoAplicar).AplicarEm(atributo);

            Assert.AreEqual(10.3f * 1.3f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void SoTemporario_ExpiraSemDeixarNada()
        {
            var atributo = new Atributo(10f);
            var bonus = new BonusDeAtributo(TipoModificador.Plano, 2f, 1f, Grupo, TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar);

            bonus.AplicarEm(atributo);
            atributo.Atualizar(1f);

            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.Modificadores.Count);
        }

        [Test]
        public void SoPermanente_EntraNaHora()
        {
            var atributo = new Atributo(10f);
            var bonus = new BonusDeAtributo(TipoModificador.Plano, 0f, 0f, null, TipoModificador.PercentualAditivo, 0.5f, MomentoDoPermanente.AoAplicar);

            bonus.AplicarEm(atributo);

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Aplicar_GuardaAOrigem()
        {
            var atributo = new Atributo(10f);
            var origem = new object();

            Coxa(MomentoDoPermanente.AoAplicar).AplicarEm(atributo, origem);

            Assert.AreEqual(2, atributo.RemoverModificadoresDaOrigem(origem));
        }

        [Test]
        public void Grupo_EspacosNasPontasSaoIgnorados()
        {
            var bonus = new BonusDeAtributo(TipoModificador.Plano, 1f, 1f, "  coxa ", TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar);

            Assert.AreEqual("coxa", bonus.Grupo);
        }

        // --- Validação ---

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => new BonusDeAtributo(TipoModificador.Plano, 1f, 0f, Grupo, TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar), "temporário sem duração");
            Assert.Throws<ArgumentException>(() => new BonusDeAtributo(TipoModificador.Plano, 1f, 2f, " ", TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar), "temporário sem grupo");
            Assert.Throws<ArgumentException>(() => new BonusDeAtributo(TipoModificador.Plano, 0f, 0f, null, TipoModificador.Plano, 1f, MomentoDoPermanente.AoFimDoTemporario), "espera um temporário que não existe");
            Assert.Throws<ArgumentException>(() => new BonusDeAtributo(TipoModificador.Plano, float.NaN, 2f, Grupo, TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar), "NaN");
            Assert.Throws<ArgumentException>(() => new BonusDeAtributo(TipoModificador.Plano, 0f, 0f, null, TipoModificador.Plano, float.PositiveInfinity, MomentoDoPermanente.AoAplicar), "infinito");
            Assert.Throws<ArgumentOutOfRangeException>(() => new BonusDeAtributo((TipoModificador)99, 1f, 2f, Grupo, TipoModificador.Plano, 0f, MomentoDoPermanente.AoAplicar), "tipo desconhecido");
            Assert.Throws<ArgumentOutOfRangeException>(() => new BonusDeAtributo(TipoModificador.Plano, 1f, 2f, Grupo, TipoModificador.Plano, 0f, (MomentoDoPermanente)7), "momento desconhecido");
        }

        [Test]
        public void Aplicar_EmAtributoNulo_LancaExcecao()
        {
            Assert.Throws<ArgumentNullException>(() => Coxa().AplicarEm(null));
        }
    }
}
