# MateMagia

Jogo educativo 3D feito em Unity para praticar **adição, subtração, multiplicação e divisão** com crianças do Ensino Fundamental I. O jogador atravessa quatro mundos e derrota inimigos resolvendo contas, com figuras de apoio ao lado dos números para quem ainda conta nos dedos.

Projeto desenvolvido como Trabalho de Conclusão de Curso de Sistemas de Informação na **UNIARA**.

Disponível em **português, inglês e espanhol**.

---

## Como funciona

- **Modo História:** cutscene de abertura, as quatro fases em sequência, chefe final e cutscene de encerramento.
- **Modo Fases:** o jogador escolhe uma operação e completa a fase **3 vezes**.
- **Combate por quiz:** ao encontrar um inimigo, aparece uma conta com 4 alternativas. Acertar lança uma bola de fogo e errar tira um dos 3 corações.
- **Números de 1 a 5** em todas as operações. A divisão é sempre exata.
- **Referência visual:** cada número aparece acompanhado de figuras no padrão de dado, uma por fase.

| Fase | Operação | Figura |
|---|---|---|
| Floresta Somaris | Adição | Cogumelo |
| Deserto de Meenos | Subtração | Cacto |
| Antigo Templo Dyvi Zan | Divisão | Coqueiro |
| Galáxia V-zis | Multiplicação (chefe final) | Ametista |

## Plataforma NextLearn

O jogo pode ser usado junto com a **NextLearn**, uma plataforma web em que o professor cadastra a turma e acompanha acertos, erros, tempo e respostas de cada aluno. O jogo envia esses dados para a API pelo `ApiManager.cs` e pelo plugin `ApiPlugin.jslib`, só no build WebGL.

Repositório da plataforma: [nextlearn-matemagia](https://github.com/caioapgoncalves/nextlearn-matemagia)

## Tecnologias

- Unity **6000.2.7f2** com Universal Render Pipeline (URP)
- C#
- TextMeshPro, Cinemachine, Input System, Timeline
- DOTween (animações de interface e de combate)
- Build principal: **WebGL**

## Como abrir o projeto

Os arquivos grandes (modelos, texturas, áudio e vídeos) estão no **Git LFS**, então instale o LFS antes de clonar:

```bash
git lfs install
git clone https://github.com/caioapgoncalves/matemagia-unity.git
```

Depois:

1. Abra a pasta no **Unity Hub** com a versão **6000.2.7f2**.
2. Abra a cena `Assets/Scenes/Menu.unity` e aperte Play.

### Ordem das cenas no build

`Menu` → `CutSceneInicial` → `Adicao` → `Subtracao` → `Divisao` → `Multiplicacao` → `CutSceneFinal`

## Estrutura

```
Assets/
├── Scenes/              Menu, cutscenes e as 4 fases
├── Prefabs/
│   ├── scripts/         Lógica do jogo (quiz, jogador, inimigos, idiomas, API)
│   ├── canvas/          Painel do quiz, corações e fonte
│   └── personagems/     Personagens, animações e prefabs de inimigos
├── Plugins/             DOTween e WebGL/ApiPlugin.jslib (ponte com a NextLearn)
├── StreamingAssets/     Vídeos das cutscenes
└── Editor/              Ferramentas do editor (menu "Ferramentas")
```

### Scripts principais

| Script | Função |
|---|---|
| `QuizIntermed.cs` | Gera a conta, as alternativas e as figuras de apoio |
| `AndarAutomaticoPLayer.cs` | Movimento do jogador, vidas, rodadas e envio do desempenho |
| `InimigoQuiz.cs` / `InimigoChefaoFinalQuiz.cs` | Inimigos comuns e chefe final |
| `MenuPrincipal.cs` | Navegação do menu |
| `Idioma.cs`, `TextoTraduzido.cs`, `SeletorIdioma.cs` | Sistema de idiomas (PT / EN / ES) |
| `ApiManager.cs` | Envio do desempenho para a NextLearn |

### Ferramentas do editor (menu **Ferramentas**)

- **Igualar Fases:** deixa as 4 fases com 3 rodadas, números até 5 e figuras de apoio.
- **Configurar Idiomas:** aplica as traduções no menu, nos nomes das fases e no painel de pausa.
- **Otimizar Texturas:** reduz o tamanho e comprime as texturas do projeto.

## Idiomas

O idioma é escolhido nos botões **PT / EN / ES** do menu principal e fica salvo. Para editar um texto, use o componente **Texto Traduzido** no Inspector. Os nomes das fases e o aviso do chefe ficam nos componentes **NomeFase** e **AvisoBoss** de cada cena.

## Artigo

GONÇALVES, Caio Augusto Poleto; SILVA, André Luiz da; FLORIAN, Fabiana. **Aplicação da gamificação no ensino de operações matemáticas básicas para o Ensino Fundamental**. *Revista Ibero-Americana de Humanidades, Ciências e Educação (REASE)*, v. 12, n. 10, 2026.

## Autores

- **Caio Augusto Poleto Gonçalves:** desenvolvimento
- **Prof. Dr. André Luiz da Silva:** orientador (UNIARA)
- **Profa. Fabiana Florian:** coorientadora (UNIARA)

## Créditos

- Modelos 3D, VFX, cenários e interface de pacotes de terceiros da Unity Asset Store (Polyart, Hovl Studio, Vefects, BOXOPHOBIC e outros, nas pastas de origem dentro de `Assets/`)
- Parte dos modelos foi gerada com **Tripo AI** e parte das animações vem do **Mixamo**
- DOTween, da Demigiant
- Fonte Chewy, do Google Fonts

## Licença

A definir.
