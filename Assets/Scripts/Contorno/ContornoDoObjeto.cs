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
    /// <item>O objeto só entra na lista (<see cref="Ativos"/>) enquanto o componente está ligado: um item coletado ou
    /// destruído sai do contorno.</item>
    /// </list>
    /// </remarks>
    public class ContornoDoObjeto : MonoBehaviour
    {
        private static readonly ListaSemRepeticao<ContornoDoObjeto> _ativos = new ListaSemRepeticao<ContornoDoObjeto>();

        private Renderer[] _desenhos = Array.Empty<Renderer>();

        /// <summary>Objetos com contorno ligados agora.</summary>
        public static IReadOnlyList<ContornoDoObjeto> Ativos => _ativos.Itens;

        /// <summary>Os desenhos do objeto (incluindo os desligados agora).</summary>
        public IReadOnlyList<Renderer> Desenhos => _desenhos;

        private void Awake()
        {
            _desenhos = Array.FindAll(GetComponentsInChildren<Renderer>(true), desenho => desenho is MeshRenderer || desenho is SkinnedMeshRenderer);
        }

        private void OnEnable() => _ativos.Adicionar(this);

        private void OnDisable() => _ativos.Remover(this);

        /// <summary>Indica se o desenho deve entrar no contorno agora (existe, está ligado e ativo).</summary>
        public static bool Desenhavel(Renderer desenho) => desenho != null && desenho.enabled && desenho.gameObject.activeInHierarchy;
    }
}
