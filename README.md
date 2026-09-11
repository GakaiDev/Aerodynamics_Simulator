# Simulador Aerodinâmico 2D

Simulador de escoamento potencial bidimensional desenvolvido do zero em C# com a engine Godot, aplicando conceitos avançados de Álgebra Linear, Geometria Analítica e Mecânica dos Fluidos. O projeto abstrai motores físicos tradicionais para resolver numericamente o comportamento de fluidos ideais (inviscidos e incompressíveis) ao redor de geometrias arbitrárias.

---

## Sobre o Projeto
Desenvolvido como projeto prático para a disciplina de Álgebra Linear na **UFERSA (Universidade Federal Rural do Semi-Árido)**, este software demonstra como a transposição de problemas físicos contínuos para modelos discretos resulta invariavelmente em sistemas de equações lineares da forma:
$$[A][\lambda] = [b]$$

O simulador monta a matriz de influência geométrica, resolve o sistema em tempo real e renderiza o campo de velocidades, as linhas de corrente e o mapa termal de pressões ($C_p$) sobre diferentes obstáculos.

---

## Fundamentos Matemáticos e Computacionais

* **Discretização Geométrica (Painéis):** O contorno do objeto é dividido em $N$ segmentos de reta orientados no sentido horário, calculando pontos de controle, vetores tangentes ($\vec{t}$) e vetores normais ($\vec{n}$).
* **Condição de Contorno de Dirichlet:** Impede a penetração do fluido impondo que a soma da velocidade do vento livre com as fontes induzidas seja nula na direção normal de cada painel.
* **Eliminação de Gauss com Pivotamento Parcial:** Solucionador matricial customizado desenvolvido para isolar o vetor de intensidades $[\lambda]$, garantindo estabilidade numérica contra divisões por zero em geometrias com arestas vivas.
* **Equação de Bernoulli e Coeficiente de Pressão ($C_p$):** Cálculo analítico da velocidade tangencial superficial para mapeamento termal de sobrepressão e sucção, evidenciando o *Paradoxo de D'Alembert*.
* **Integração de Euler e Detecção de Colisão:** Propagação cinemática de partículas de vento e aplicação do produto escalar adaptado do Teorema do Eixo Separador (TES) para assegurar o escoamento tangencial sem interpenetração.

---

## Tecnologias Utilizadas
* **Linguagem:** C# (.NET)
* **Engine Gráfica:** Godot Engine (para renderização vetorial 2D e interface em tempo real)

---

## Funcionalidades e Geometrias Testadas
O simulador suporta a alternância em tempo real entre três topologias distintas:
1. **Círculo:** Validação analítica com curvatura homogênea e simetria de escoamento.
2. **Quadrado:** Teste de estresse algébrico com quinas de $90^\circ$ e gradientes de velocidade bruscos.
3. **Cunha / Triângulo:** Análise de perfil aerodinâmico angular com superfícies não-ortogonais ao fluxo livre.

---

## Como Executar o Projeto

1. Certifique-se de ter a **Godot Engine** (compatível com .NET / C#) instalada em sua máquina.
2. Clone este repositório:
   ```bash
   git clone [https://github.com/seu-usuario/aerodynamics-simulator-2d.git](https://github.com/seu-usuario/aerodynamics-simulator-2d.git)
   ```
3. Abra a Godot Engine, clique em Import e selecione o arquivo project.godot localizado na pasta raiz do repositório clonado.
4. Pressione F5 para compilar o código C# e executar a simulação.
