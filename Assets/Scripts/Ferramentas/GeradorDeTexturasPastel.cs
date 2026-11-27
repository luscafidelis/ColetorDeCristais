#if UNITY_EDITOR
using System.IO;
using ColetorDeCristais.Tema;
using UnityEditor;
using UnityEngine;

namespace ColetorDeCristais.Ferramentas
{
    /// <summary>
    /// Ferramenta só da Editor (não entra no build): cria a versão pastel dos kits da Kenney usados no jogo.
    /// </summary>
    /// <remarks>
    /// Para cada kit (<c>MiniShop</c> e <c>FoodKit</c>, em <c>Assets/Terceiros/Kenney/</c>):
    /// <list type="number">
    /// <item>Lê a textura original <c>Textures/colormap.png</c> e grava a versão pastel
    /// (<see cref="PaletaPastel"/>, com os valores da <see cref="PaletaDoTema"/>) em
    /// <c>Assets/Materiais/Pastel/&lt;Kit&gt;_colormap_pastel.png</c>, com as mesmas opções de importação.</item>
    /// <item>Cria ou atualiza o material <c>Assets/Materiais/Pastel/&lt;Kit&gt;_Pastel.mat</c>, cópia do material
    /// <c>colormap</c> dos modelos com a textura pastel.</item>
    /// <item>Em cada modelo do kit, troca o material <c>colormap</c> pelo pastel (aba <i>Materials</i> do importador,
    /// <i>Remapped Materials</i>). Os arquivos originais da Kenney não mudam.</item>
    /// </list>
    /// Rode com a Editor aberta e fora do Play Mode:
    /// <c>ColetorDeCristais.Ferramentas.GeradorDeTexturasPastel.Gerar();</c>
    /// </remarks>
    public static class GeradorDeTexturasPastel
    {
        private const string PastaDeSaida = "Assets/Materiais/Pastel";
        private static readonly string[] Kits = { "MiniShop", "FoodKit" };

        /// <summary>Gera as texturas e os materiais pastel e remapeia os modelos. Devolve um resumo.</summary>
        public static string Gerar()
        {
            if (!AssetDatabase.IsValidFolder(PastaDeSaida))
            {
                AssetDatabase.CreateFolder("Assets/Materiais", "Pastel");
            }

            var resumo = new System.Text.StringBuilder();
            foreach (var kit in Kits)
            {
                resumo.AppendLine(GerarKit(kit));
            }

            AssetDatabase.SaveAssets();
            return resumo.ToString();
        }

        private static string GerarKit(string kit)
        {
            var pastaDoKit = $"Assets/Terceiros/Kenney/{kit}";
            var caminhoOriginal = $"{pastaDoKit}/Textures/colormap.png";
            var caminhoPastel = $"{PastaDeSaida}/{kit}_colormap_pastel.png";

            var original = new Texture2D(2, 2);
            original.LoadImage(File.ReadAllBytes(caminhoOriginal));
            var pixels = original.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = PaletaPastel.Converter(pixels[i], PaletaDoTema.ClareamentoDoMundo, PaletaDoTema.SaturacaoDoMundo);
            }

            var pastel = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
            pastel.SetPixels(pixels);
            pastel.Apply();
            File.WriteAllBytes(caminhoPastel, pastel.EncodeToPNG());
            Object.DestroyImmediate(original);
            Object.DestroyImmediate(pastel);
            AssetDatabase.ImportAsset(caminhoPastel);

            // Mesmas opções de importação da textura original (filtro, mipmaps, compressão).
            var importadorOriginal = (TextureImporter)AssetImporter.GetAtPath(caminhoOriginal);
            var importadorPastel = (TextureImporter)AssetImporter.GetAtPath(caminhoPastel);
            EditorUtility.CopySerialized(importadorOriginal, importadorPastel);
            importadorPastel.SaveAndReimport();
            var texturaPastel = AssetDatabase.LoadAssetAtPath<Texture2D>(caminhoPastel);

            var caminhoDoMaterial = $"{PastaDeSaida}/{kit}_Pastel.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(caminhoDoMaterial);
            var modelos = AssetDatabase.FindAssets("t:Model", new[] { pastaDoKit });
            if (material == null)
            {
                var base_ = PrimeiroMaterialColormap(modelos);
                material = base_ != null ? new Material(base_) : new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, caminhoDoMaterial);
            }

            material.SetTexture("_BaseMap", texturaPastel);
            material.SetTexture("_MainTex", texturaPastel);
            EditorUtility.SetDirty(material);

            var remapeados = 0;
            var origem = new AssetImporter.SourceAssetIdentifier(typeof(Material), "colormap");
            foreach (var guid in modelos)
            {
                var importador = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as ModelImporter;
                if (importador == null)
                {
                    continue;
                }

                importador.AddRemap(origem, material);
                importador.SaveAndReimport();
                remapeados++;
            }

            return $"{kit}: {caminhoPastel}, {caminhoDoMaterial}, {remapeados} modelos remapeados.";
        }

        private static Material PrimeiroMaterialColormap(string[] modelos)
        {
            foreach (var guid in modelos)
            {
                foreach (var objeto in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    if (objeto is Material material && material.name == "colormap")
                    {
                        return material;
                    }
                }
            }

            return null;
        }
    }
}
#endif
