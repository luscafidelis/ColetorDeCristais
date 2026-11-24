using System;
using ColetorDeCristais.Corpo;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>Testes do <see cref="SorteioDeDrop"/>: limites da chance e frequência perto da chance pedida.</summary>
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
    }
}
