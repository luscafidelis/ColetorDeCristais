using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Contorno
{
    /// <summary>
    /// Marca um objeto para ganhar contorno: os desenhos (<c>MeshRenderer</c> e <c>SkinnedMeshRenderer</c>) dele e dos
    /// filhos entram no contorno da tela, desenhado pelo <see cref="ContornoRendererFeature"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Os desenhos são lidos no <c>Awake</c>, inclusive os desligados; a cada quadro só entram os que estão
    /// ligados e ativos. Assim, o contorno some junto com o desenho (ex.: o pisca-pisca do dano).</item>
    /// <item>Cada desenho entra com todas as partes (submalhas) da malha dele, contadas no <c>Awake</c>
    /// (<see cref="ContagemDePartes"/>), mesmo que tenha menos ou mais materiais que partes.</item>
    /// <item>O objeto só entra na lista (<see cref="Ativos"/>) enquanto o componente está ligado: um item coletado ou
    /// destruído sai do contorno.</item>
    /// </list>
    /// </remarks>
    public class ContornoDoObjeto : MonoBehaviour
    {
        private static readonly ListaSemRepeticao<ContornoDoObjeto> _ativos = new ListaSemRepeticao<ContornoDoObjeto>();

        private Renderer[] _desenhos = Array.Empty<Renderer>();
        private int[] _partes = Array.Empty<int>();

        /// <summary>Objetos com contorno ligados agora.</summary>
        public static IReadOnlyList<ContornoDoObjeto> Ativos => _ativos.Itens;

        /// <summary>Os desenhos do objeto (incluindo os desligados agora).</summary>
        public IReadOnlyList<Renderer> Desenhos => _desenhos;

        /// <summary>Quantas partes (submalhas) cada desenho tem, na mesma ordem de <see cref="Desenhos"/>.</summary>
        public IReadOnlyList<int> PartesDosDesenhos => _partes;

        private void Awake()
        {
            _desenhos = Array.FindAll(GetComponentsInChildren<Renderer>(true), desenho => desenho is MeshRenderer || desenho is SkinnedMeshRenderer);
            _partes = new int[_desenhos.Length];
            for (var i = 0; i < _desenhos.Length; i++)
            {
                var malha = MalhaDo(_desenhos[i]);
                _partes[i] = ContagemDePartes(malha != null ? malha.subMeshCount : 0, _desenhos[i].sharedMaterials.Length);
            }
        }

        private static Mesh MalhaDo(Renderer desenho)
        {
            if (desenho is SkinnedMeshRenderer pele)
            {
                return pele.sharedMesh;
            }

            var filtro = desenho.GetComponent<MeshFilter>();
            return filtro != null ? filtro.sharedMesh : null;
        }

        private void OnEnable() => _ativos.Adicionar(this);

        private void OnDisable() => _ativos.Remover(this);

        /// <summary>Indica se o desenho deve entrar no contorno agora (existe, está ligado e ativo).</summary>
        public static bool Desenhavel(Renderer desenho) => desenho != null && desenho.enabled && desenho.gameObject.activeInHierarchy;

        /// <summary>
        /// Quantas partes de um desenho entram no contorno: as submalhas da malha; sem malha (zero submalhas), uma por
        /// material, e no mínimo uma.
        /// </summary>
        /// <param name="subMalhas">Submalhas da malha (<c>Mesh.subMeshCount</c>; zero se não houver malha).</param>
        /// <param name="materiais">Materiais do desenho.</param>
        public static int ContagemDePartes(int subMalhas, int materiais) => subMalhas > 0 ? subMalhas : Math.Max(1, materiais);
    }
}
