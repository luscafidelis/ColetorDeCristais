using System;
using ColetorDeCristais.Corpo;
using ColetorDeCristais.Nivel;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>Testes do <see cref="SorteioDeDrop"/>: limites da chance, frequência perto da chance pedida e onde o item pode cair.</summary>
    public class SorteioDeDropTestes
    {
        [TestCase(0.33f, 0.0, true)]
        [TestCase(0.33f, 0.3299, true)]
        [TestCase(0.33f, 0.3301, false)] // 0,33f em float é um pouco mais que 0,33
        [TestCase(0.33f, 0.99, false)]
        [TestCase(0f, 0.0, false)]
        [TestCase(1f, 0.9999, true)]
        [TestCase(-1f, 0.0, false)]
        [TestCase(2f, 0.9999, true)]
        public void Sorteou_QuandoONumeroEMenorQueAChance(float chance, double sorteio, bool esperado)
        {
            Assert.AreEqual(esperado, SorteioDeDrop.Sorteou(chance, sorteio));
        }

        [Test]
        public void ComMuitosSorteios_FrequenciaPertoDaChance()
        {
            var sorteio = new Random(1234);
            var caiu = 0;
            const int vezes = 30000;
            for (var i = 0; i < vezes; i++)
            {
                if (SorteioDeDrop.Sorteou(0.33f, sorteio.NextDouble()))
                {
                    caiu++;
                }
            }

            Assert.AreEqual(0.33, caiu / (double)vezes, 0.01);
        }

        [Test]
        public void NaN_Lanca()
        {
            Assert.Throws<ArgumentException>(() => SorteioDeDrop.Sorteou(float.NaN, 0.5));
            Assert.Throws<ArgumentException>(() => SorteioDeDrop.Sorteou(0.5f, double.NaN));
        }

        [Test]
        public void LocalPermitido_ForaDasZonas()
        {
            var zonas = new[] { new Retangulo(new UnityEngine.Vector2(5f, 5f), new UnityEngine.Vector2(2f, 2f)) };

            Assert.IsTrue(SorteioDeDrop.LocalPermitido(new UnityEngine.Vector2(0f, 0f), zonas));
            Assert.IsFalse(SorteioDeDrop.LocalPermitido(new UnityEngine.Vector2(5.5f, 4.5f), zonas), "dentro de um vão");
        }

        [Test]
        public void LocalPermitido_SemZonas_SempreAceita()
        {
            Assert.IsTrue(SorteioDeDrop.LocalPermitido(UnityEngine.Vector2.zero, null));
            Assert.IsTrue(SorteioDeDrop.LocalPermitido(UnityEngine.Vector2.zero, Array.Empty<Retangulo>()));
        }

        [Test]
        public void LocalPermitido_ZonaInvalida_Ignorada()
        {
            Assert.IsTrue(SorteioDeDrop.LocalPermitido(UnityEngine.Vector2.zero, new[] { default(Retangulo) }));
        }
    }
}
