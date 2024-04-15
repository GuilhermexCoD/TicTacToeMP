# Tic Tac Toe

## Resumo

O jogo da velha, também conhecido como tic-tac-toe, é um jogo de estratégia para dois jogadores. É jogado em um tabuleiro de 3x3 (ou seja, 3 linhas por 3 colunas), totalizando 9 casas. Os jogadores se alternam ao marcar uma casa vazia no tabuleiro com um dos símbolos: "X" ou "O". O objetivo do jogo é ser o primeiro a formar uma linha reta com três símbolos iguais, seja horizontal, vertical ou diagonal.

## Regras

Aqui estão as regras detalhadas do jogo:

Tabuleiro: O jogo é jogado em um tabuleiro quadrado de 3x3, composto por nove casas.

Jogadores: Dois jogadores participam do jogo. Um jogador usa o símbolo "X" e o outro jogador usa o símbolo "O".

Ordem de jogada: Os jogadores se revezam para jogar. O jogador com o símbolo "X" geralmente começa, seguido pelo jogador com o símbolo "O". Eles continuam alternando jogadas até o final do jogo.

Marcação: Cada jogador, em sua vez, coloca seu símbolo ("X" ou "O") em uma das casas vazias do tabuleiro.

Objetivo: O objetivo do jogo é formar uma linha reta com três símbolos iguais, seja horizontal, vertical ou diagonal.

Condições de vitória: O jogo termina quando um jogador consegue formar uma linha reta com três símbolos iguais (horizontal, vertical ou diagonal). O jogador que conseguir isso é declarado vencedor.

Empate: Se todas as casas do tabuleiro forem preenchidas e nenhum jogador conseguir formar uma linha reta com três símbolos iguais, o jogo termina em empate.

Duração: Um jogo de tic-tac-toe é geralmente curto, pois pode terminar rapidamente se um jogador conseguir formar uma linha reta com seus símbolos ou se o tabuleiro ficar cheio sem que nenhum jogador vença.

Essas são as regras básicas do jogo da velha. É um jogo simples, mas pode ser desafiador, exigindo estratégia e pensamento antecipado dos jogadores.

## Caracteristicas

- Visualização 2D
- Tabuleiro (3x3)
  - Possui quadrados ou Tiles
    - Podemos criar um Prefab do Tile
      - Imagem Background do Tile
      - Imagem com o Icon (X ou O)
      - Script para realizar essas alterações

- Numero de jogadores = 2
  - Nome
  - Numero de vitorias
  - Numero de derrotas
  - Numero de empate
  - Calcular numero total de partidas (vitorias + derrotas + empates)

- GameManager 
  - Determina quantas partidas vão ser jogadas no total.
  - Contagem de partidas ganhas dos dois jogadores
  - Contagem de partidas que foram empate
  - Fluxo do jogo

- HUD
  - Botão de desistir
  - Numero de Partidas
  - Numero de Derrotas
  - Numero de Vitorias
  - Numero de Empates

## Fluxo do Jogo

1. Escolhemos um jogador para começar.
    - o primeiro jogador sera o Host.
2. Jogador da vez escolhe um quadrado vazio para colocar seu simbolo.
3. Validamos se não houve alguma condição de vitoria ou empate.
4. Passamos a vez para o proximo jogador.
5. Repete o item 2 até condição de vitoria ou empate.
