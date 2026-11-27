using ColetorDeCristais.Tema;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Tema
{
    /// <summary>
    /// Testes do tema: a conversão para o pastel do mundo (<see cref="PaletaPastel"/>) e as cores decididas
    /// (<see cref="PaletaDoTema"/>).
    /// </summary>
    public class TemaTestes
    {
        private const float Tolerancia = 0.002f;

        [Test]
        public void Pastel_SemClarearNemTirarSaturacao_IgualAOriginal()
        {
            var cor = new Color(0.8f, 0.3f, 0.1f, 1f);
            var pastel = PaletaPastel.Converter(cor, 0f, 1f);

            Assert.AreEqual(cor.r, pastel.r, Tolerancia);
            Assert.AreEqual(cor.g, pastel.g, Tolerancia);
            Assert.AreEqual(cor.b, pastel.b, Tolerancia);
        }

        [Test]
        public void Pastel_ClareiaETiraSaturacao()
        {
            var cor = new Color(0.8f, 0.3f, 0.1f, 1f);
            Color.RGBToHSV(cor, out var matiz, out var saturacao, out var brilho);

            var pastel = PaletaPastel.Converter(cor, PaletaDoTema.ClareamentoDoMundo, PaletaDoTema.SaturacaoDoMundo);
            Color.RGBToHSV(pastel, out var matizPastel, out var saturacaoPastel, out var brilhoPastel);

            Assert.AreEqual(matiz, matizPastel, Tolerancia, "mantém o matiz");
            Assert.AreEqual(saturacao * PaletaDoTema.SaturacaoDoMundo, saturacaoPastel, Tolerancia, "fica a fração da saturação");
            Assert.AreEqual(brilho + (1f - brilho) * PaletaDoTema.ClareamentoDoMundo, brilhoPastel, Tolerancia,
                "anda a fração do caminho até o branco");
        }

        [Test]
        public void Pastel_PretoViraCinzaClaro()
        {
            var pastel = PaletaPastel.Converter(Color.black, 0.3f, 0.7f);

            Assert.AreEqual(0.3f, pastel.r, Tolerancia);
            Assert.AreEqual(0.3f, pastel.g, Tolerancia);
            Assert.AreEqual(0.3f, pastel.b, Tolerancia);
        }

        [Test]
        public void Pastel_MantemATransparencia()
        {
            Assert.AreEqual(0.25f, PaletaPastel.Converter(new Color(0.2f, 0.6f, 0.4f, 0.25f), 0.3f, 0.7f).a);
        }

        [TestCase(-1f, 2f)]
        [TestCase(float.NaN, 1f)]
        public void Pastel_ValoresForaDaFaixa_UsamOLimite(float clareamento, float saturacao)
        {
            var cor = new Color(0.8f, 0.3f, 0.1f, 1f);
            var pastel = PaletaPastel.Converter(cor, clareamento, saturacao);

            Assert.AreEqual(cor.r, pastel.r, Tolerancia);
            Assert.AreEqual(cor.g, pastel.g, Tolerancia);
            Assert.AreEqual(cor.b, pastel.b, Tolerancia);
        }

        [Test]
        public void Pastel_ClareamentoTotal_Branco()
        {
            var pastel = PaletaPastel.Converter(new Color(0.8f, 0.3f, 0.1f, 1f), 1f, 0f);

            Assert.AreEqual(1f, pastel.r, Tolerancia);
            Assert.AreEqual(1f, pastel.g, Tolerancia);
            Assert.AreEqual(1f, pastel.b, Tolerancia);
        }

        [Test]
        public void Paleta_CoresDecididas()
        {
            Assert.AreEqual("5A0F14", ColorUtility.ToHtmlStringRGB(PaletaDoTema.Sangue));
            Assert.AreEqual("8E3B3B", ColorUtility.ToHtmlStringRGB(PaletaDoTema.Carne));
            Assert.AreEqual("6B7A3A", ColorUtility.ToHtmlStringRGB(PaletaDoTema.Gosma));
            Assert.AreEqual("4A0D14", ColorUtility.ToHtmlStringRGB(PaletaDoTema.Contorno));
        }
    }
}
