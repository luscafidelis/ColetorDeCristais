# Créditos

Todos os assets de terceiros usados no jogo, com criador, licença e origem. Esta lista é a base da tela de créditos do jogo.

## Assets de terceiros

| Pacote | Versão | Criador | Licença | Origem | No projeto | Usado em |
|---|---|---|---|---|---|---|
| Food Kit | 2.0 | Kenney ([www.kenney.nl](https://www.kenney.nl)) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [kenney.nl/assets/food-kit](https://kenney.nl/assets/food-kit) | `Assets/Terceiros/Kenney/FoodKit/` | Itens coletáveis: `meat-raw` (carne), `fish` (peixe), `whole-ham` (coxa) |
| Mini Shop | 2.1 | Kenney ([www.kenney.nl](https://www.kenney.nl)) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [kenney.nl/assets/mini-shop](https://kenney.nl/assets/mini-shop) | `Assets/Terceiros/Kenney/MiniShop/` | Piso (`floor`), paredes (`wall`) e colunas dos cantos (`column`) da Arena |
| Cronenberg Monster Creature | — | Joppe Monnens | [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) | [Sketchfab](https://sketchfab.com/3d-models/cronenberg-monster-creature-joppe-monnens-b469c9fe16f84845941bffaae9a06b00) | `Assets/Terceiros/JoppeMonnens/CronenbergMonster/` | Visual do jogador (o "montrengo"), com modificações: texturas em 1K, mapas metálico e rugosidade combinados para o URP, materiais recriados, escala e animação procedural |

Cada pasta traz o arquivo de licença do pacote (o `License.txt` original da Kenney, ou um `LICENSE.txt` com a atribuição quando o pacote não traz um).

**Licenças e obrigação de crédito:**
- **CC0 1.0** (Kenney): domínio público, não exige crédito. O registro mantém a origem de cada asset.
- **CC BY 4.0** (Joppe Monnens): **o crédito é obrigatório**, no repositório e na tela de créditos do jogo. Ele precisa ter o nome do autor, o link da obra, o link da licença e o aviso de que houve modificação. Texto pronto:

  > "Cronenberg Monster Creature" (https://sketchfab.com/3d-models/cronenberg-monster-creature-joppe-monnens-b469c9fe16f84845941bffaae9a06b00) by Joppe Monnens, licensed under CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/). Modified.

## Como registrar um asset novo
Ao importar qualquer asset de terceiros (modelo, textura, som, fonte), acrescente uma linha na tabela acima no mesmo conjunto de alterações, com: pacote, versão, criador, licença (com link), página de origem, pasta no projeto e onde é usado. Guarde o arquivo de licença original junto do asset, em `Assets/Terceiros/<Criador>/<Pacote>/`.
