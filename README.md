# Coletor de Cristais

Jogo 3D em Unity sobre coletar cristais numa arena.

## O jogo
Arena 3D com câmera fixa elevada. O jogador anda pela arena e coleta cristais. Cada cristal é um item com um efeito:
- **Atributo:** aumenta a velocidade.
- **Habilidade passiva:** ímã, que puxa cristais próximos.
- **Habilidade ativa:** investida, com tempo de recarga.

Plataforma: Windows. Sem monetização.

> Estado atual: só o projeto, o repositório e a estrutura de testes existem. Movimento, cristais e atributos ainda não foram implementados.

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
Na Editor, abra `Assets/Scenes/SampleScene.unity` e aperte Play.

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

## Estrutura de pastas
| Caminho | Conteúdo |
|---|---|
| `Assets/Scripts/` | Código do jogo. Assembly `ColetorDeCristais` (`ColetorDeCristais.asmdef`), namespace `ColetorDeCristais` |
| `Assets/Scripts/InformacoesDoProjeto.cs` | Classe mínima (nome do jogo), criada com o projeto para validar a estrutura de testes |
| `Assets/Testes/EditMode/` | Testes EditMode. Assembly `ColetorDeCristais.Testes.EditMode`, namespace `ColetorDeCristais.Testes`; referencia `ColetorDeCristais` e só compila na Editor |
| `Assets/Scenes/` | Cenas (`SampleScene` do template) |
| `Assets/Settings/` | Assets do URP (pipelines PC e Mobile, renderers, volume profiles) |
| `Assets/InputSystem_Actions.inputactions` | Ações padrão do Input System, vindas do template |
| `Assets/TutorialInfo/`, `Assets/Readme.asset` | Readme do template da Unity (pode ser removido quando atrapalhar) |
| `Packages/` | `manifest.json` e lock dos pacotes UPM. Inclui `com.unity.pipeline` (liga a CLI à Editor) e `com.unity.test-framework` |
| `ProjectSettings/` | Configurações do projeto |

Pastas geradas (`Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, `.csproj`, `.slnx`) estão no `.gitignore`.

## Convenções
- Código novo vai em `Assets/Scripts/`, dentro do namespace `ColetorDeCristais` (subpastas e sub-namespaces por sistema, conforme as tarefas forem criando).
- Toda lógica nova tem teste EditMode em `Assets/Testes/EditMode/`. Lógica testável fica em classes C# puras, separada dos `MonoBehaviour`, para não depender de Play Mode.
- Cada `MonoBehaviour` fica sozinho em um `.cs` com o nome da classe.
- Uma branch por tarefa (ex.: `movimento-jogador`). Os commits são feitos pelo desenvolvedor.
- Nada pessoal ou externo ao jogo entra no repositório (dados pessoais, credenciais, cronogramas, datas, notas). Esse conteúdo fica em arquivos ignorados, como o `CLAUDE.local.md`.
- Regras para agentes: ver `CLAUDE.md`.

## Versionamento
- `.gitignore` de Unity na raiz. A seção "Arquivos pessoais e locais" ignora `CLAUDE.local.md`, `*.local.md`, as configurações locais do Claude Code (`.claude/settings.local.json`), as pastas `Notas/` e `Rascunhos/`, os arquivos `*.notas.md`, `.env` e os arquivos de credenciais e chaves.
- `.gitattributes` com Git LFS para texturas, áudio, modelos 3D, vídeo, fontes e outros binários, e merge `unityyamlmerge` para os YAML da Unity (para usar o UnityYAMLMerge como ferramenta de merge, configure-o no Git local; sem isso o Git faz merge de texto normal).
