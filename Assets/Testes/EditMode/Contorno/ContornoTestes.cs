using ColetorDeCristais.Contorno;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Contorno
{
    /// <summary>
    /// Testes do contorno: a espessura em UV (<see cref="MedidaDoContorno"/>), a lista dos objetos ligados
    /// (<see cref="ListaSemRepeticao{T}"/>) e quais desenhos entram (<see cref="ContornoDoObjeto.Desenhavel"/>).
    /// </summary>
    public class ContornoTestes
    {
        private const float Tolerancia = 1e-6f;

        [Test]
        public void Medida_Em1080_UmPixelPorUnidade()
        {
            var deslocamento = MedidaDoContorno.Deslocamento(3f, 1920, 1080);

            Assert.AreEqual(3f / 1080f, deslocamento.y, Tolerancia);
            Assert.AreEqual(3f / 1920f, deslocamento.x, Tolerancia);
        }

        [Test]
        public void Medida_AcompanhaAResolucao()
        {
            // Em 4K, 3 pixels de 1080 viram 6 pixels: o deslocamento em UV é o mesmo.
            var hd = MedidaDoContorno.Deslocamento(3f, 1920, 1080);
            var quatroK = MedidaDoContorno.Deslocamento(3f, 3840, 2160);

            Assert.AreEqual(hd.x, quatroK.x, Tolerancia);
            Assert.AreEqual(hd.y, quatroK.y, Tolerancia);
            Assert.AreEqual(6f, quatroK.y * 2160f, 1e-4f);
        }

        [Test]
        public void Medida_MesmoNumeroDePixelsNosDoisEixos()
        {
            var deslocamento = MedidaDoContorno.Deslocamento(4f, 1280, 720);

            Assert.AreEqual(deslocamento.y * 720f, deslocamento.x * 1280f, 1e-4f);
        }

        [TestCase(0f, 1920, 1080)]
        [TestCase(-1f, 1920, 1080)]
        [TestCase(float.NaN, 1920, 1080)]
        [TestCase(float.PositiveInfinity, 1920, 1080)]
        [TestCase(3f, 0, 1080)]
        [TestCase(3f, 1920, 0)]
        public void Medida_ValoresInvalidos_SemContorno(float espessura, int largura, int altura)
        {
            Assert.AreEqual(Vector2.zero, MedidaDoContorno.Deslocamento(espessura, largura, altura));
        }

        [Test]
        public void Lista_NaoAceitaRepetidoNemNulo()
        {
            var lista = new ListaSemRepeticao<string>();

            Assert.IsTrue(lista.Adicionar("a"));
            Assert.IsFalse(lista.Adicionar("a"));
            Assert.IsFalse(lista.Adicionar(null));
            Assert.IsTrue(lista.Adicionar("b"));

            CollectionAssert.AreEqual(new[] { "a", "b" }, lista.Itens);
        }

        [Test]
        public void Lista_Remover()
        {
            var lista = new ListaSemRepeticao<string>();
            lista.Adicionar("a");
            lista.Adicionar("b");

            Assert.IsTrue(lista.Remover("a"));
            Assert.IsFalse(lista.Remover("a"));
            Assert.IsFalse(lista.Remover(null));
            CollectionAssert.AreEqual(new[] { "b" }, lista.Itens);
        }

        [Test]
        public void Desenhavel_SoLigadoEAtivo()
        {
            var objeto = new GameObject("TesteContorno");
            try
            {
                var desenho = objeto.AddComponent<MeshRenderer>();
                Assert.IsTrue(ContornoDoObjeto.Desenhavel(desenho), "ligado e ativo");

                desenho.enabled = false;
                Assert.IsFalse(ContornoDoObjeto.Desenhavel(desenho), "desligado (ex.: pisca-pisca do dano)");

                desenho.enabled = true;
                objeto.SetActive(false);
                Assert.IsFalse(ContornoDoObjeto.Desenhavel(desenho), "objeto inativo");

                Assert.IsFalse(ContornoDoObjeto.Desenhavel(null), "nulo");
            }
            finally
            {
                Object.DestroyImmediate(objeto);
            }
        }
    }
}
