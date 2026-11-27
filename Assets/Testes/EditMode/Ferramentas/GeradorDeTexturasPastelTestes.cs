using ColetorDeCristais.Ferramentas;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Ferramentas
{
    /// <summary>Testes das verificações do <see cref="GeradorDeTexturasPastel"/>.</summary>
    public class GeradorDeTexturasPastelTestes
    {
        [Test]
        public void OriginalCarregada_ImagemDeVerdade_Verdadeiro()
        {
            Assert.IsTrue(GeradorDeTexturasPastel.OriginalCarregada(true, 512, 512));
        }

        [Test]
        public void OriginalCarregada_LoadImageFalhou_Falso()
        {
            Assert.IsFalse(GeradorDeTexturasPastel.OriginalCarregada(false, 512, 512));
        }

        [Test]
        public void OriginalCarregada_FicouNoTamanhoProvisorio_Falso()
        {
            Assert.IsFalse(GeradorDeTexturasPastel.OriginalCarregada(true, 2, 2));
        }

        [Test]
        public void PrecisaRemapear_SemRemapeamento_Verdadeiro()
        {
            var pastel = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            try
            {
                Assert.IsTrue(GeradorDeTexturasPastel.PrecisaRemapear(null, pastel));
            }
            finally
            {
                Object.DestroyImmediate(pastel);
            }
        }

        [Test]
        public void PrecisaRemapear_OutroMaterial_Verdadeiro()
        {
            var pastel = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var outro = new Material(pastel);
            try
            {
                Assert.IsTrue(GeradorDeTexturasPastel.PrecisaRemapear(outro, pastel));
            }
            finally
            {
                Object.DestroyImmediate(pastel);
                Object.DestroyImmediate(outro);
            }
        }

        [Test]
        public void PrecisaRemapear_JaApontaParaOPastel_Falso()
        {
            var pastel = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            try
            {
                Assert.IsFalse(GeradorDeTexturasPastel.PrecisaRemapear(pastel, pastel));
            }
            finally
            {
                Object.DestroyImmediate(pastel);
            }
        }
    }
}
