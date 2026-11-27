using UnityEngine;

namespace ColetorDeCristais.Tema
{
    /// <summary>
    /// Paleta do tema "Carne Viva": o mundo pastel da Kenney e o lado sanguinolento em tons escuros e realistas.
    /// As cores ficam aqui para que código, materiais e documentação usem os mesmos valores (ver o README, "Tema e
    /// paleta").
    /// </summary>
    public static class PaletaDoTema
    {
        /// <summary>Sangue: bordô escuro (#5A0F14).</summary>
        public static readonly Color Sangue = new Color32(0x5A, 0x0F, 0x14, 0xFF);

        /// <summary>Carne: vermelho-carne escuro (#8E3B3B). Também o ícone do Bote na HUD.</summary>
        public static readonly Color Carne = new Color32(0x8E, 0x3B, 0x3B, 0xFF);

        /// <summary>Gosma: oliva (#6B7A3A). Também o ícone do Faro na HUD.</summary>
        public static readonly Color Gosma = new Color32(0x6B, 0x7A, 0x3A, 0xFF);

        /// <summary>Contorno dos objetos: bordô bem escuro (#4A0D14).</summary>
        public static readonly Color Contorno = new Color32(0x4A, 0x0D, 0x14, 0xFF);

        /// <summary>
        /// Tinta multiplicada na textura da Carniça e do chefe (#B06A6A): escurece e puxa a carne para o bordô.
        /// </summary>
        public static readonly Color TintaDaCarne = new Color32(0xB0, 0x6A, 0x6A, 0xFF);

        /// <summary>Quanto o mundo pastel clareia as cores da Kenney (0,3: 30% do caminho até o branco).</summary>
        public const float ClareamentoDoMundo = 0.3f;

        /// <summary>Quanto da saturação das cores da Kenney fica no mundo pastel (0,7 = 70%).</summary>
        public const float SaturacaoDoMundo = 0.7f;
    }
}
