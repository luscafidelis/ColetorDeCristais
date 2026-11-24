# Coletor de Cristais

Jogo 3D em Unity sobre coletar cristais numa arena.

## O jogo
Arena 3D com câmera fixa elevada. O jogador anda pela arena e coleta cristais. A partida é em ondas: o objetivo é sobreviver até o tempo da onda acabar; aí vem o chefe da onda, que precisa ser derrotado para passar para a próxima. Cada arena tem 3 ondas, e a última termina no chefe da arena. Cada cristal é um item com um efeito:
- **Atributo:** aumenta a velocidade.
- **Habilidade passiva:** ímã, que puxa cristais próximos.
- **Habilidade ativa:** investida, com tempo de recarga.

O jogo começa no **minimercado** (arena 0), que serve de tutorial: corredores entre prateleiras, carrinhos de compras que empurram o personagem e um vão que só se atravessa com a investida. Cair no vão é fim de jogo.

Plataforma: Windows. Sem monetização.

> Estado atual: a arena com o personagem que anda (teclado ou gamepad), desenhado como um monstro de carne (o "montrengo", modelo de Joppe Monnens) que pulsa parado e estica, achata e se inclina ao andar, e a câmera fixa elevada; o sistema de atributos com modificadores permanentes e temporários (a velocidade do personagem já é um atributo); os itens coletáveis com visual de comida (carne, peixe e coxa), que somem ao ser coletados e são contados, numa arena de piso e paredes de supermercado, com modelos da Kenney. A coxa acelera o personagem (+30% por 5 s e, ao fim, +0,3 m/s permanente); o café libera a investida, um avanço rápido com recarga de 5 s (4,375 m na velocidade base), durante o qual o montrengo estica para a frente; a rosquinha libera o ímã, que puxa os itens próximos até o personagem (os cristais de experiência, pequenos losangos ciano, de mais longe e mais rápido); carne e peixe ainda não têm efeito, e os cristais de experiência ainda não dão experiência. A partida tem 3 ondas de 120 s, com cronômetro regressivo no topo da tela; a cada nova onda entram itens novos em posições sorteadas. Os chefes, os inimigos e a vida ainda não existem: na vez de um chefe, qualquer tecla conta como vitória sobre ele, e a única forma de perder é cair no vão do mercado. O mercado (arena 0) tem 1 onda, 3 corredores com carrinhos que empurram o personagem, um vão de 3 m que só se atravessa com a investida e, do outro lado, uma área bônus com a rosquinha e 10 cristais de experiência; uma investida contra uma prateleira derruba um produto (ainda sem efeito). A planta do mercado é provisória. Contagem, recarga e efeitos na tela e sons ainda não foram implementados.

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
Na Editor, abra `Assets/Scenes/Mercado.unity` (a arena 0, primeira cena do build) ou `Assets/Scenes/Arena.unity` e aperte Play. Clique na janela Game para ela receber o teclado.

**Controles:**
| Ação | Teclado | Gamepad |
|---|---|---|
| Andar | WASD ou setas | Analógico esquerdo |
| Investida (depois de pegar o café) | Espaço | Gatilho direito (RT/R2) |
| Continuar (na vez do chefe) ou recomeçar (no fim) | Qualquer tecla | Qualquer botão |

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
| `Assets/Scripts/` | Código do jogo. Assembly `ColetorDeCristais` (`ColetorDeCristais.asmdef`), namespace `ColetorDeCristais`. Referencia `Unity.InputSystem` e `UnityEngine.UI` (uGUI, usado na interface mínima da partida) |
| `Assets/Scripts/InformacoesDoProjeto.cs` | Classe mínima (nome do jogo), criada com o projeto para validar a estrutura de testes |
| `Assets/Scripts/Atributos/` | Sistema de atributos em C# puro (namespace `ColetorDeCristais.Atributos`): `Atributo`, `Modificador`, `TipoModificador`, `BonusDeAtributo` e `MomentoDoPermanente`. Ver "Atributos e modificadores" abaixo |
| `Assets/Scripts/Movimento/` | Movimento do personagem (namespace `ColetorDeCristais.Movimento`): `CalculadoraDeMovimento` (cálculos puros) e `MovimentoDoJogador` (`MonoBehaviour`). Ver "Movimento do personagem" abaixo |
| `Assets/Scripts/Habilidades/` | Habilidades do jogador (namespace `ColetorDeCristais.Habilidades`). Investida: `Investida` e `ParametrosDeInvestida` (C# puro), `DefinicaoInvestida` (ScriptableObject) e `InvestidaDoJogador` (`MonoBehaviour`). Ímã: `Ima` e `ParametrosDeIma` (C# puro), `DefinicaoIma` (ScriptableObject) e `ImaDoJogador` (`MonoBehaviour`). Ver "Investida" e "Ímã" abaixo |
| `Assets/Scripts/Partida/` | Partida em ondas (namespace `ColetorDeCristais.Partida`): `CicloDaPartida`, `ParametrosDaPartida`, `EstadoDaPartida`, `ComposicaoDaOnda` e `GeradorDePosicoes` (C# puro), `DefinicaoPartida` (ScriptableObject), `ControladorDaPartida` e `MensagensDaPartida` (`MonoBehaviour`). Ver "Partida, ondas e chefes" abaixo |
| `Assets/Scripts/Nivel/` | Elementos de nível (namespace `ColetorDeCristais.Nivel`): `Retangulo`, `VaiEVem`, `Empurrao`, `RegraDeQueda` e `EstadoDaPrateleira` (C# puro), `DefinicaoNivel` (ScriptableObject), `ControladorDoNivel`, `CarrinhoVaiEVem`, `PrateleiraSecreta` e `ImpactoDaInvestida` (`MonoBehaviour`). Ver "Mercado (arena 0)" abaixo |
| `Assets/Testes/EditMode/` | Testes EditMode. Assembly `ColetorDeCristais.Testes.EditMode`, namespace `ColetorDeCristais.Testes`; referencia `ColetorDeCristais` e só compila na Editor. Subpastas espelham as de `Assets/Scripts/` (ex.: `Atributos/`, `Movimento/`) |
| `Assets/Scenes/Mercado.unity` | Arena 0, o tutorial (primeira cena do Build Profile): o minimercado, jogador, câmera, luz. Ver "Mercado (arena 0)" |
| `Assets/Scenes/Arena.unity` | A arena de 20×20 m (segunda cena do Build Profile): arena, jogador, câmera, luz |
| `Assets/Scenes/SampleScene.unity` | Cena de exemplo do template, fora do build; serviu de base para a Arena |
| `Assets/Scripts/Cristais/` | Cristais coletáveis (namespace `ColetorDeCristais.Cristais`): lógica pura (`EstadoDeColeta`, `ContadorDeColeta`, `CalculadoraDeFlutuacao`), dados (`DefinicaoCristal`, `EfeitoDeCristal`) e componentes (`CristalColetavel`, `ColetorDoJogador`, `GiroEFlutuacao`). Ver "Cristais coletáveis" abaixo |
| `Assets/Scripts/Animacao/` | Animação procedural (namespace `ColetorDeCristais.Animacao`): `CalculadoraDeDeformacao`, `ParametrosDeDeformacao` e `PoseDeDeformacao` (C# puro) e `DeformacaoProcedural` (`MonoBehaviour`). Ver "Visual do jogador (montrengo)" abaixo |
| `Assets/Scripts/Ferramentas/` | Ferramentas de desenvolvimento (namespace `ColetorDeCristais.Ferramentas`): `GravadorDeQuadros`, só da Editor (`#if UNITY_EDITOR`, fica fora do build), grava quadros da câmera do jogo seguindo um `RoteiroDeEntrada` (C# puro, testado). Uso no `ATUALIZACOES.md` |
| `Assets/Prefabs/Cristal.prefab` | Prefab base do item coletável: trigger, coleta e visual animado (losango ciano) |
| `Assets/Prefabs/Comida_Carne.prefab`, `Comida_Peixe.prefab`, `Comida_Coxa.prefab`, `Comida_Cafe.prefab`, `Comida_Rosquinha.prefab` | Prefab Variants de `Cristal` com modelos de comida do Food Kit (ver "Cristais coletáveis") |
| `Assets/Prefabs/Cristal_Experiencia.prefab` | Prefab Variant de `Cristal` para os cristais de experiência: definição `Experiencia` e visual com 1/4 do tamanho (ver "Cristais coletáveis") |
| `Assets/Prefabs/VisualMontrengo.prefab` | Visual do jogador: o modelo do montrengo com a `DeformacaoProcedural`, sem colisão (ver "Visual do jogador (montrengo)") |
| `Assets/Prefabs/ArenaVisualKenney.prefab` | Visual da Arena: piso e paredes do Mini Shop, sem colisão (ver "Movimento do personagem") |
| `Assets/Prefabs/MercadoVisualKenney.prefab` | Visual do mercado: o mesmo da Arena, sem as placas de piso do vão |
| `Assets/Prefabs/Produto_Lata.prefab`, `Produto_Refrigerante.prefab`, `Produto_Garrafa.prefab`, `Produto_Biscoito.prefab` | Prefab Variants de `Cristal` com modelos do Food Kit: os produtos que caem das prateleiras do mercado |
| `Assets/Terceiros/Kenney/` | Assets de terceiros da Kenney (CC0): `FoodKit/` e `MiniShop/`, em FBX com textura `Textures/colormap.png` e o `License.txt` de cada pacote. Créditos em [`CREDITOS.md`](CREDITOS.md) |
| `Assets/Terceiros/JoppeMonnens/CronenbergMonster/` | Modelo do montrengo (CC BY 4.0, crédito obrigatório): `CronenBergFinal.fbx`, `Texturas/` (cor e normal originais e o mapa metálico+suavidade combinado, importados em 1K), `Materiais/` (`Montrengo_Corpo`, `Montrengo_Osso`, URP Lit) e `LICENSE.txt` com a atribuição e as modificações |
| `Assets/Dados/Cristais/` | Assets `DefinicaoCristal`: `CristalBase` (carne e peixe, sem efeito), `Coxa`, `Cafe`, `Rosquinha`, `Experiencia` (cristais de experiência) e os produtos das prateleiras, sem efeito: `Lata`, `Refrigerante`, `Garrafa` e `Biscoito`. Em `Efeitos/`, os assets de efeito (`VelocidadeCoxa`, `LiberarInvestida`, `LiberarIma`) |
| `Assets/Dados/Habilidades/` | Assets de valores das habilidades: `Investida` (`DefinicaoInvestida`) e `Ima` (`DefinicaoIma`) |
| `Assets/Dados/Partida/` | Assets `DefinicaoPartida`: `Partida` (a Arena, 3 ondas) e `PartidaTutorial` (o mercado, 1 onda): cronômetro, ondas e itens novos de cada onda |
| `Assets/Dados/Nivel/` | Asset `Mercado` (`DefinicaoNivel`): o vão, as áreas sem itens novos, a velocidade dos carrinhos e os produtos das prateleiras |
| `Assets/Materiais/` | Materiais URP Lit: `Chao`, `Mureta`, `Jogador`, `NarizJogador`, `Cristal` e `Vao` (quase preto, o fosso do vão) |
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
**Como funciona no jogo:** os itens coletáveis são comidas (carne crua, peixe, coxa, xícara de café e rosquinha, do Food Kit da Kenney) de cerca de 0,8 m e cristais de experiência (um losango ciano pequeno, com 1/4 do tamanho do prefab base `Cristal`, cerca de 0,2 m de altura), que giram e flutuam. O tamanho menor vale só para o desenho: a área de coleta (o trigger) é a mesma dos outros itens. Ao ser coletado, o item some na hora. Na Arena há 11 itens:
- carne no centro, onde o jogador nasce e por isso é coletada logo no início;
- peixe nos cantos noroeste e sudeste, e coxa nos cantos nordeste e sudoeste, em (±6, ±6);
- café em (0, 6) e rosquinha em (0, -4);
- quatro cristais de experiência, em (±3, 3) e (±2,5, -7).

As posições do café, da rosquinha e dos cristais de experiência são provisórias, para teste. A coleta ainda não tem som nem contagem na tela.

**Efeitos dos itens** (a coluna "Ímã" é o nível de atração do item, ver "Ímã"):
| Item | Definição | Efeito | Ímã |
|---|---|---|---|
| Coxa | `Coxa` (`coxa`) | **Velocidade:** +30% por 5 s; quando esse efeito acaba, +0,3 m/s permanente. Pegar outra coxa durante o efeito soma mais 30% e reinicia os 5 s, e os +0,3 m/s de cada coxa entram juntos no fim. Na velocidade base (3,5 m/s): 4,55 m/s por 5 s e depois 3,8 m/s; duas coxas seguidas: 5,6 m/s e depois 4,1 m/s. | Reduzida |
| Café | `Cafe` (`cafe`) | **Libera a investida** até o fim da partida (ver "Investida"). Pegar outro café não muda nada. | Nenhuma |
| Rosquinha | `Rosquinha` (`rosquinha`) | **Libera o ímã** até o fim da partida (ver "Ímã"). Pegar outra rosquinha não muda nada. | Nenhuma |
| Carne, peixe | `CristalBase` (`cristal-base`) | Nenhum (só contam). | Reduzida |
| Lata, refrigerante, garrafa, biscoito | `Lata`, `Refrigerante`, `Garrafa`, `Biscoito` | Nenhum por enquanto (só contam). Caem das prateleiras do mercado (ver "Mercado (arena 0)"). | Reduzida |
| Cristal de experiência | `Experiencia` (`experiencia`) | Nenhum por enquanto (só conta). | Completa |

Os valores da coxa ficam no asset `Assets/Dados/Cristais/Efeitos/VelocidadeCoxa` e podem ser ajustados no Inspector.

**Visuais por Prefab Variant:** cada comida é uma *Prefab Variant* de `Cristal` (`Comida_Carne`, `Comida_Peixe`, `Comida_Coxa`). A variante herda tudo do prefab base: trigger, `CristalColetavel` com a definição `CristalBase` e `GiroEFlutuacao`. Ela muda só o visual: em `Visual`, *Scale* (1, 1, 1), a `Gema` desativada e um filho `Comida` com o modelo, escalado para que a maior dimensão tenha 0,8 m e centralizado no `Visual`. Mudanças de lógica no prefab base valem para todas as comidas. Para criar uma variante na Editor, clique com o botão direito em `Cristal.prefab` e escolha *Create > Prefab Variant*. Depois, edite o `Visual` da variante.

**Código** (`Assets/Scripts/Cristais/`):

*Lógica pura* (testada em EditMode):
- `EstadoDeColeta`: a regra de coleta. Só um coletor válido coleta, e só uma vez, mesmo que a física avise duas entradas no trigger no mesmo quadro.
- `ContadorDeColeta`: total de cristais coletados e quantidade por tipo (identificador da definição). Tem `Zerar()`.
- `CalculadoraDeFlutuacao`: altura (senoide) e ângulo de giro em função do tempo.

*Dados:*
- `DefinicaoCristal` (ScriptableObject): `Identificador` (único e estável, usado na contagem), `NomeExibicao`, uma lista de `Efeitos` e `AtracaoDoIma` (`Completa`, o padrão, `Reduzida` ou `Nenhuma`).
- `EfeitoDeCristal` (ScriptableObject abstrato): **ponto de extensão** para os efeitos (velocidade, ímã, investida). Cada efeito é uma subclasse com `Aplicar(ColetorDoJogador coletor)`, criada como asset e colocada na lista de `Efeitos` de uma definição. A lista da `CristalBase` está vazia.
- `EfeitoDeVelocidade` (`EfeitoDeCristal`, menu *Create > Coletor de Cristais > Efeitos > Velocidade*): campos da parte temporária (*Tipo Temporario*, *Valor Temporario*, *Duracao*, *Grupo*) e da permanente (*Tipo Permanente*, *Valor Permanente*, *Momento*). Monta um `BonusDeAtributo` e o aplica em `MovimentoDoJogador.Velocidade` do jogador que coletou, com o próprio asset como origem. Escreve a nova velocidade no Console. Sem `MovimentoDoJogador` no coletor, registra um erro e não aplica nada. Valores inválidos no asset (ex.: parte temporária sem grupo) geram um aviso no Inspector (`OnValidate`) e, na coleta, um erro no Console, sem aplicar o efeito. `AplicarEm(atributo)` aplica o bônus num atributo qualquer e devolve se aplicou.
- `EfeitoLiberarInvestida` (`EfeitoDeCristal`, menu *Create > Coletor de Cristais > Efeitos > Liberar Investida*): chama `InvestidaDoJogador.Liberar()` no jogador que coletou e escreve "investida liberada" no Console. Sem `InvestidaDoJogador` no coletor, registra um erro e não faz nada.
- `EfeitoLiberarIma` (`EfeitoDeCristal`, menu *Create > Coletor de Cristais > Efeitos > Liberar Ímã*): o mesmo para o ímã, com `ImaDoJogador.Liberar()` ("ímã liberado" no Console).

*Componentes* (finos):
- `CristalColetavel`: no `OnTriggerEnter` e no `OnTriggerStay`, procura um `ColetorDoJogador` em quem encosta. Se a regra deixar (coletor ligado e item ainda não coletado), registra a coleta e destrói o cristal. O `OnTriggerStay` cobre o item que já encostava no jogador quando a coleta estava desligada: ele é coletado assim que ela volta. `CristalColetavel.Ativos` lista os cristais ativos na cena (cada um entra no `OnEnable` e sai no `OnDisable`); o ímã usa essa lista.
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
8. **Rosquinha que libera o ímã:** como o café, com o efeito `LiberarIma` (*Create > Coletor de Cristais > Efeitos > Liberar Ímã*), a definição `Rosquinha` (*Identificador* `rosquinha`, *Nome Exibicao* `Rosquinha`, *Atracao Do Ima* `Nenhuma`), a variante `Comida_Rosquinha` com o modelo `donut` e uma instância em (0; 0,8; -4).
9. **Cristais de experiência:** a definição `Assets/Dados/Cristais/Experiencia` (*Identificador* `experiencia`, *Nome Exibicao* `Cristal de Experiência`, sem efeitos, *Atracao Do Ima* `Completa`). Crie a variante `Cristal_Experiencia` de `Cristal` (*Create > Prefab Variant*): *Definicao* `Experiencia` e, em `Visual`, *Scale* (0,25; 0,3375; 0,25), 1/4 da escala do base. A `Gema` e o trigger ficam como no base. Na cena, quatro instâncias da variante em `Cristais` (`Cristal_Experiencia_1` a `4`), em (-2,5; 0,8; -7), (2,5; 0,8; -7), (-3; 0,8; 3) e (3; 0,8; 3).
10. **Níveis de atração:** em `CristalBase` e `Coxa`, *Atracao Do Ima* `Reduzida`; em `Cafe`, `Nenhuma`.

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
  - `Cancelar()`: interrompe a investida em andamento e descarta o resto do avanço; a recarga continua. O `InvestidaDoJogador` chama ao ser desligado (ex.: jogador travado no fim da onda), para o avanço não terminar sozinho quando ele volta.
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

## Ímã
**Como funciona no jogo:** o ímã é a primeira habilidade passiva. Ele começa bloqueado e é liberado, até o fim da partida, ao pegar a rosquinha. Depois disso, sem botão, os itens perto do personagem são puxados até ele e coletados normalmente (pelo mesmo trigger de sempre). Cada tipo de item tem um nível de atração (`AtracaoDoIma` na definição):

| Nível | Raio | Velocidade (borda → junto do jogador) | Itens |
|---|---|---|---|
| `Completa` | 3 m | 1,5 → 6 m/s | Cristais de experiência |
| `Reduzida` | 1 m | 0,5 → 2 m/s | Comidas com efeito ou com efeito previsto: carne, peixe, coxa |
| `Nenhuma` | — | — | Itens que liberam habilidades: café, rosquinha |

- **Distância:** medida no chão (a altura não conta), do centro do personagem ao centro do item.
- **Aceleração:** o item começa na borda do raio a 25% da velocidade e acelera em linha reta até a velocidade cheia junto do personagem. O item é coletado quando encosta no personagem, a cerca de 1 m do centro dele. Com o personagem parado, um cristal de experiência que entra pela borda é coletado em cerca de 0,75 s (sem a coleta, chegaria ao centro em cerca de 0,9 s).
- **Nível reduzido:** como a coleta já acontece a cerca de 1 m, o raio de 1 m quase não aparece na prática: o item é coletado praticamente ao entrar no raio.
- **Caminho:** o item anda em linha reta no chão, mantendo a altura. Só é puxado se nada sólido estiver entre ele e o personagem (verificado a cada quadro, na altura do item), então não atravessa paredes nem obstáculos: fica parado até o caminho ficar livre.
- Ainda não há efeito visual do ímã nem indicação na tela de que ele foi liberado.

Os valores ficam no asset `Assets/Dados/Habilidades/Ima` e podem ser ajustados no Inspector.

**Código** (`Assets/Scripts/Habilidades/`):
- `ParametrosDeIma` (struct, C# puro): `Raio` (> 0), `VelocidadeMaxima` (> 0, junto do jogador) e `FracaoNaBorda` (0 a 1; 1 = velocidade constante), validados no construtor (`ArgumentException`).
- `Ima` (C# puro, testado em EditMode):
  - `Liberar()` e `Liberada`.
  - `TentarCalcularPuxao(coletado, atracao, completa, reduzida, item, jogador, deltaTempo, out novaPosicao)`: decide se um item é puxado neste quadro e calcula para onde. Não puxa com o ímã bloqueado, item já coletado, item sem definição (`atracao` nula), nível `Nenhuma` ou fora do raio do nível. O caminho livre (paredes) fica com quem chama.
  - `TentarObterParametros(atracao, completa, reduzida, out parametros)` (estático): escolhe os valores do nível, ou `false` para `Nenhuma`.
  - `DistanciaHorizontal`, `EstaNoAlcance(item, jogador, parametros)` (na borda conta) e `Velocidade(distancia, parametros)`: `vMax × (f + (1 - f) × (1 - d / R))`.
  - `DistanciaApos(distancia, deltaTempo, parametros)`: resolve o movimento de forma exata (`d(t) = c - (c - d0) × e^(k t)`, com `k = vMax × (1 - f) / R` e `c = R / (1 - f)`; com `f = 1`, `d0 - vMax × t`). Por isso o caminho não depende da taxa de quadros, e a distância nunca fica negativa.
  - `CalcularNovaPosicao(item, jogador, parametros, deltaTempo)`: a nova posição do item, em linha reta no chão na direção do jogador, mantendo a altura e sem passar do jogador. Fora do raio, não mexe.
  - Valores não finitos ou tempo negativo geram `ArgumentException`.
- `DefinicaoIma` (ScriptableObject, menu *Create > Coletor de Cristais > Definição de Ímã*): *Raio Completo* 3, *Velocidade Completa* 6, *Raio Reduzido* 1, *Velocidade Reduzida* 2 e *Fracao Na Borda* 0,25. `ParametrosCompletos` e `ParametrosReduzidos` montam os `ParametrosDeIma` de cada nível; `TentarObterParametros(atracao, out parametros)` devolve os do nível pedido (escolha do `Ima`), ou `false` para `Nenhuma`.
- `ImaDoJogador` (`MonoBehaviour`, no `Jogador`): depois de liberado, percorre `CristalColetavel.Ativos` a cada quadro e pergunta a `Ima.TentarCalcularPuxao` se cada cristal é puxado e para onde. Antes de mover, faz um `Physics.Linecast` do cristal até o jogador, na altura do cristal, ignorando triggers: se acertar algo que não seja o próprio jogador, o cristal não se mexe naquele quadro. `Liberar()` é chamado pelo efeito da rosquinha.

**Decisões técnicas:**
- O nível de atração fica na definição do cristal, e os valores de cada nível ficam num asset só. Um tipo novo de item escolhe o nível sem código novo.
- O cristal se move pelo `transform` (o trigger é estático, sem `Rigidbody`). A coleta pelo trigger foi verificada em Play Mode com o personagem parado e o cristal chegando até ele.

**Como foi montado na Editor:**
1. **Valores:** em `Assets/Dados/Habilidades`, *Create > Coletor de Cristais > Definição de Ímã* (`Ima`), com os valores padrão.
2. **Jogador:** adicione `Ima Do Jogador` ao `Jogador` e, em *Definicao*, escolha o asset `Ima`.
3. **Itens:** a rosquinha, os cristais de experiência e os níveis de atração (passos 8 a 10 de "Cristais coletáveis").

## Partida, ondas e chefes
**Como funciona no jogo:**
- **Ondas:** a Arena tem **3 ondas de 120 s**; o mercado (arena 0), 1. O cronômetro é regressivo e aparece no topo da tela, junto com a onda (ex.: "Onda 1/3   1:54"). O objetivo é sobreviver até o tempo acabar.
- **Chefes:** quando o tempo de uma onda acaba, vem o **chefe da onda**; na 3ª onda, no lugar dele, vem o **chefe da arena**. Os chefes ainda não existem: aparece a mensagem "Chefe da onda N!" ou "Chefe da arena!", e **qualquer tecla** (ou botão) conta como chefe derrotado.
- **Próxima onda:** depois do chefe da onda, começa a próxima onda, com o tempo cheio. O jogador fica onde está e mantém as habilidades liberadas e os bônus. Os itens que sobraram ficam na arena, e entram itens novos em posições sorteadas:
  - as mesmas comidas da primeira onda: 1 carne, 2 peixes e 2 coxas;
  - os cristais de experiência dobram a cada onda: 4 na primeira (os da cena), 8 na segunda, 16 na terceira;
  - um item de habilidade, café ou rosquinha, sorteado. Pegar de novo uma habilidade já liberada ainda não muda nada.
- **Posições sorteadas:** dentro da área de -8 a 8 m (2 m das paredes), a pelo menos 1,5 m do jogador, dos itens que sobraram e uns dos outros. Se a arena estiver cheia, entram menos itens (um aviso vai para o Console). Na ordem de prioridade: o item de habilidade, as comidas e por último os cristais. Com esses valores, cabem cerca de 80 itens na arena ao mesmo tempo: numa 3ª onda sem nada coletado antes, entram os 22 itens novos e ficam 46 na arena.
- **Vitória:** derrotar o chefe da arena mostra "Arena concluída!". Ainda não há outra arena.
- **Derrota:** acontece quando o jogador cai num vão (no mercado) e, no futuro, quando ficar sem vida. Aparece "Derrota".
- **Fora de uma onda** (chefe, vitória ou derrota), o jogador fica parado: não anda, não usa a investida, o ímã não puxa e nada é coletado. Uma investida em andamento para ali (a recarga continua), e um item que estiver encostado no personagem é coletado quando a onda seguinte começa. Os efeitos temporários (ex.: os 5 s da coxa) também param de contar.
- **Recomeçar:** depois da vitória ou da derrota, qualquer tecla recomeça a arena do zero (a cena é recarregada).
- Depois de cada mudança de fase, as teclas são ignoradas por 0,5 s, para a mensagem não ser pulada sem querer; a instrução "Aperte qualquer tecla" só aparece depois disso.

Os valores ficam nos assets `Assets/Dados/Partida/Partida` (Arena) e `PartidaTutorial` (mercado, igual com *Quantidade De Ondas* 1) e podem ser ajustados no Inspector.

**Código** (`Assets/Scripts/Partida/`):
- `ParametrosDaPartida` (struct): `DuracaoDaOnda` (> 0) e `QuantidadeDeOndas` (≥ 1), validados no construtor (`ArgumentException`).
- `EstadoDaPartida` (enum): `EmOnda`, `ChefeDaOnda`, `ChefeDaArena`, `Vitoria` e `Derrota`.
- `CicloDaPartida` (C# puro, testado em EditMode): `Onda`, `TempoRestante`, `Estado`, `EmOnda`, `EmChefe` e `Encerrada`.
  - `Atualizar(deltaTempo)`: o cronômetro só corre durante a onda e não fica negativo; devolve `true` no passo em que o tempo acaba (a fase vira chefe). O fim do tempo não depende da taxa de quadros.
  - `DerrotarChefe()`: começa a próxima onda (tempo cheio) ou, depois do chefe da arena, a vitória.
  - `DerrotarJogador()`: derrota, a partir de qualquer fase em jogo. Depois do fim, nada muda o estado.
  - `Reiniciar()`: onda 1 com o tempo cheio.
  - `FormatarTempo(segundos)` (estático): "m:ss", arredondando para cima ("2:00" no início, "0:00" só quando acabou).
- `ComposicaoDaOnda` (estática, C# pura): `CristaisDeExperiencia(onda, naPrimeiraOnda)` (dobra a cada onda), `TrazItensNovos(onda)` (só a partir da 2ª) e `SortearItemDeHabilidade(quantidade, sorteio)`.
- `GeradorDePosicoes` (estático, C# puro): `Gerar(quantidade, limite, jogador, ocupadas, distanciaMinima, sorteio, tentativasPorPosicao, proibidas)` sorteia posições no chão respeitando a área, as distâncias e as áreas proibidas do nível (retângulos `Retangulo`; os inválidos são ignorados); com a área cheia, devolve menos posições. Com a mesma semente, o resultado é o mesmo.
- `DefinicaoPartida` (ScriptableObject, menu *Create > Coletor de Cristais > Definição de Partida*): *Duracao Da Onda* 120, *Quantidade De Ondas* 3, *Espera Antes De Continuar* 0,5, *Cristais De Experiencia Na Primeira Onda* 4, *Prefab De Experiencia* (`Cristal_Experiencia`), *Comidas Por Onda* (`Comida_Carne`, `Comida_Peixe` ×2, `Comida_Coxa` ×2), *Itens De Habilidade* (`Comida_Cafe`, `Comida_Rosquinha`), *Limite Da Area* 8, *Distancia Minima* 1,5 e *Altura Dos Itens* 0,8.
- `ControladorDaPartida` (`MonoBehaviour`, no objeto `Partida`): corre o cronômetro com `Time.deltaTime`, liga e desliga os componentes do jogador (`MovimentoDoJogador`, `InvestidaDoJogador`, `ImaDoJogador` e `ColetorDoJogador`) e, fora de uma onda, escuta qualquer tecla (`InputSystem.onAnyButtonPress`). `Continuar()` faz o que a tecla faz (público para testes pela CLI); `DerrotarJogador()` registra a derrota (chamado pela queda no vão e, no futuro, pela vida). Coloca os itens novos com `Instantiate` dentro do objeto `Cristais`; com um *Nivel* (`DefinicaoNivel`, opcional), eles não entram nos vãos nem nas áreas sem itens dele. Valores inválidos no asset registram um erro e desligam o componente.
- `MensagensDaPartida` (`MonoBehaviour`, no objeto `Partida`): **interface mínima e provisória**, até o HUD completo. Cria no `Awake` um Canvas de uGUI em *Screen Space - Camera* na câmera principal, com dois textos (`Text` com a fonte embutida `LegacyRuntime.ttf`): o cronômetro no topo e a mensagem da fase no centro. Como o Canvas é desenhado pela câmera, os textos aparecem nas capturas da câmera. `TextoDaMensagem(estado, onda, aguardandoTecla)` (estático, testado) monta o texto central.
- `CristalColetavel` só coleta se o `ColetorDoJogador` estiver ligado (`isActiveAndEnabled`): é assim que a coleta para fora das ondas.

**Decisões técnicas:**
- O jogador é travado desligando os componentes dele, sem cada sistema precisar saber da partida. Os efeitos temporários param junto porque perdem tempo no `MovimentoDoJogador`.
- Recomeçar recarrega a cena: tudo volta ao estado inicial (itens, habilidades, bônus e contagem) sem código de limpeza.
- A primeira onda usa os itens colocados na cena; as seguintes sorteiam posições com `System.Random` sem semente fixa, então cada partida é diferente.

**Como foi montado na Editor:**
1. **Valores:** pasta `Assets/Dados/Partida`, *Create > Coletor de Cristais > Definição de Partida* (`Partida`). Preencha *Prefab De Experiencia*, *Comidas Por Onda* e *Itens De Habilidade* com os prefabs listados acima.
2. **Cena:** um GameObject vazio `Partida` na raiz, com `Controlador Da Partida` (*Definicao* `Partida`, *Jogador* o objeto `Jogador`, *Grupo De Itens* o objeto `Cristais`) e `Mensagens Da Partida`.

## Mercado (arena 0)
**Como funciona no jogo:** o jogo começa num minimercado, a arena 0, que serve de tutorial. A planta é **provisória**: a definitiva vai seguir um desenho do desenvolvedor.

![O mercado visto pela câmera do jogo](docs/atualizacoes/mercado.png)

- **Planta** (20×20 m, com a mesma câmera da Arena), de sul a norte:
  - **açougue**, onde o personagem começa (z de -10 a -5), com um balcão de freezers junto da parede sul;
  - **3 corredores** de cerca de 4,4 m de largura entre prateleiras, de z = -4,6 a 1,8: o do meio (x de -2,3 a 2,3) e os dois laterais (x de ±4,5 a ±8,9);
  - um **corredor transversal** (z de 1,8 a 4);
  - o **vão**, de 3 m, na largura toda (z de 4 a 7);
  - a **área bônus**, junto da parede norte (z de 7 a 10).
- **Partida:** 1 onda de 120 s. Quando o tempo acaba, vem o chefe da arena (por enquanto só a mensagem) e depois "Arena concluída!".
- **Vão:** cai quem está com o centro sobre o vão sem estar numa investida, e **cair é fim de jogo** ("Derrota"); o personagem afunda 5 m no fosso. Durante a investida ele passa por cima, mas cai se ela terminar sobre o vão. Por isso o vão **só se atravessa com a investida** (liberada pelo café):

  | Velocidade | Investida | Começar a até |
  |---|---|---|
  | Base (3,5 m/s) | 4,375 m | 1,375 m da borda |
  | Com a coxa ativa (+30%, 4,55 m/s) | 5,69 m | 2,69 m da borda |

  Começando mais longe, a investida termina sobre o vão. A volta funciona do mesmo jeito.
- **Carrinhos:** um carrinho de compras por corredor, em vai e vem a 2 m/s entre z = -4 e 0,5 (4,5 m), os três em momentos diferentes do ciclo. Quem fica na frente de um carrinho é empurrado até o fim do percurso dele; encostar de lado não faz nada. Fora da onda, os carrinhos param.
- **Itens:** carne no ponto de partida (coletada logo no início), 2 coxas no açougue, 2 peixes e 4 cristais de experiência nos corredores, o café no fim do corredor do meio e, na área bônus, a rosquinha e 10 cristais de experiência. Os cristais da área bônus ficam a mais de 4 m da borda do vão, fora do alcance do ímã de quem está do outro lado. Com 1 onda, nenhum item novo é sorteado.
- **Segredo das prateleiras:** uma investida contra uma prateleira derruba um produto sorteado (lata, refrigerante, garrafa ou biscoito), **uma vez por prateleira** (são 30, uma por peça). Andando, nada cai. O produto cai a 0,8 m da face batida e 1,5 m para um lado sorteado, longe o bastante para não ser coletado na hora. Ele é coletado e contado, mas **ainda não tem efeito** (ímã reduzido, como as comidas com efeito previsto).

Os valores ficam no asset `Assets/Dados/Nivel/Mercado` e podem ser ajustados no Inspector.

**Código** (`Assets/Scripts/Nivel/`):
- `Retangulo` (struct serializável, C# puro): retângulo no chão alinhado aos eixos (`x` = X do mundo, `y` = Z do mundo), usado no vão, nas bases das prateleiras e dos carrinhos e nas áreas sem itens. `PelosCantos`, `Minimo`, `Maximo`, `Contem` (a borda conta), `PontoMaisProximo`, `DistanciaAte`, `ComCentro` e `Valido` (falso com tamanho zero, como um item novo de lista no Inspector).
- `VaiEVem` (estático, C# puro): `Posicao(inicio, fim, velocidade, tempo, fase)` e `Percorrido(...)`. A posição vem direto do tempo acumulado (em `double`), então é a mesma em qualquer taxa de quadros; a `fase` (0 a 1) adianta o ciclo.
- `Empurrao` (estático, C# puro): `Calcular(obstaculo, movimento, personagem, raio, folga)`. Se, com o obstáculo na posição nova, o círculo do personagem ficou sobreposto a ele, devolve o deslocamento na direção do movimento do obstáculo que tira o personagem de lá, mais a folga. A saída do retângulo "engordado" pelo raio (convexo) é achada por busca binária, então funciona também na quina.
- `RegraDeQueda` (estática, C# pura): `Caiu(posicao, emInvestida, vaos)` e `Descida(tempo, gravidade, profundidade)` (queda livre com limite, só o desenho).
- `EstadoDaPrateleira` (C# puro): `TentarDerrubar(emInvestida)` (só na primeira batida durante uma investida), `SortearProduto`, `PosicaoDoProduto(prateleira, personagem, afastamento, deslocamentoLateral)` e `NormalDaFace`.
- `DefinicaoNivel` (ScriptableObject, menu *Create > Coletor de Cristais > Definição de Nível*): *Vaos* (o do mercado: centro (0; 5,5), tamanho (20; 3)), *Gravidade Da Queda* 20, *Profundidade Da Queda* 5, *Areas Sem Itens Novos* (as prateleiras, o balcão e a área bônus), *Velocidade Dos Carrinhos* 2, *Produtos* (os 4 `Produto_*`), *Afastamento Do Produto* 0,8, *Deslocamento Lateral Do Produto* 1,5 e *Altura Do Produto* 0,8. `ZonasSemItensNovos` junta os vãos e as áreas.
- `ControladorDoNivel` (`MonoBehaviour`, no objeto `Nivel`): no `LateUpdate`, depois da investida andar no quadro, aplica a `RegraDeQueda` durante a onda; ao cair, chama `ControladorDaPartida.DerrotarJogador()` e afunda o jogador. `EmJogo` diz se a partida está numa onda. Também guarda o sorteio e as referências que os carrinhos e as prateleiras usam.
- `CarrinhoVaiEVem` (`MonoBehaviour`, em cada carrinho, com `BoxCollider` e `Rigidbody` cinemático): *Ponto De Volta* (0, 0, 4,5) e *Fase* (0, 1/3 e 2/3). A cada quadro em jogo, calcula a posição nova, empurra o jogador com `CharacterController.Move` e só depois anda.
- `PrateleiraSecreta` (`MonoBehaviour`, em cada peça de prateleira, com `BoxCollider`): `ReceberBatida(personagem, emInvestida)` derruba o produto (instanciado dentro de `Cristais`).
- `ImpactoDaInvestida` (`MonoBehaviour`, no `Jogador`): no `OnControllerColliderHit`, avisa a `PrateleiraSecreta` só quando a batida veio do deslocamento da investida (`InvestidaDoJogador.AplicandoInvestida`, verdadeiro durante esse `Move`, inclusive no último quadro do avanço).

**Decisões técnicas:**
- O jogo não tem gravidade (o `CharacterController` só anda no plano), então a queda é uma regra sobre a posição, e a descida é só visual. O fosso não tem colisão.
- O `CharacterController` não é empurrado por corpos cinemáticos, então o empurrão é calculado. O carrinho empurra antes de andar e a folga inclui a pele (*Skin Width*) do `CharacterController`: se o carrinho entrasse no personagem primeiro, o `CharacterController` resolveria a sobreposição subindo em cima dele.
- Os carrinhos não giram: a base deles é um retângulo alinhado aos eixos.
- As peças do Mini Shop são pequenas para o montrengo (2 m), então foram aumentadas: prateleiras ×1,6 (1,28 m de comprimento, 1,42 m de altura e 1,12 m de profundidade), carrinhos ×3 (base de 1,05 × 1,6 m) e freezers ×2.
- Cada peça de prateleira tem a própria caixa de colisão e o próprio segredo. As fileiras do meio são duas peças de costas uma para a outra, uma para cada corredor.

**Como foi montado na Editor:**
1. **Valores:** `PartidaTutorial` é uma cópia de `Partida` com *Quantidade De Ondas* 1. Em `Assets/Dados/Nivel`, *Create > Coletor de Cristais > Definição de Nível* (`Mercado`), com os valores acima.
2. **Produtos:** assets `DefinicaoCristal` `Lata`, `Refrigerante`, `Garrafa` e `Biscoito` (sem efeitos, ímã `Reduzida`) e Prefab Variants de `Cristal` com `can`, `soda-can`, `soda-bottle` e `cookie` do Food Kit, na mesma receita das comidas (maior dimensão 0,8 m).
3. **Cena:** `Mercado.unity` é uma cópia da `Arena.unity`. O objeto `Arena` virou `Mercado`, com o visual `MercadoVisualKenney` (sem as 60 placas de piso de z = 4 a 7). Na raiz, um objeto `Nivel` com `Controlador Do Nivel` (*Definicao* `Mercado`, *Jogador*, *Partida* e *Grupo De Itens* `Cristais`) e os filhos:
   - `Vao`: o fosso, com cubos sem colisão e o material `Vao`: fundo em y = -6 e quatro paredes;
   - `Acougue`: uma caixa de colisão de 16 × 1,8 × 0,96 m junto da parede sul e 8 `freezers-standing` (×2);
   - `Prateleiras`: 6 fileiras de 5 peças (`shelf-boxes` e `shelf-bags` alternadas, ×1,6), em x = ±9,44 (paredes), ±3,96 e ±2,84 (meio), de z = -4 a 1,12, cada uma com `Box Collider` e `Prateleira Secreta`;
   - `Carrinhos`: 3 `shopping-cart` (×3) em x = -6,7, 0 e 6,7, z = -4, cada um com `Box Collider` do tamanho do modelo, `Rigidbody` cinemático e `Carrinho Vai E Vem`.
4. **Jogador** em (0, 1, -7), com `Impacto Da Investida`. Na `Partida`, *Definicao* `PartidaTutorial` e *Nivel* `Mercado`.
5. **Build Profile:** `Mercado` primeiro, `Arena` depois.

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
