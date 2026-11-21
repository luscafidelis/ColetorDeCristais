# Coletor de Cristais

Jogo 3D em Unity sobre coletar cristais numa arena.

## O jogo
Arena 3D com câmera fixa elevada. O jogador anda pela arena e coleta cristais. Cada cristal é um item com um efeito:
- **Atributo:** aumenta a velocidade.
- **Habilidade passiva:** ímã, que puxa cristais próximos.
- **Habilidade ativa:** investida, com tempo de recarga.

Plataforma: Windows. Sem monetização.

> Estado atual: a arena com o personagem que anda (teclado ou gamepad) e a câmera fixa elevada; o sistema de atributos com modificadores permanentes e temporários (a velocidade do personagem já é um atributo); o cristal coletável base (some ao ser coletado e é contado, ainda sem efeito). Efeitos dos cristais, contagem na tela e sons ainda não foram implementados.

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

**Controles:** WASD ou setas no teclado; analógico esquerdo no gamepad.

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
| `Assets/Scripts/Atributos/` | Sistema de atributos em C# puro (namespace `ColetorDeCristais.Atributos`): `Atributo`, `Modificador`, `TipoModificador`. Ver "Atributos e modificadores" abaixo |
| `Assets/Scripts/Movimento/` | Movimento do personagem (namespace `ColetorDeCristais.Movimento`): `CalculadoraDeMovimento` (cálculos puros) e `MovimentoDoJogador` (`MonoBehaviour`). Ver "Movimento do personagem" abaixo |
| `Assets/Testes/EditMode/` | Testes EditMode. Assembly `ColetorDeCristais.Testes.EditMode`, namespace `ColetorDeCristais.Testes`; referencia `ColetorDeCristais` e só compila na Editor. Subpastas espelham as de `Assets/Scripts/` (ex.: `Atributos/`, `Movimento/`) |
| `Assets/Scenes/Arena.unity` | Cena do jogo (única cena no Build Profile): arena, jogador, câmera, luz |
| `Assets/Scenes/SampleScene.unity` | Cena de exemplo do template, fora do build; serviu de base para a Arena |
| `Assets/Scripts/Cristais/` | Cristais coletáveis (namespace `ColetorDeCristais.Cristais`): lógica pura (`EstadoDeColeta`, `ContadorDeColeta`, `CalculadoraDeFlutuacao`), dados (`DefinicaoCristal`, `EfeitoDeCristal`) e componentes (`CristalColetavel`, `ColetorDoJogador`, `GiroEFlutuacao`). Ver "Cristais coletáveis" abaixo |
| `Assets/Scripts/Ferramentas/` | Ferramentas de desenvolvimento (namespace `ColetorDeCristais.Ferramentas`): `GravadorDeQuadros`, só da Editor (`#if UNITY_EDITOR`, fica fora do build), grava quadros da câmera do jogo seguindo um `RoteiroDeEntrada` (C# puro, testado). Uso no `ATUALIZACOES.md` |
| `Assets/Prefabs/Cristal.prefab` | Prefab do cristal: trigger, coleta e visual animado |
| `Assets/Dados/Cristais/` | Assets `DefinicaoCristal` (por enquanto, `CristalBase`) |
| `Assets/Materiais/` | Materiais URP Lit: `Chao`, `Mureta`, `Jogador`, `NarizJogador`, `Cristal` |
| `Assets/Settings/` | Assets do URP (pipelines PC e Mobile, renderers, volume profiles) |
| `Assets/InputSystem_Actions.inputactions` | Ações do Input System (vindas do template). O personagem usa `Player/Move`. É o asset de ações do projeto (*Project Settings > Input System Package*) |
| `Assets/TutorialInfo/`, `Assets/Readme.asset` | Readme do template da Unity (pode ser removido quando atrapalhar) |
| `Packages/` | `manifest.json` e lock dos pacotes UPM. Inclui `com.unity.pipeline` (liga a CLI à Editor) e `com.unity.test-framework` |
| `ProjectSettings/` | Configurações do projeto |
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
- A remoção é pela instância do modificador (`RemoverModificador`), por origem (`RemoverModificadoresDaOrigem`, ex.: tudo que um item concedeu) ou total (`RemoverTodosModificadores`). Adicionar duas vezes a mesma instância gera `InvalidOperationException`.

**Permanentes e temporários:**
- Sem `duracao`, o modificador é **permanente**: só sai quando é removido.
- Com `duracao` (segundos, finita e maior que zero), o modificador é **temporário**. O tempo só passa quando quem usa o atributo chama `Atualizar(deltaTempo)` (ex.: uma vez por quadro com `Time.deltaTime`). Quando o tempo restante chega a zero, o modificador é removido e o valor final volta a ser calculado. `Atualizar` devolve quantos modificadores expiraram.
- `TentarObterTempoRestante(modificador, out tempo)` informa quanto falta (útil para a interface).
- **Renovação por grupo:** quando dois cristais temporários com o mesmo modificador são coletados, o tempo reseta e os modificadores se somam. Um temporário pode ter um `grupo` (texto, ex.: o tipo de cristal). Ao adicionar um temporário de um grupo que já está ativo, **todos** os temporários desse grupo voltam à duração completa do novo modificador, e os efeitos continuam somados. Por isso eles expiram juntos. Temporários sem grupo têm tempos independentes. Grupo só é aceito em temporários (`ArgumentException` em permanentes).

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
```

Os valores concretos de cada atributo e item (base, bônus, limites, durações e quais cristais são temporários) não fazem parte deste sistema: ficam nos componentes e assets que o usam.

## Movimento do personagem
**Como funciona no jogo:** controles por teclado (WASD/setas) e gamepad (analógico esquerdo); velocidade base 3,5 m/s; o personagem gira suavemente para a direção do movimento (720°/s); arena 20×20 m em cinza neutro; câmera fixa inclinada a 60°.

**Código** (`Assets/Scripts/Movimento/`):
- `CalculadoraDeMovimento` (estática, sem `MonoBehaviour`, testada em EditMode):
  - `CalcularDeslocamento(entrada, velocidade, deltaTempo)`: a entrada 2D vira um deslocamento no chão (x da entrada → X do mundo, y da entrada → Z). A entrada é limitada a tamanho 1, então a diagonal do teclado não é mais rápida. Velocidade negativa conta como zero.
  - `TentarCalcularAnguloAlvo(entrada, out angulo)`: ângulo Y para onde olhar (0° = +Z, 90° = +X). Entradas menores que `EntradaMinima` (0,01) não giram o personagem.
  - `GirarEmDirecao(atual, alvo, velocidadeAngular, deltaTempo)`: gira pelo caminho mais curto sem passar do alvo.
- `MovimentoDoJogador` (`MonoBehaviour`, exige `CharacterController`): lê a ação, chama a calculadora, aplica com `CharacterController.Move` e ajusta a rotação. Não tem lógica própria.

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
4. **Jogador:** *3D Object > Capsule* em (0, 1, 0), tag `Player`.
   - Remova o `Capsule Collider` e adicione um `Character Controller` (*Height* 2, *Radius* 0,5, *Center* (0, 0, 0)). O `Character Controller` já faz a colisão.
   - Filho `Nariz`: um cubo em (0, 0,5, 0,45), escala (0,3, 0,2, 0,3), sem `Box Collider` e com o material `NarizJogador`. Ele mostra para onde o personagem está virado.
   - Adicione o componente `MovimentoDoJogador`. Em **Acao Mover**, escolha `Player/Move` (clique no círculo ao lado do campo). Deixe **Velocidade Base** em 3,5 e **Velocidade De Giro** em 720.
5. **Câmera:** `Main Camera` com *Rotation* (60, 0, 0), *Position* (0, 19,27, -13,06) e *Field of View* 60. Com esses valores, a arena inteira cabe numa tela 16:9.

**Input System:** o pacote `com.unity.inputsystem` e o asset de ações já vêm no template. Em *Edit > Project Settings > Player > Active Input Handling*, o projeto usa só o Input System novo. Para ver ou mudar os botões, dê dois cliques em `InputSystem_Actions` e abra o mapa **Player > Move**: ele tem os bindings *WASD*, setas e *Left Stick* do gamepad.

## Cristais coletáveis
**Como funciona no jogo:** o cristal é um losango levemente esticado, ciano brilhante, que gira e flutua. Ao ser coletado, some na hora. Na Arena há 5 cristais: um no centro, onde o jogador nasce e por isso é coletado logo no início, e um perto de cada canto, em (±6, ±6). A coleta ainda não tem som nem contagem na tela.

**Código** (`Assets/Scripts/Cristais/`):

*Lógica pura* (testada em EditMode):
- `EstadoDeColeta`: a regra de coleta. Só um coletor válido coleta, e só uma vez, mesmo que a física avise duas entradas no trigger no mesmo quadro.
- `ContadorDeColeta`: total de cristais coletados e quantidade por tipo (identificador da definição). Tem `Zerar()`.
- `CalculadoraDeFlutuacao`: altura (senoide) e ângulo de giro em função do tempo.

*Dados:*
- `DefinicaoCristal` (ScriptableObject): `Identificador` (único e estável, usado na contagem), `NomeExibicao` e uma lista de `Efeitos`.
- `EfeitoDeCristal` (ScriptableObject abstrato): **ponto de extensão** para os efeitos (velocidade, ímã, investida). Ainda não existe nenhum efeito concreto e a lista da `CristalBase` está vazia. Cada efeito futuro será uma subclasse com `Aplicar(ColetorDoJogador coletor)`. Essa assinatura é provisória e pode mudar no primeiro efeito real.

*Componentes* (finos):
- `CristalColetavel`: no `OnTriggerEnter`, procura um `ColetorDoJogador` em quem entrou. Se a regra deixar, registra a coleta e destrói o cristal.
- `ColetorDoJogador` (no `Jogador`): guarda o `ContadorDeColeta` (`Contador`), escreve "Cristal coletado: …" no Console e aplica os efeitos da definição. É por ele que outras partes do jogo (ex.: interface) leem a contagem.
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

Ao apertar Play, o cristal do centro é coletado logo de cara e o Console mostra "Cristal coletado: Cristal Base (cristal-base). Total: 1.". Ande até os outros para coletá-los.

## Convenções
- Código novo vai em `Assets/Scripts/`, dentro do namespace `ColetorDeCristais` (uma subpasta e um sub-namespace por sistema).
- Toda lógica nova tem teste EditMode em `Assets/Testes/EditMode/`. Lógica testável fica em classes C# puras, separada dos `MonoBehaviour`, para não depender de Play Mode.
- Cada `MonoBehaviour` fica sozinho em um `.cs` com o nome da classe.
- Uma branch por tarefa (ex.: `movimento-jogador`). Os commits são feitos pelo desenvolvedor.
- Nada pessoal ou externo ao jogo entra no repositório (dados pessoais, credenciais, cronogramas, datas, notas). Esse conteúdo fica em arquivos ignorados, como o `CLAUDE.local.md`.
- Cada entrega que muda o que o jogador vê ou faz ganha uma entrada no [`ATUALIZACOES.md`](ATUALIZACOES.md), com imagem ou animação quando ajudar.
- Regras para agentes: ver `CLAUDE.md`.

## Versionamento
- `.gitignore` de Unity na raiz. A seção "Arquivos pessoais e locais" ignora `CLAUDE.local.md`, `*.local.md`, as configurações locais do Claude Code (`.claude/settings.local.json`), as pastas `Notas/` e `Rascunhos/`, os arquivos `*.notas.md`, `.env` e os arquivos de credenciais e chaves.
- `.gitattributes` com Git LFS para texturas, áudio, modelos 3D, vídeo, fontes e outros binários, e merge `unityyamlmerge` para os YAML da Unity (para usar o UnityYAMLMerge como ferramenta de merge, configure-o no Git local; sem isso o Git faz merge de texto normal).
