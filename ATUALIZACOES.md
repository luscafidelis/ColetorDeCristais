# Atualizações do Coletor de Cristais

Registro do que foi entrando no jogo, em ordem: da primeira atualização (no topo) até a mais recente (no fim). Cada entrada diz o que mudou para quem joga, o que mudou por dentro e como foi validada. Os detalhes técnicos de cada sistema ficam no [README](README.md).

As imagens ficam em [`docs/atualizacoes/`](docs/atualizacoes/) e são versionadas com Git LFS. As animadas são PNG animado (APNG): o GitHub, os navegadores e a pré-visualização de Markdown do VS Code mostram a animação.

---

## 1. Projeto e base de desenvolvimento

**Para quem joga:** ainda nada jogável.

**Por dentro:**
- Projeto Unity 6.3 LTS (6000.3.28f1) com URP ("Universal 3D"), para Windows.
- Repositório Git com `.gitignore` de Unity e Git LFS para binários (texturas, áudio, modelos, vídeo, fontes).
- Código do jogo no assembly `ColetorDeCristais` e testes EditMode no assembly `ColetorDeCristais.Testes.EditMode`.
- `README.md` e `CLAUDE.md` com as regras do projeto.

**Validação:** 2 testes EditMode passando, só para provar que a estrutura de testes funciona.

---

## 2. Atributos e modificadores

**Para quem joga:** nada visível ainda. Esta é a base para os cristais mudarem o personagem (ex.: mais velocidade).

**Por dentro** (`Assets/Scripts/Atributos/`, C# puro):
- `Atributo`: tem um valor base, uma lista de modificadores e um valor final. Também pode ter limites mínimo e máximo, que são opcionais.
- Três tipos de modificador, sempre aplicados nesta ordem: **plano**, **percentual aditivo** e **percentual multiplicativo**.
- Um modificador pode ser **permanente** ou **temporário**, com duração em segundos.
- Regra para temporários do mesmo grupo (ex.: o mesmo tipo de cristal coletado de novo): **o tempo reseta e os efeitos se somam**.

**Validação:** 62 testes EditMode passando. Eles cobrem a ordem de aplicação, valores negativos, limites, expiração e a renovação por grupo.

---

## 3. Arena e personagem que anda

**Para quem joga:** uma arena de 20×20 m vista de cima por uma câmera fixa inclinada a 60°. O personagem (cápsula azul, com um "nariz" branco que mostra a frente) anda com **WASD/setas** ou com o **analógico esquerdo** do gamepad, a 3,5 m/s, e gira suavemente para a direção do movimento. As muretas seguram o personagem dentro da arena.

![Arena vista pela câmera fixa, com o personagem no meio](docs/atualizacoes/arena.png)

**Por dentro** (`Assets/Scripts/Movimento/`):
- `CalculadoraDeMovimento` (C# pura): transforma a entrada em deslocamento (a diagonal não é mais rápida que as retas), calcula o ângulo alvo e gira pelo caminho mais curto.
- `MovimentoDoJogador` (`MonoBehaviour` fino): lê a ação `Player/Move` do Input System e move o personagem com um `CharacterController`.
- A velocidade do personagem já é um `Atributo`, pronta para receber modificadores.
- Nova cena `Assets/Scenes/Arena.unity`, que passa a ser a cena do build.

**Validação:** 84 testes EditMode passando. Em Play Mode, o personagem parou nas quatro muretas. O desenvolvedor testou com teclado e gamepad.

---

## 4. Cristais coletáveis

**Para quem joga:** cristais ciano em forma de losango esticado, girando e flutuando. Ao encostar, o personagem coleta o cristal, que some na hora. A arena tem 5 cristais: um no centro, coletado assim que o jogo começa, e um perto de cada canto. Ainda não há efeito, som nem contagem na tela.

![Cristais na arena, girando e flutuando](docs/atualizacoes/cristais-na-arena.png)

O personagem andando até um cristal do canto, coletando, e seguindo para o próximo:

![Animação: o personagem anda até o cristal do canto superior direito, coleta e desce até o cristal do canto inferior direito](docs/atualizacoes/movimento-e-coleta.png)

**Por dentro** (`Assets/Scripts/Cristais/`):
- `DefinicaoCristal` (ScriptableObject): os dados de um tipo de cristal (identificador, nome e lista de efeitos). O asset `CristalBase` está em `Assets/Dados/Cristais/`.
- `EfeitoDeCristal`: ponto de extensão para os efeitos futuros. Ainda não existe nenhum efeito.
- Prefab `Assets/Prefabs/Cristal.prefab`: um trigger na raiz e o visual animado num filho.
- Lógica pura e testada:
  - `EstadoDeColeta`: só o jogador coleta, e só uma vez.
  - `ContadorDeColeta`: total e quantidade por tipo, guardados em `ColetorDoJogador.Contador`.
  - `CalculadoraDeFlutuacao`: o giro e a flutuação.

**Validação:** 106 testes EditMode passando. Em Play Mode, o cristal do centro foi coletado ao iniciar, e um cristal de canto foi coletado com o personagem sobre ele. O desenvolvedor testou jogando.

---

## Como registrar uma nova atualização
1. Acrescente uma seção no fim, com o próximo número, seguindo o mesmo formato: **Para quem joga**, **Por dentro** e **Validação**, com o número de testes passando.
2. Não registre aqui planejamento, cronograma ou dados pessoais (veja a regra no `CLAUDE.md`).
3. Imagens em `docs/atualizacoes/`, com nomes descritivos em minúsculas e hífens. Com a Editor aberta e o jogo em Play Mode:
   ```powershell
   unity command capture game_view --project-path . -- --source camera --width 1280 --height 720 --output-path Temp/captura.png
   ```
   Depois, copie o arquivo de `Temp/` para `docs/atualizacoes/`.
4. Animações: grave uma sequência de PNGs do mesmo tamanho (ex.: com `Time.captureFramerate` fixo e um script temporário que salva um quadro por frame, apagado depois). Depois junte os quadros num PNG animado com a ferramenta do projeto, que usa só Python, sem pacotes extras:
   ```powershell
   python ferramentas/montar_apng.py <pasta_com_os_quadros> docs/atualizacoes/<nome>.png 50
   ```
   O último número é a duração de cada quadro em ms (50 = 20 quadros por segundo). Use sempre a câmera do jogo, para a animação mostrar o visual real. Mantenha cada animação em poucos MB; a de movimento tem 147 quadros de 480×270 e ocupa cerca de 4,4 MB.
