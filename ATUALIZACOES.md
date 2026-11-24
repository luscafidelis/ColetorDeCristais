# Atualizações do Coletor de Cristais

Registro do que foi entrando no jogo, em ordem: da primeira atualização (no topo) até a mais recente (no fim). Cada entrada diz o que mudou para quem joga, o que mudou por dentro e como foi validada. Os detalhes técnicos de cada sistema ficam no [README](README.md).

As imagens ficam em [`docs/atualizacoes/`](docs/atualizacoes/) e são versionadas com Git LFS. As animadas são PNG animado (APNG): os navegadores e a pré-visualização de Markdown do VS Code mostram a animação. A exibição no GitHub (arquivos em LFS) ainda não foi conferida.

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

**Validação:** 84 testes EditMode passando. Em Play Mode, o personagem parou nas quatro muretas. Teste manual jogando; o controle por gamepad não tem teste confirmado.

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

**Validação:** 106 testes EditMode passando. Em Play Mode, o cristal do centro foi coletado ao iniciar, e um cristal de canto foi coletado com o personagem sobre ele. Teste manual jogando.

---

## 5. Visual de supermercado: comidas e arena da Kenney

**Para quem joga:** os itens coletáveis passaram a ser **comidas** com cerca de 0,8 m, que giram e flutuam como os cristais: carne crua no centro, peixe nos cantos noroeste e sudeste e coxa nos cantos nordeste e sudoeste. A arena ganhou **piso em placas e paredes de supermercado**. A coleta e o tamanho da arena (20×20 m) não mudaram.

**Por dentro:**
- Modelos do Food Kit e do Mini Shop, da Kenney (licença CC0), em `Assets/Terceiros/Kenney/`, com a licença original. Créditos em `CREDITOS.md`.
- Cada comida é uma *Prefab Variant* do `Cristal` (`Comida_Carne`, `Comida_Peixe`, `Comida_Coxa`): herda o trigger, a coleta e a animação, e troca só o visual.
- Colisão e visual separados: os cubos da arena continuam segurando o jogador, mas com o desenho desligado. O que aparece é o prefab `ArenaVisualKenney`, com 400 placas de piso, 80 paredes e 4 colunas.

**Validação:** 115 testes EditMode passando. Em Play Mode, as paredes seguraram o personagem no limite da arena, a carne do centro foi coletada ao iniciar e uma coxa de canto foi coletada com o personagem sobre ela, sem erros no Console.

---

## 6. O montrengo

**Para quem joga:** o personagem deixou de ser uma cápsula e virou o **montrengo**, um monstro de carne com olhos em antenas e a boca virada para a frente, com cerca de 2 m de largura e 1,6 m de altura. Parado, ele **pulsa** devagar (±4%, um pulso por segundo). Andando, **estica e achata** no ritmo do passo (até 10%) e **se inclina** até 12° na direção do movimento. Só o desenho mudou: a colisão, o movimento e a coleta continuam os mesmos.

![O montrengo no centro da arena de supermercado, com as comidas nos cantos](docs/atualizacoes/montrengo-na-arena.png)

**Por dentro:**
- Modelo "Cronenberg Monster Creature", de Joppe Monnens, sob licença CC BY 4.0, que **exige crédito**. Fica em `Assets/Terceiros/JoppeMonnens/CronenbergMonster/`, com a atribuição e a lista de modificações (texturas em 1K, mapas combinados para o URP, materiais recriados). Créditos em `CREDITOS.md`.
- Prefab `VisualMontrengo`: o modelo com o pivô nos pés, filho do `Jogador`. O desenho da cápsula foi desligado, e o colisor é o mesmo.
- Animação procedural em C# puro (`Assets/Scripts/Animacao/`): a `CalculadoraDeDeformacao` calcula o pulso, o esticar e achatar (preservando o volume) e a inclinação a partir da velocidade, e a `DeformacaoProcedural` aplica o resultado no visual. Os valores podem ser ajustados no Inspector.

**Validação:** 136 testes EditMode passando. Em Play Mode, parado, o pulso ficou em ±4%. Andando, o corpo achatou (0,975 na altura e 1,013 na largura) e se inclinou 12° para o lado do movimento, sem erros no Console. Teste jogando com o teclado não confirmado.

---

## 7. Coxa que acelera

**Para quem joga:** a coxa é o primeiro item com efeito. Ao pegar uma coxa, o personagem fica **30% mais rápido por 5 segundos**. Quando esse efeito acaba, fica um bônus **permanente de +0,3 m/s**. Pegar outra coxa durante o efeito soma mais 30% e reinicia os 5 segundos, e os bônus permanentes de cada coxa entram juntos no fim. Na velocidade base, isso dá 4,55 m/s por 5 s e depois 3,8 m/s. Carne e peixe continuam sem efeito. Ainda não há indicação na tela de que o efeito está ativo.

O personagem pega a coxa do canto superior direito e desce até o peixe, mais rápido:

![Animação: o personagem pega a coxa do canto superior direito e desce acelerado até o peixe do canto inferior direito](docs/atualizacoes/coxa-velocidade.png)

**Por dentro:**
- `Atributo` ganhou **modificadores agendados** (`AgendarAoFimDoGrupo`): eles ficam fora do cálculo até o grupo de temporários acabar.
- `BonusDeAtributo` (C# puro): um bônus de item com parte temporária e parte permanente, que entra junto ou só no fim do temporário.
- `EfeitoDeVelocidade`: o primeiro efeito concreto de `EfeitoDeCristal`. Ele aplica o bônus em `MovimentoDoJogador.Velocidade`.
- Novos assets: a definição `Coxa` e o efeito `VelocidadeCoxa`, com os valores ajustáveis no Inspector. A variante `Comida_Coxa` usa a definição `Coxa`.

**Validação:** 156 testes EditMode passando, incluindo o cenário da coxa (uma coxa, duas seguidas e coxa depois do fim do efeito). Em Play Mode, com o personagem posicionado sobre as coxas, a velocidade foi de 3,5 para 4,55 m/s, depois para 3,8 m/s no fim do efeito, para 4,94 m/s com a segunda coxa e para 4,1 m/s no fim. O teste jogando com o teclado ainda não foi feito.

---

## Como registrar uma nova atualização
1. Acrescente uma seção no fim, com o próximo número, seguindo o mesmo formato: **Para quem joga**, **Por dentro** e **Validação**, com o número de testes passando.
2. Não registre aqui planejamento, cronograma ou dados pessoais (veja a regra no `CLAUDE.md`).
3. Imagens em `docs/atualizacoes/`, com nomes descritivos em minúsculas e hífens. Com a Editor aberta e o jogo em Play Mode:
   ```powershell
   unity command capture game_view --project-path . -- --source camera --width 1280 --height 720 --output-path Temp/captura.png
   ```
   Depois, copie o arquivo de `Temp/` para `docs/atualizacoes/`.
4. Animações: grave os quadros com o `GravadorDeQuadros`, uma ferramenta só da Editor em `Assets/Scripts/Ferramentas/`. Ele fixa `Time.captureFramerate`, move o personagem por um roteiro de entradas simuladas e salva a câmera do jogo em `Temp/quadros/`. Com a Editor aberta na cena Arena:
   ```powershell
   unity command editor.playmode start --project-path .
   unity command script eval --project-path . --code "ColetorDeCristais.Ferramentas.GravadorDeQuadros.Iniciar();"
   unity command job wait --project-path . -- --condition '{"member":"ColetorDeCristais.Ferramentas.GravadorDeQuadros.Concluido","op":"equals","value":true}' --timeout 120
   unity command editor.playmode stop --project-path .
   ```
   Se a Editor estiver sem foco, rode antes `unity command editor set_auto_tick` e `unity command editor.playmode set_run_in_background`. Depois da gravação, desligue os dois (`-- --enabled false`). Para outro percurso, tamanho ou duração, edite os campos do componente (`etapas`, `largura`, `altura`, `quadrosPorSegundo`).

   Depois junte os quadros num PNG animado com a ferramenta do projeto, que usa só Python, sem pacotes extras:
   ```powershell
   python ferramentas/montar_apng.py Temp/quadros docs/atualizacoes/<nome>.png 50
   ```
   O último número é a duração de cada quadro em ms (50 = 20 quadros por segundo). Cada quadro depois do primeiro guarda só a região que mudou. A animação de movimento tem 147 quadros de 480×270 e ocupa cerca de 1,3 MB. Use sempre a câmera do jogo, para a animação mostrar o visual real.
