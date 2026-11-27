# CLAUDE.md — Carne Viva

Instruções para agentes que trabalham neste repositório. Leia também o `README.md` (o que é o jogo, como abrir, testar e a estrutura de pastas).

Detalhes da máquina local (caminhos de instalação, restrições de disco) **não são versionados**: ficam em `CLAUDE.local.md`, que está no `.gitignore`. Se o arquivo não existir na sua máquina, peça esse contexto ao desenvolvedor.

## Contexto
- Unity 6000.3.28f1 (6.3 LTS), URP, Windows.
- Cada sessão trabalha em **uma tarefa** combinada com o desenvolvedor. Não adiante outras tarefas.

## Regra de escopo
Decisões criativas e de escopo são sempre do desenvolvedor, nunca do agente. O agente pode propor ideias, apontar riscos e estimar impacto, mas não decide nem aplica. Decisões técnicas de implementação dentro de uma tarefa combinada (estrutura do código, nomes, padrões) o agente pode tomar, documentando; se a decisão muda o que o jogador vê ou faz, é criativa e fica com o desenvolvedor. Ideias ou mudanças de escopo que surgirem durante uma tarefa não são implementadas: o agente as descreve ao desenvolvedor como mudanças propostas, e o desenvolvedor decide se e quando entram. Exceção: correções de bug e ajustes técnicos necessários para concluir a tarefa combinada, sem mudar o que ela entrega.

## Regra de documentação
A documentação reflete o **produto**, não a conversa. Ela descreve o que o jogo **é** e como funciona hoje: regras, mecânicas, arquitetura, como rodar, testar e jogar. Não registra a conversa que levou até ali: nada de "conforme pedido", "o usuário disse" ou "o agente sugeriu", perguntas e respostas, nem relatos de sessão. O histórico de decisões e mudanças tem lugar próprio: o planejamento do jogo, fora deste repositório, o `ATUALIZACOES.md` e o histórico do Git.

## Regras gerais
- **Commits:** nenhum commit, push ou merge feito pelo agente. Ao terminar, apresente um resumo das alterações (arquivos e o que mudou) para o desenvolvedor revisar e commitar.
- **Branches:** uma branch por tarefa (ex.: `movimento-jogador`). O agente pode sugerir o nome; criar e trocar de branch é do desenvolvedor, a menos que ele peça.
- **GitHub:** comandos que acessam o GitHub e pedem login rodam na aba de terminal do desenvolvedor, nunca pelo agente.
- **Nada pessoal ou externo ao jogo no repositório:** informações pessoais (nomes, e-mails, contas, caminhos da máquina do usuário), credenciais e documentos de organização do trabalho (cronogramas, datas, notas) não entram em arquivos versionados. Esse conteúdo vai no `CLAUDE.local.md` ou em arquivos cobertos pelo `.gitignore` (seção "Arquivos pessoais e locais"). Ao criar um tipo novo de arquivo desse tipo, adicione o padrão ao `.gitignore` no mesmo conjunto de alterações. Antes de apresentar as alterações, confira com `git status` que nada desse tipo ficou para ser commitado.
- **Registro de atualizações:** ao concluir uma tarefa que muda o jogo, proponha a entrada correspondente no `ATUALIZACOES.md`, no formato e com o processo de imagens descritos no fim dele. Não inclua sprints, IDs de tarefa, datas nem dados pessoais. Imagens: só da câmera real do jogo, sem simular um visual que o jogo não tem. Para animações, use o `GravadorDeQuadros` (`Assets/Scripts/Ferramentas/`) e o `ferramentas/montar_apng.py`, sem scripts temporários. Só registre como validado o que foi de fato testado (ex.: não afirme teste com gamepad sem confirmação).
- **Assets de terceiros e créditos:** todo asset de terceiros (modelo, textura, som, fonte) fica em `Assets/Terceiros/<Criador>/<Pacote>/` junto do arquivo de licença original. No mesmo conjunto de alterações, ele ganha uma linha no `CREDITOS.md` (pacote, versão, criador, licença, origem, pasta e uso). Downloads só depois de o desenvolvedor aprovar nome, origem e tamanho, sempre numa pasta do E:.
- **Testes:** toda lógica nova tem testes EditMode em `Assets/Testes/EditMode/` (assembly `ColetorDeCristais.Testes.EditMode`). Rode os testes antes de apresentar as alterações e informe o resultado com fidelidade.
- **Documentação:** em português, como código, nomes de variáveis, comentários e docstrings (exceto palavras reservadas e APIs externas). Toda mudança de comportamento atualiza o `README.md` no mesmo conjunto de alterações.
- **Disco:** nada instalado no C: (está cheio). Ferramentas, caches e temporários ficam no E: (detalhes no `CLAUDE.local.md`).
- **Conta e sistema:** login na Unity, aceite de termos e UAC ficam com o desenvolvedor.

## Como trabalhar no projeto
- Código do jogo em `Assets/Scripts/` (assembly `ColetorDeCristais`, namespace `ColetorDeCristais`). Lógica testável em classes C# puras; `MonoBehaviour` só faz a ponte com a Unity. Um `MonoBehaviour` por arquivo, com o nome da classe.
- Organização por sistema: cada sistema tem uma subpasta e um sub-namespace (ex.: `Assets/Scripts/Atributos/`, `ColetorDeCristais.Atributos`), e os testes ficam na subpasta de mesmo nome em `Assets/Testes/EditMode/`.
- Atributos numéricos (velocidade e outros) usam `ColetorDeCristais.Atributos.Atributo` com modificadores. Não crie outro mecanismo de bônus; as regras de cálculo estão no README ("Atributos e modificadores"). Valores concretos (base, bônus, limites) são decisões criativas do desenvolvedor.
- O assembly `ColetorDeCristais` referencia `Unity.InputSystem`, `UnityEngine.UI` e o URP (`Unity.RenderPipelines.Core.Runtime` e `Unity.RenderPipelines.Universal.Runtime`). Se um novo assembly ou pacote for necessário, adicione a referência no `.asmdef` correspondente e documente no README.
- Cenas do jogo: `Assets/Scenes/Mercadinho.unity` (arena 0, o tutorial, primeira do build) e `Assets/Scenes/Arena.unity` (segunda). O personagem é o objeto `Jogador` (tag `Player`, `CharacterController`, `MovimentoDoJogador`); a velocidade dele é o `Atributo` em `MovimentoDoJogador.Velocidade`, onde itens aplicam modificadores. O tamanho é o `Atributo` em `CorpoDoJogador.Tamanho`, que escala o objeto `Jogador` inteiro (desenho e `CharacterController`): medidas que dependem do raio do personagem precisam usar a escala dele.
- Cristais: o prefab `Assets/Prefabs/Cristal.prefab` usa um asset `DefinicaoCristal` (em `Assets/Dados/Cristais/`). Novos tipos de cristal são novos assets de definição, não novos scripts. Efeitos são subclasses de `EfeitoDeCristal` colocadas na lista da definição. A contagem fica em `ColetorDoJogador.Contador` (no `Jogador`).
- Para validar uma coleta pela CLI: mover o jogador por vários `CharacterController.Move` num único `script eval` atravessa o trigger antes do passo de física e o cristal não é coletado. Posicione o jogador sobre o cristal e espere um quadro.
- Para verificar movimento em Play Mode pela CLI: com a Editor sem foco, o Input System desliga teclados simulados. Valide a lógica pelos testes EditMode e as colisões chamando o `CharacterController` por `script eval`. O teste com teclado de verdade é do desenvolvedor.
- Com a Editor aberta, edite cenas, prefabs e assets pela CLI (`unity command ...`, `script eval`), não editando o YAML à mão.
- Testes (na raiz do projeto):
  - Editor fechada: `unity test . --mode EditMode`
  - Editor aberta: `unity command test run --mode editor --filter ColetorDeCristais.Testes.EditMode --filter-type Assembly --project-path .`
- Não feche a Editor do desenvolvedor para rodar testes em batch.
