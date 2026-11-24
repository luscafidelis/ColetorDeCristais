# Coletor de Cristais

Jogo 3D em Unity sobre coletar cristais numa arena.

## O jogo
Arena 3D com câmera fixa elevada. O jogador anda pela arena e coleta cristais. Cada cristal é um item com um efeito:
- **Atributo:** aumenta a velocidade.
- **Habilidade passiva:** ímã, que puxa cristais próximos.
- **Habilidade ativa:** investida, com tempo de recarga.

Plataforma: Windows. Sem monetização.

> Estado atual: a arena com o personagem que anda (teclado ou gamepad), desenhado como um monstro de carne (o "montrengo", modelo de Joppe Monnens) que pulsa parado e estica, achata e se inclina ao andar, e a câmera fixa elevada; o sistema de atributos com modificadores permanentes e temporários (a velocidade do personagem já é um atributo); os itens coletáveis com visual de comida (carne, peixe e coxa), que somem ao ser coletados e são contados, numa arena de piso e paredes de supermercado, com modelos da Kenney. A coxa acelera o personagem (+30% por 5 s e, ao fim, +0,3 m/s permanente); o café libera a investida, um avanço rápido com recarga de 5 s (4,375 m na velocidade base), durante o qual o montrengo estica para a frente; carne e peixe ainda não têm efeito. Contagem, recarga e efeitos na tela e sons ainda não foram implementados.

## Requisitos
| Item | Versão |
|---|---|
| Unity Editor | 6000.3.28f1 (Unity 6.3 LTS) |
| Unity CLI | `unity` 1.0.x |
| Render pipeline | URP (template `com.unity.template.urp-blank`, "Universal 3D") |
| Editor de scripts | VS Code, com as extensões C# Dev Kit e Unity |
| .NET SDK | 9.0.200 ou superior, necessário para abrir o `.slnx` |
| Git + Git LFS | Git LFS ativado neste repositório |

## Como abrir e rodar
Na raiz do projeto:
```powershell
unity open .
unity status --until-ready --project-path .
```
Na Editor, abra `Assets/Scenes/Arena.unity` e aperte Play. Clique na janela Game para ela receber o teclado.

**Controles:**
| Ação | Teclado | Gamepad |
|---|---|---|
| Andar | WASD ou setas | Analógico esquerdo |
| Investida (depois de pegar o café) | Espaço | Gatilho direito (RT/R2) |

Depois de clonar o repositório pela primeira vez, rode `git lfs install` (uma vez por máquina) e `git lfs pull` para baixar os binários.

## Como rodar os testes
Os testes são EditMode (NUnit, Unity Test Framework). Rode os comandos na raiz do projeto.

**Editor fechada** (modo batch; a CLI recusa o modo batch se a Editor estiver aberta nesse projeto):
```powershell
unity test . --mode EditMode
```

**Editor aberta:**
```powershell
unity command test run --mode editor --filter ColetorDeCristais.Testes.EditMode --filter-type Assembly --project-path .
```

Na Editor também dá para usar *Window > General > Test Runner*, aba EditMode.

**Ferramentas em Python** (`ferramentas/`, fora da Unity):
```powershell
python -m unittest discover -s ferramentas
```
(`python -m unittest ferramentas.test_montar_apng` também funciona.)

## Estrutura de pastas
| Caminho | Conteúdo |
|---|---|
| `Assets/Scripts/` | Código do jogo. Assembly `ColetorDeCristais` (`ColetorDeCristais.asmdef`), namespace `ColetorDeCristais` |
| `Assets/Scripts/InformacoesDoProjeto.cs` | Classe mínima (nome do jogo), criada com o projeto para validar a estrutura de testes |
| `Assets/Scripts/Atributos/` | Sistema de atributos em C# puro (namespace `ColetorDeCristais.Atributos`): `Atributo`, `Modificador`, `TipoModificador`, `BonusDeAtributo` e `MomentoDoPermanente`. Ver "Atributos e modificadores" abaixo |
| `Assets/Scripts/Movimento/` | Movimento do personagem (namespace `ColetorDeCristais.Movimento`): `CalculadoraDeMovimento` (cálculos puros) e `MovimentoDoJogador` (`MonoBehaviour`). Ver "Movimento do personagem" abaixo |
| `Assets/Scripts/Habilidades/` | Habilidades ativas (namespace `ColetorDeCristais.Habilidades`): `Investida` e `ParametrosDeInvestida` (C# puro), `DefinicaoInvestida` (ScriptableObject) e `InvestidaDoJogador` (`MonoBehaviour`). Ver "Investida" abaixo |
| `Assets/Testes/EditMode/` | Testes EditMode. Assembly `ColetorDeCristais.Testes.EditMode`, namespace `ColetorDeCristais.Testes`; referencia `ColetorDeCristais` e só compila na Editor. Subpastas espelham as de `Assets/Scripts/` (ex.: `Atributos/`, `Movimento/`) |
| `Assets/Scenes/Arena.unity` | Cena do jogo (única cena no Build Profile): arena, jogador, câmera, luz |
| `Assets/Scenes/SampleScene.unity` | Cena de exemplo do template, fora do build; serviu de base para a Arena |
| `Assets/Scripts/Cristais/` | Cristais coletáveis (namespace `ColetorDeCristais.Cristais`): lógica pura (`EstadoDeColeta`, `ContadorDeColeta`, `CalculadoraDeFlutuacao`), dados (`DefinicaoCristal`, `EfeitoDeCristal`) e componentes (`CristalColetavel`, `ColetorDoJogador`, `GiroEFlutuacao`). Ver "Cristais coletáveis" abaixo |
| `Assets/Scripts/Animacao/` | Animação procedural (namespace `ColetorDeCristais.Animacao`): `CalculadoraDeDeformacao`, `ParametrosDeDeformacao` e `PoseDeDeformacao` (C# puro) e `DeformacaoProcedural` (`MonoBehaviour`). Ver "Visual do jogador (montrengo)" abaixo |
| `Assets/Scripts/Ferramentas/` | Ferramentas de desenvolvimento (namespace `ColetorDeCristais.Ferramentas`): `GravadorDeQuadros`, só da Editor (`#if UNITY_EDITOR`, fica fora do build), grava quadros da câmera do jogo seguindo um `RoteiroDeEntrada` (C# puro, testado). Uso no `ATUALIZACOES.md` |
| `Assets/Prefabs/Cristal.prefab` | Prefab base do item coletável: trigger, coleta e visual animado (losango ciano) |
| `Assets/Prefabs/Comida_Carne.prefab`, `Comida_Peixe.prefab`, `Comida_Coxa.prefab`, `Comida_Cafe.prefab` | Prefab Variants de `Cristal` com modelos de comida do Food Kit (ver "Cristais coletáveis") |
| `Assets/Prefabs/VisualMontrengo.prefab` | Visual do jogador: o modelo do montrengo com a `DeformacaoProcedural`, sem colisão (ver "Visual do jogador (montrengo)") |
| `Assets/Prefabs/ArenaVisualKenney.prefab` | Visual da Arena: piso e paredes do Mini Shop, sem colisão (ver "Movimento do personagem") |
| `Assets/Terceiros/Kenney/` | Assets de terceiros da Kenney (CC0): `FoodKit/` e `MiniShop/`, em FBX com textura `Textures/colormap.png` e o `License.txt` de cada pacote. Créditos em [`CREDITOS.md`](CREDITOS.md) |
| `Assets/Terceiros/JoppeMonnens/CronenbergMonster/` | Modelo do montrengo (CC BY 4.0, crédito obrigatório): `CronenBergFinal.fbx`, `Texturas/` (cor e normal originais e o mapa metálico+suavidade combinado, importados em 1K), `Materiais/` (`Montrengo_Corpo`, `Montrengo_Osso`, URP Lit) e `LICENSE.txt` com a atribuição e as modificações |
| `Assets/Dados/Cristais/` | Assets `DefinicaoCristal`: `CristalBase` (carne e peixe, sem efeito), `Coxa` e `Cafe`. Em `Efeitos/`, os assets de efeito (`VelocidadeCoxa`, `LiberarInvestida`) |
| `Assets/Dados/Habilidades/` | Assets de valores das habilidades: `Investida` (`DefinicaoInvestida`) |
| `Assets/Materiais/` | Materiais URP Lit: `Chao`, `Mureta`, `Jogador`, `NarizJogador`, `Cristal` |
| `Assets/Settings/` | Assets do URP (pipelines PC e Mobile, renderers, volume profiles) |
| `Assets/InputSystem_Actions.inputactions` | Ações do Input System (vindas do template). O personagem usa `Player/Move` e `Player/Investida`. É o asset de ações do projeto (*Project Settings > Input System Package*) |
| `Assets/TutorialInfo/`, `Assets/Readme.asset` | Readme do template da Unity (pode ser removido quando atrapalhar) |
| `Packages/` | `manifest.json` e lock dos pacotes UPM. Inclui `com.unity.pipeline` (liga a CLI à Editor) e `com.unity.test-framework` |
| `ProjectSettings/` | Configurações do projeto |
| `CREDITOS.md` | Créditos e licenças de todos os assets de terceiros |
| `ATUALIZACOES.md` | Registro das atualizações do jogo, em ordem, com imagens |
| `docs/atualizacoes/` | Imagens e animações (APNG) do registro de atualizações (Git LFS) |
| `ferramentas/` | Ferramentas de apoio em Python, sem pacotes extras. `montar_apng.py` junta PNGs num PNG animado: lê os quadros em ordem natural do nome, ignora a própria saída se ela estiver na pasta, copia paleta e chunks de cor e guarda só a região alterada de cada quadro (RGB/RGBA de 8 bits). Tem testes (`test_montar_apng.py`) |

Pastas geradas (`Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, `.csproj`, `.slnx`) estão no `.gitignore`.

## Atributos e modificadores
Código em `Assets/Scripts/Atributos/`, C# puro, sem `MonoBehaviour` e sem dependência da Unity. Um `Atributo` (ex.: velocidade) tem um **valor base** e uma lista de **modificadores**, permanentes ou temporários; o **valor final** é recalculado quando algo muda.

**Tipos de modificador** (`TipoModificador`). Nos percentuais, o valor é uma fração: `0.1` = 10%.
| Tipo | Efeito | Dois modificadores de `0.1` |
|---|---|---|
| `Plano` | Soma ao valor base | +0,2 |
| `PercentualAditivo` | Os percentuais se somam e o total multiplica uma vez | ×1,2 |
| `PercentualMultiplicativo` | Cada um multiplica de forma independente | ×1,21 |

**Ordem de aplicação** (fixa, não depende da ordem em que os modificadores foram adicionados):
```
final = (base + Σ planos) × (1 + Σ percentuais aditivos) × Π (1 + percentual multiplicativo)
final = limitar(final, ValorMinimo, ValorMaximo)
```

**Regras:**
- Valores negativos são aceitos na base e nos modificadores (reduzem o atributo). Um multiplicativo de `-1` zera o valor.
- Os limites `ValorMinimo`/`ValorMaximo` são opcionais (padrão: sem limite) e valem só para o valor final. Sem limites, o valor final pode ficar negativo.
- Valores NaN ou infinitos geram `ArgumentException`; mínimo maior que máximo também.
- A remoção é pela instância do modificador (`RemoverModificador`), por origem (`RemoverModificadoresDaOrigem`, ex.: tudo que um item concedeu) ou total (`RemoverTodosModificadores`). Adicionar duas vezes a mesma instância, ou adicionar uma instância que está agendada, gera `InvalidOperationException`. `Modificadores` e `ModificadoresAgendados` são somente leitura: só mudam pelos métodos do atributo.

**Permanentes e temporários:**
- Sem `duracao`, o modificador é **permanente**: só sai quando é removido.
- Com `duracao` (segundos, finita e maior que zero), o modificador é **temporário**. O tempo só passa quando quem usa o atributo chama `Atualizar(deltaTempo)` (ex.: uma vez por quadro com `Time.deltaTime`). Quando o tempo restante chega a zero, o modificador é removido e o valor final volta a ser calculado. `Atualizar` devolve quantos modificadores expiraram.
- `TentarObterTempoRestante(modificador, out tempo)` informa quanto falta (útil para a interface).
- **Renovação por grupo:** quando dois cristais temporários com o mesmo modificador são coletados, o tempo reseta e os modificadores se somam. Um temporário pode ter um `grupo` (texto, ex.: o tipo de cristal). Ao adicionar um temporário de um grupo que já está ativo, **todos** os temporários desse grupo voltam à duração completa do novo modificador, e os efeitos continuam somados. Por isso eles expiram juntos. Temporários sem grupo têm tempos independentes. Grupo só é aceito em temporários (`ArgumentException` em permanentes). Espaços nas pontas do grupo são removidos, e um grupo vazio ou só com espaços conta como sem grupo.

**Agendados para o fim de um grupo:** `AgendarAoFimDoGrupo(grupo, modificador)` guarda um modificador fora do cálculo até o grupo não ter mais temporários ativos. Aí ele entra no atributo (ex.: um bônus permanente que só vale depois que o efeito temporário acaba). Se o grupo já não tiver temporários, ele entra na hora. Renovar o grupo adia todos os agendados dele para o novo fim. A verificação acontece sempre que um modificador sai do atributo (expirou ou foi removido). O grupo é comparado sem os espaços nas pontas, como no `Modificador`. `ModificadoresAgendados` lista os que estão esperando. `RemoverModificadoresDaOrigem` também descarta os agendados da origem, e `RemoverTodosModificadores` descarta todos.

**Bônus de itens (`BonusDeAtributo`):** descreve o que um item faz num atributo, em duas partes opcionais (valor zero desliga a parte):
- **temporária:** tipo, valor, duração e grupo (obrigatórios quando há parte temporária);
- **permanente:** tipo, valor e `MomentoDoPermanente`: `AoAplicar` (entra junto) ou `AoFimDoTemporario` (agendada para o fim do grupo da parte temporária).

`AplicarEm(atributo, origem)` cria modificadores novos a cada chamada. Coletar de novo durante o efeito soma a parte temporária e reinicia o tempo (regra do grupo). Com `AoFimDoTemporario`, cada coleta agenda mais uma parte permanente, e todas entram juntas no fim. Combinações inválidas (temporário sem duração ou sem grupo, permanente esperando um temporário que não existe, valores não finitos) geram `ArgumentException`.

**Exemplo** (números ilustrativos, não são valores de jogo):
```csharp
var velocidade = new Atributo(valorBase: 5f, valorMinimo: 0f);

// Permanente, removido pela origem
var bonus = Modificador.PercentualAditivo(0.2f, origem: itemQueConcedeu);
velocidade.AdicionarModificador(bonus);                   // ValorFinal == 6
velocidade.RemoverModificadoresDaOrigem(itemQueConcedeu); // volta a 5

// Temporários do mesmo grupo: somam e o tempo reseta
velocidade.AdicionarModificador(Modificador.Plano(1f, duracao: 4f, grupo: "cristal-velocidade")); // 6
velocidade.Atualizar(3f);                                                                         // falta 1 s
velocidade.AdicionarModificador(Modificador.Plano(1f, duracao: 4f, grupo: "cristal-velocidade")); // 7, os dois com 4 s
velocidade.Atualizar(4f);                                                                         // os dois expiram: 5

// Bônus com parte permanente que só entra no fim do temporário
var bonusDeItem = new BonusDeAtributo(TipoModificador.PercentualAditivo, 0.5f, 2f, "item",
                                      TipoModificador.Plano, 1f, MomentoDoPermanente.AoFimDoTemporario);
bonusDeItem.AplicarEm(velocidade); // 7,5 (5 × 1,5); o +1 fica agendado
velocidade.Atualizar(2f);          // o temporário expira e o +1 entra: 6
```

Os valores concretos de cada atributo e item (base, bônus, limites, durações e quais cristais são temporários) não fazem parte deste sistema: ficam nos componentes e assets que o usam.

## Movimento do personagem
**Como funciona no jogo:** controles por teclado (WASD/setas) e gamepad (analógico esquerdo); velocidade base 3,5 m/s; o personagem gira suavemente para a direção do movimento (720°/s); arena 20×20 m com piso em placas e paredes do Mini Shop (Kenney); câmera fixa inclinada a 60°.

**Código** (`Assets/Scripts/Movimento/`):
- `CalculadoraDeMovimento` (estática, sem `MonoBehaviour`, testada em EditMode):
  - `CalcularDeslocamento(entrada, velocidade, deltaTempo)`: a entrada 2D vira um deslocamento no chão (x da entrada → X do mundo, y da entrada → Z). A entrada é limitada a tamanho 1, então a diagonal do teclado não é mais rápida. Velocidade negativa conta como zero.
  - `TentarCalcularAnguloAlvo(entrada, out angulo)`: ângulo Y para onde olhar (0° = +Z, 90° = +X). Entradas menores que `EntradaMinima` (0,01) não giram o personagem.
  - `GirarEmDirecao(atual, alvo, velocidadeAngular, deltaTempo)`: gira pelo caminho mais curto sem passar do alvo.
- `MovimentoDoJogador` (`MonoBehaviour`, exige `CharacterController`): lê a ação, chama a calculadora, aplica com `CharacterController.Move` e ajusta a rotação. Não tem lógica própria.
  - `LerEntrada()`: lê a entrada de movimento na hora, direto da ação. A investida usa como direção; como ela roda antes deste componente, assim recebe a entrada do próprio quadro.
  - `Suspenso`: enquanto for `true`, o componente não move nem gira o personagem, mas continua chamando `Velocidade.Atualizar` (os efeitos temporários continuam perdendo tempo). A investida liga no início do avanço e desliga no fim; ela só escreve na flag nessas duas transições.

**Decisões técnicas:**
- A velocidade é um `Atributo` (`MovimentoDoJogador.Velocidade`), criado no `Awake` com o valor base do Inspector e mínimo 0. Itens aplicam modificadores nele. O componente chama `Velocidade.Atualizar(Time.deltaTime)` a cada quadro, então modificadores temporários expiram sozinhos. Mudar o valor base no Inspector durante o Play não tem efeito (use `Velocidade.ValorBase`).
- `CharacterController` em vez de `Rigidbody`: colide com as muretas sem precisar de física, e continua funcionando com triggers para coletar itens. Não há gravidade nem pulo: o deslocamento é sempre horizontal (o chão é plano).
- A entrada vem por `InputActionReference` (campo **Acao Mover** no Inspector), apontando para `Player/Move` no `InputSystem_Actions`. O componente habilita a ação no `OnEnable`.
- A câmera não tem script: é só a `Main Camera` posicionada. "Para cima" no controle anda para +Z, que é para onde a câmera olha.

**Como a cena foi montada** (para reproduzir na Editor):
1. **Cena:** `Assets/Scenes/Arena.unity` é uma cópia da `SampleScene` (que já tem `Directional Light`, `Global Volume` e `Main Camera` do URP). Em *File > Build Profiles*, ela é a única cena da lista.
2. **Materiais:** em `Assets/Materiais`, *Create > Material* (shader *Universal Render Pipeline/Lit*), mudando só a *Base Map* (cor): `Chao` cinza claro, `Mureta` cinza escuro, `Jogador` azul, `NarizJogador` branco.
3. **Arena:** um GameObject vazio `Arena` com cubos (*GameObject > 3D Object > Cube*):
   | Objeto | Position | Scale |
   |---|---|---|
   | `Chao` | (0, -0,1, 0) | (20, 0,2, 20) |
   | `MuretaNorte` / `MuretaSul` | (0, 0,5, ±10,25) | (21, 1, 0,5) |
   | `MuretaLeste` / `MuretaOeste` | (±10,25, 0,5, 0) | (0,5, 1, 20) |

   O topo do chão fica em y = 0 e as muretas ficam por fora da área de 20×20. Os cubos mantêm o `Box Collider` padrão (é ele que segura o jogador) e estão marcados como *Static*.

   **Visual da arena (Kenney):** os cubos são só colisão. O `Mesh Renderer` deles está desligado (desmarque o componente no Inspector). O que aparece é `Arena/VisualKenney`, uma instância do prefab `Assets/Prefabs/ArenaVisualKenney.prefab`. Com as peças dentro de um prefab, a cena guarda uma referência só, e não 484 objetos. Para mudar o visual, abra o prefab com dois cliques. Ele tem modelos do Mini Shop arrastados de `Assets/Terceiros/Kenney/MiniShop/`, todos *Static* e sem collider:
   - `Piso`: 400 placas `floor` (1×1 m), de (-9,5; 0; -9,5) a (9,5; 0; 9,5), de metro em metro.
   - `Paredes`: 20 peças `wall` (1 m de largura, 1 m de altura, 0,6 m de espessura) por lado, a 10,3 m do centro. Assim a face interna fica no limite de 10 m. Rotação Y: norte 180°, sul 0°, leste 270°, oeste 90°. Nos quatro cantos há uma `column`.

   Separar colisão e visual permite trocar o visual da arena sem mexer na física.
4. **Jogador:** *3D Object > Capsule* em (0, 1, 0), tag `Player`.
   - Remova o `Capsule Collider` e adicione um `Character Controller` (*Height* 2, *Radius* 0,5, *Center* (0, 0, 0)). O `Character Controller` já faz a colisão.
   - Filho `Nariz`: um cubo em (0, 0,5, 0,45), escala (0,3, 0,2, 0,3), sem `Box Collider` e com o material `NarizJogador`. Ele mostrava para onde a cápsula estava virada.
   - Adicione o componente `MovimentoDoJogador`. Em **Acao Mover**, escolha `Player/Move` (clique no círculo ao lado do campo). Deixe **Velocidade Base** em 3,5 e **Velocidade De Giro** em 720.
   - **Visual do montrengo:** desligue o `Mesh Renderer` da cápsula e desative o `Nariz`. Arraste o prefab `VisualMontrengo` para dentro do `Jogador`, em (0, -1, 0), que é a base do `Character Controller` (os pés). Detalhes em "Visual do jogador (montrengo)".
5. **Câmera:** `Main Camera` com *Rotation* (60, 0, 0), *Position* (0, 19,27, -13,06) e *Field of View* 60. Com esses valores, a arena inteira cabe numa tela 16:9.

**Input System:** o pacote `com.unity.inputsystem` e o asset de ações já vêm no template. Em *Edit > Project Settings > Player > Active Input Handling*, o projeto usa só o Input System novo. Para ver ou mudar os botões, dê dois cliques em `InputSystem_Actions` e abra o mapa **Player > Move**: ele tem os bindings *WASD*, setas e *Left Stick* do gamepad. A ação **Player > Investida** (tipo *Button*) tem *Space* (esquema Keyboard&Mouse) e *Right Trigger* do gamepad (esquema Gamepad).

## Visual do jogador (montrengo)
**Como funciona no jogo:** o jogador é desenhado como um monstro de carne de cerca de 2 m de largura e 1,6 m de altura, com olhos em antenas e a boca virada para a frente. Parado, ele pulsa devagar (±4%, um pulso por segundo). Andando, estica e achata no ritmo do passo (até 10%, um ciclo a cada 0,4 s na velocidade base) e se inclina até 12° na direção do movimento. O efeito de andar cresce com a velocidade e o pulso parado some na mesma proporção. Durante a investida, ele estica 30% para a frente e afina na largura e na altura, mantendo o volume; o esticamento entra e sai suavizado, então chega perto de 30% no fim do avanço e volta ao normal logo depois. Só o desenho muda: colisão, movimento e coleta continuam os da cápsula (`Character Controller` de 2 m de altura e 0,5 m de raio).

**Código** (`Assets/Scripts/Animacao/`):
- `CalculadoraDeDeformacao` (C# puro, testada em EditMode): guarda as fases do pulso e do passo e a velocidade suavizada. `Atualizar(velocidadeLocal, deltaTempo, parametros)` recebe a velocidade no referencial do corpo (x para a direita, y para a frente) e devolve uma `PoseDeDeformacao` (multiplicador de escala e rotação de inclinação). `Reiniciar()` volta ao repouso.
  - Intensidade de andar = velocidade suavizada ÷ velocidade de referência, limitada a 1. O pulso parado é multiplicado por `1 - intensidade` e o esticar e achatar por `intensidade`.
  - O pulso parado muda os três eixos por igual. O esticar e achatar preserva o volume: altura × `f`, largura e profundidade × `1/√f`.
  - O passo avança no ritmo da intensidade, então fica mais lento quando o personagem anda devagar e para quando ele para.
  - A inclinação gira o topo para a direção do movimento (`InclinacaoMaxima × intensidade`).
  - A suavização da velocidade é exponencial (`TempoDeResposta`) e não depende da taxa de quadros.
  - Investida: `Atualizar(..., emInvestida)` suaviza um fator de 0 a 1 com o mesmo `TempoDeResposta`. Com `c = 1 + EstiramentoDaInvestida × fator`, a escala é multiplicada por `c` na profundidade (Z local, a frente) e por `1/√c` na largura e na altura.
- `ParametrosDeDeformacao` (struct): os valores acima, validados no construtor (`ArgumentException` fora da faixa).
- `DeformacaoProcedural` (`MonoBehaviour`, no `LateUpdate`): mede a velocidade pela mudança de posição do objeto, no referencial do pai (o `Jogador`), e aplica a pose sobre a escala e a rotação originais. Não depende do `MovimentoDoJogador`: funciona com qualquer forma de movimento e com modificadores de velocidade. Para o esticamento da investida, procura um `InvestidaDoJogador` no pai (opcional; sem ele, não estica). O campo *Estiramento Da Investida* vale 0,3. Os campos podem ser ajustados no Inspector durante o Play.

**Prefab `Assets/Prefabs/VisualMontrengo.prefab`:** a raiz tem a `DeformacaoProcedural` e o pivô na base do corpo, para a deformação não afundar o modelo no chão. O filho `Modelo` é o FBX, com escala 0,429 (maior dimensão de 2 m), centralizado na horizontal e com a base em y = 0. A frente do modelo (a boca) é o +Z do FBX, a mesma frente do `Jogador`.

**Importação do modelo** (`Assets/Terceiros/JoppeMonnens/CronenbergMonster/`):
- FBX com dois materiais: `Material.002` (corpo) e `Bone.002` (dentes), remapeados no *Model Importer* (aba *Materials*) para `Montrengo_Corpo` e `Montrengo_Osso` (URP Lit com *Base Map*, *Normal Map* e *Metallic Map*).
- Texturas com *Max Size* 1024; as `_Normal` como *Normal map*; as `_MetallicSmoothness` sem sRGB. O URP Lit lê a suavidade do alfa do mapa metálico, então os mapas originais Metallic e Roughness foram combinados num só (metálico no RGB, `1 - rugosidade` no alfa). O corpo é bem liso (rugosidade média 0,24, aparência molhada) e os dentes, foscos (0,9). Os mapas Displacement do pacote não são usados.

## Cristais coletáveis
**Como funciona no jogo:** os itens coletáveis são comidas (carne crua, peixe, coxa e xícara de café, do Food Kit da Kenney) de cerca de 0,8 m que giram e flutuam. Ao ser coletado, o item some na hora. Na Arena há 6 itens: carne no centro, onde o jogador nasce e por isso é coletada logo no início, peixe nos cantos noroeste e sudeste, coxa nos cantos nordeste e sudoeste, em (±6, ±6), e o café em (0, 6), ao norte do centro. A posição do café é provisória, para teste. O prefab base `Cristal` (losango ciano) continua no projeto. A coleta ainda não tem som nem contagem na tela.

**Efeitos das comidas:**
| Comida | Definição | Efeito |
|---|---|---|
| Coxa | `Coxa` (`coxa`) | **Velocidade:** +30% por 5 s; quando esse efeito acaba, +0,3 m/s permanente. Pegar outra coxa durante o efeito soma mais 30% e reinicia os 5 s, e os +0,3 m/s de cada coxa entram juntos no fim. Na velocidade base (3,5 m/s): 4,55 m/s por 5 s e depois 3,8 m/s; duas coxas seguidas: 5,6 m/s e depois 4,1 m/s. |
| Café | `Cafe` (`cafe`) | **Libera a investida** até o fim da partida (ver "Investida"). Pegar outro café não muda nada. |
| Carne, peixe | `CristalBase` (`cristal-base`) | Nenhum (só contam). |

Os valores da coxa ficam no asset `Assets/Dados/Cristais/Efeitos/VelocidadeCoxa` e podem ser ajustados no Inspector.

**Visuais por Prefab Variant:** cada comida é uma *Prefab Variant* de `Cristal` (`Comida_Carne`, `Comida_Peixe`, `Comida_Coxa`). A variante herda tudo do prefab base: trigger, `CristalColetavel` com a definição `CristalBase` e `GiroEFlutuacao`. Ela muda só o visual: em `Visual`, *Scale* (1, 1, 1), a `Gema` desativada e um filho `Comida` com o modelo, escalado para que a maior dimensão tenha 0,8 m e centralizado no `Visual`. Mudanças de lógica no prefab base valem para todas as comidas. Para criar uma variante na Editor, clique com o botão direito em `Cristal.prefab` e escolha *Create > Prefab Variant*. Depois, edite o `Visual` da variante.

**Código** (`Assets/Scripts/Cristais/`):

*Lógica pura* (testada em EditMode):
- `EstadoDeColeta`: a regra de coleta. Só um coletor válido coleta, e só uma vez, mesmo que a física avise duas entradas no trigger no mesmo quadro.
- `ContadorDeColeta`: total de cristais coletados e quantidade por tipo (identificador da definição). Tem `Zerar()`.
- `CalculadoraDeFlutuacao`: altura (senoide) e ângulo de giro em função do tempo.

*Dados:*
- `DefinicaoCristal` (ScriptableObject): `Identificador` (único e estável, usado na contagem), `NomeExibicao` e uma lista de `Efeitos`.
- `EfeitoDeCristal` (ScriptableObject abstrato): **ponto de extensão** para os efeitos (velocidade, ímã, investida). Cada efeito é uma subclasse com `Aplicar(ColetorDoJogador coletor)`, criada como asset e colocada na lista de `Efeitos` de uma definição. A lista da `CristalBase` está vazia.
- `EfeitoDeVelocidade` (`EfeitoDeCristal`, menu *Create > Coletor de Cristais > Efeitos > Velocidade*): campos da parte temporária (*Tipo Temporario*, *Valor Temporario*, *Duracao*, *Grupo*) e da permanente (*Tipo Permanente*, *Valor Permanente*, *Momento*). Monta um `BonusDeAtributo` e o aplica em `MovimentoDoJogador.Velocidade` do jogador que coletou, com o próprio asset como origem. Escreve a nova velocidade no Console. Sem `MovimentoDoJogador` no coletor, registra um erro e não aplica nada. Valores inválidos no asset (ex.: parte temporária sem grupo) geram um aviso no Inspector (`OnValidate`) e, na coleta, um erro no Console, sem aplicar o efeito. `AplicarEm(atributo)` aplica o bônus num atributo qualquer e devolve se aplicou.
- `EfeitoLiberarInvestida` (`EfeitoDeCristal`, menu *Create > Coletor de Cristais > Efeitos > Liberar Investida*): chama `InvestidaDoJogador.Liberar()` no jogador que coletou e escreve "investida liberada" no Console. Sem `InvestidaDoJogador` no coletor, registra um erro e não faz nada.

*Componentes* (finos):
- `CristalColetavel`: no `OnTriggerEnter`, procura um `ColetorDoJogador` em quem entrou. Se a regra deixar, registra a coleta e destrói o cristal.
- `ColetorDoJogador` (no `Jogador`): guarda o `ContadorDeColeta` (`Contador`), escreve "Cristal coletado: …" no Console e aplica os efeitos da definição, em ordem. Se um efeito lançar exceção, ela vai para o Console (`Debug.LogException`) e os outros efeitos são aplicados mesmo assim: a coleta é contada e o item some. É por ele que outras partes do jogo (ex.: interface) leem a contagem.
- `GiroEFlutuacao`: anima um objeto, por padrão a 90°/s, 0,15 m de amplitude e período de 2 s.

**Decisões técnicas:**
- O jogador é reconhecido pelo componente `ColetorDoJogador`, não pela tag. Assim, só quem tem o componente coleta.
- O trigger fica na raiz do prefab e a animação fica no filho `Visual`. Assim o trigger não se mexe com a flutuação.
- O `CharacterController` do jogador dispara `OnTriggerEnter` sem precisar de `Rigidbody`.
- Por enquanto todos os cristais giram em sincronia, porque a animação usa o mesmo `Time.time` para todos.

**Como foi montado na Editor** (para reproduzir):
1. **ScriptableObject (definição):** na janela Project, clique com o botão direito em `Assets/Dados/Cristais` e escolha *Create > Coletor de Cristais > Definição de Cristal*. Isso cria um asset de dados (não é um objeto da cena). No Inspector, preencha *Identificador* (`cristal-base`) e *Nome Exibicao* (`Cristal Base`). Os cristais apontam para esse asset, então mudar o asset muda todos.
2. **Material:** `Assets/Materiais/Cristal` (URP Lit), *Base Map* ciano (0,1; 0,9; 1), *Smoothness* 0,85, *Emission* ligada com a mesma cor a 60%.
3. **Prefab:**
   - Crie um GameObject vazio `Cristal` e adicione:
     - `Sphere Collider` com *Is Trigger* marcado e *Radius* 0,5. Um *trigger* não bloqueia ninguém: ele só avisa (`OnTriggerEnter`) quando outro collider entra.
     - `Cristal Coletavel`, com o campo *Definicao* apontando para `CristalBase`.
   - Filho `Visual` (vazio), com *Scale* (1; 1,35; 1) para esticar na vertical, e o componente `Giro E Flutuacao`.
   - Filho de `Visual`: `Gema`, um cubo com *Rotation* (45; 0; 35,264), que deixa o cubo apoiado numa ponta, como um losango. *Scale* 0,5 e material `Cristal`. Remova o `Box Collider`: quem detecta o jogador é só o trigger da raiz.
   - Arraste `Cristal` da Hierarchy para `Assets/Prefabs` para virar prefab. Depois edite pelo prefab (dois cliques nele): as instâncias na cena recebem as mudanças.
4. **Jogador:** adicione o componente `Coletor Do Jogador` ao `Jogador`.
5. **Cena:** um GameObject vazio `Cristais` com 5 instâncias do prefab (arraste de `Assets/Prefabs` para a cena), em y = 0,8: centro (0; 0,8; 0) e cantos (±6; 0,8; ±6).

6. **Coxa com velocidade:** em `Assets/Dados/Cristais/Efeitos`, *Create > Coletor de Cristais > Efeitos > Velocidade* (`VelocidadeCoxa`): *Tipo Temporario* `PercentualAditivo`, *Valor Temporario* 0,3, *Duracao* 5, *Grupo* `coxa-velocidade`, *Tipo Permanente* `Plano`, *Valor Permanente* 0,3, *Momento* `AoFimDoTemporario`. Crie a definição `Assets/Dados/Cristais/Coxa` (*Identificador* `coxa`, *Nome Exibicao* `Coxa`) com `VelocidadeCoxa` na lista de *Efeitos*. Na variante `Comida_Coxa`, aponte o *Definicao* do `Cristal Coletavel` para `Coxa`.

Ao apertar Play, o item do centro é coletado logo de cara e o Console mostra "Cristal coletado: Cristal Base (cristal-base). Total: 1.". Ande até os outros para coletá-los. Ao pegar uma coxa, o Console mostra "VelocidadeCoxa: velocidade agora 4,55 m/s.".

7. **Café que libera a investida:** em `Assets/Dados/Cristais/Efeitos`, *Create > Coletor de Cristais > Efeitos > Liberar Investida* (`LiberarInvestida`, sem campos). Crie a definição `Assets/Dados/Cristais/Cafe` (*Identificador* `cafe`, *Nome Exibicao* `Café`) com `LiberarInvestida` na lista de *Efeitos*. Crie a variante `Comida_Cafe` como as outras comidas, com o modelo `cup-coffee` do Food Kit, e aponte o *Definicao* para `Cafe`. Na cena, coloque uma instância em `Cristais`, na posição (0; 0,8; 6).

## Investida
**Como funciona no jogo:** a investida é a primeira habilidade ativa. Ela começa bloqueada e é liberada ao pegar o café. Com o botão (Espaço ou gatilho direito), o personagem avança rápido em linha reta:
- **Velocidade:** 5 × a velocidade de movimento do momento, **durante 0,25 s**. Os bônus de velocidade (ex.: a coxa) aumentam a investida.
- **Distância:** `5 × velocidade × 0,25 s`. Na velocidade base de 3,5 m/s, **4,375 m**. Com a coxa ativa (4,55 m/s), 5,69 m; depois do fim do efeito da coxa (3,8 m/s), 4,75 m.
- **Direção:** a do controle no momento do botão; sem direção no controle, para onde o personagem está virado. O personagem vira na hora para a direção da investida, e ela não muda no meio.
- **Paredes:** a investida usa a mesma física do andar (`CharacterController`), então para na parede, sem atravessar. Na arena, uma investida para leste que começa em x = 7,5 (a 2,5 m da face da mureta) para em x = 9,42, encostada na mureta, depois de 1,92 m em vez de 4,375 m.
- **Recarga:** 5 s, contados **desde o início** da investida (0,25 s de avanço + 4,75 s de espera). Durante a recarga, o botão não faz nada.
- Durante o avanço, o movimento normal fica suspenso. Os efeitos temporários (ex.: os 5 s da coxa) continuam contando.
- Ainda não há indicação na tela da recarga nem de que a investida foi liberada.

Os valores ficam no asset `Assets/Dados/Habilidades/Investida` e podem ser ajustados no Inspector.

**Código** (`Assets/Scripts/Habilidades/`):
- `ParametrosDeInvestida` (struct, C# puro): `MultiplicadorDeVelocidade` (> 0), `Duracao` (> 0) e `Recarga` (≥ 0), validados no construtor (`ArgumentException`). `Distancia(velocidadeDeMovimento)` = velocidade × multiplicador × duração (velocidade negativa conta como zero).
- `Investida` (C# puro, testada em EditMode): o estado da habilidade.
  - `Liberar()`, `Liberada`, `EmAndamento`, `RecargaRestante`, `Pronta` e `Velocidade` (zero fora de uma investida).
  - `TentarIniciar(direcao, velocidadeDeMovimento, parametros)`: só começa se estiver pronta, com direção horizontal e velocidade maior que zero. Assim, um personagem parado por um modificador não gasta a recarga. A direção é achatada no chão (y ignorado) e normalizada. A velocidade fica fixa até o fim.
  - `Atualizar(deltaTempo)`: avança a investida e a recarga e devolve o deslocamento do quadro. O último quadro usa só o tempo que faltava, então a distância total é exata, em qualquer taxa de quadros.
  - `CalcularDirecao(entrada, frente, entradaMinima)` (estático): a entrada do controle, se for maior que o mínimo; senão, a frente do personagem no chão (+Z se a frente for vertical).
  - Valores não finitos ou tempo negativo geram `ArgumentException`.
- `DefinicaoInvestida` (ScriptableObject, menu *Create > Coletor de Cristais > Definição de Investida*): *Multiplicador De Velocidade* 5, *Duracao* 0,25 e *Recarga* 5. `Parametros` monta o `ParametrosDeInvestida`.
- `InvestidaDoJogador` (`MonoBehaviour`, no `Jogador`, exige `CharacterController` e `MovimentoDoJogador`): lê o botão (*Acao Investida*, `Player/Investida`), chama `TentarIniciar()` e aplica o deslocamento com `CharacterController.Move`. Liga `MovimentoDoJogador.Suspenso` no início da investida e desliga no fim (ou ao ser desativado), sem escrever na flag nos outros quadros. A direção vem de `MovimentoDoJogador.LerEntrada()`, lida no quadro do botão. `TentarIniciar()` é público para testes pela CLI. `Liberar()` é chamado pelo efeito do café, e `EmInvestida` é lido pela `DeformacaoProcedural`.

**Decisões técnicas:**
- `InvestidaDoJogador` roda antes do `MovimentoDoJogador` (`[DefaultExecutionOrder(-10)]`). Assim, no quadro em que a investida começa, o movimento normal já fica suspenso e os dois não movem o personagem juntos.
- A liberação fica no componente, não num atributo: é um sim ou não que vale até o fim da partida.

**Como foi montado na Editor:**
1. **Ação:** em `InputSystem_Actions`, no mapa *Player*, adicione a ação `Investida` (*Action Type* *Button*), com os bindings *Space [Keyboard]* (esquema Keyboard&Mouse) e *Right Trigger [Gamepad]* (esquema Gamepad). Salve o asset.
2. **Valores:** em `Assets/Dados/Habilidades`, *Create > Coletor de Cristais > Definição de Investida* (`Investida`), com os valores padrão.
3. **Jogador:** adicione `Investida Do Jogador` ao `Jogador`. Em *Acao Investida*, escolha `Player/Investida`; em *Definicao*, o asset `Investida`.
4. **Item:** o café (passo 7 de "Cristais coletáveis").

## Convenções
- Código novo vai em `Assets/Scripts/`, dentro do namespace `ColetorDeCristais` (uma subpasta e um sub-namespace por sistema).
- Toda lógica nova tem teste EditMode em `Assets/Testes/EditMode/`. Lógica testável fica em classes C# puras, separada dos `MonoBehaviour`, para não depender de Play Mode.
- Cada `MonoBehaviour` fica sozinho em um `.cs` com o nome da classe.
- Uma branch por tarefa (ex.: `movimento-jogador`). Os commits são feitos pelo desenvolvedor.
- Nada pessoal ou externo ao jogo entra no repositório (dados pessoais, credenciais, cronogramas, datas, notas). Esse conteúdo fica em arquivos ignorados, como o `CLAUDE.local.md`.
- Cada entrega que muda o que o jogador vê ou faz ganha uma entrada no [`ATUALIZACOES.md`](ATUALIZACOES.md), com imagem ou animação quando ajudar.
- Regras para agentes: ver `CLAUDE.md`.

## Créditos e licenças
Assets de terceiros:
- Food Kit 2.0 e Mini Shop 2.1, de **Kenney** ([www.kenney.nl](https://www.kenney.nl)), sob licença [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).
- "[Cronenberg Monster Creature](https://sketchfab.com/3d-models/cronenberg-monster-creature-joppe-monnens-b469c9fe16f84845941bffaae9a06b00)", de **Joppe Monnens**, sob licença [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/), com modificações. Essa licença **exige crédito** (também na tela de créditos do jogo).

A lista completa, com origem e uso de cada pacote, está em [`CREDITOS.md`](CREDITOS.md). Todo asset de terceiros novo entra nessa lista.

## Versionamento
- `.gitignore` de Unity na raiz. A seção "Arquivos pessoais e locais" ignora `CLAUDE.local.md`, `*.local.md`, as configurações locais do Claude Code (`.claude/settings.local.json`), as pastas `Notas/` e `Rascunhos/`, os arquivos `*.notas.md`, `.env` e os arquivos de credenciais e chaves.
- `.gitattributes` com Git LFS para texturas, áudio, modelos 3D, vídeo, fontes e outros binários, e merge `unityyamlmerge` para os YAML da Unity (para usar o UnityYAMLMerge como ferramenta de merge, configure-o no Git local; sem isso o Git faz merge de texto normal).
