import random

"""
Sabendo que você tem dois arrays de números inteiros, crie um terceiro array com a junção dos dois anteriores em 
ordem crescente.
"""
#Selection Sort - 
def concatAndSort(a, b):
    # Concatena o array a e b em c para criar a ordenação
    c = []
    for i in a:
        c.append(i)
    for j in b:
        c.append(j)

    cLen = len(c)
    for i in range(cLen):
        minIndex = i
        for j in range(i + 1, cLen):
            if c[j] < c[minIndex]:
                minIndex = j
        c[i], c[minIndex] = c[minIndex], c[i]
    return c

# Insertion Sort -
def concatAndSortInsertion(a, b):
    # Concatena o array a e b em c para criar a ordenação
    c = []
    for i in a: c.append(i)
    for j in b: c.append(j)

    for i in range(1, len(c)):
        key = c[i]
        j = i - 1
        while j >= 0 and c[j] > key:
            c[j + 1] = c[j]
            j -= 1
        c[j + 1] = key

    return c

"""
Imagine que você tenha uma tela com duas entradas, uma com o texto e outra com a string a ser encontrada. Monte um 
algoritmo para encontrar a posição dessa string nesse texto.  Caso não encontre, retornar -1.
"""
def findSubstringIndex(text, search = ''):
    if (len(search) == 0):
        return -1

    t = len(text)
    s = len(search)

    if s > t:
        return -1

    i = 0
    while (i <= t - s):
        k = 0
        while (k < s and text[k + i] == search[k] ):
            k += 1
        if (k == s):
            return i
        i += 1
    return -1

"""
Dando um número N inteiro, escreva um algoritmo que descreva os N números da sequencia de Fibonacci
"""
def fibonacci(N):
    if N <= 0:
        return []
    if N == 1:
        return [0]
    seq = [0, 1]
    for _ in range(2, N):
        seq.append(seq[-1] + seq[-2])
    return seq

"""
 Crie uma função ou procedimento que receba uma matriz AxB do tipo numérico e dois parâmetros que indicam o 
tamanho da matriz A, B. Encontre o maior número dessa matriz.
"""
def getMaxInMatrix(mat, A, B):
    if A == 0 or B == 0 or len(mat) == 0:
        raise ValueError("Matriz vazia")
    max = mat[0][0]
    i = 0
    while(i < A):
        j = 0
        while(j < B):
            if(mat[i][j] > max):
                max = mat[i][j]
            j += 1
        i += 1
    return max

"""
 Informando uma entrada numérica N, informe o total da multiplicação de N números primos seguidos.
"""
def isPrime(n):
    if n <= 1:
        return False
    if n == 2:
        return True
    if n % 2 == 0:
        return False
    d = 2
    while d * d <= n:
        if n % d == 0:
            return False
        d += 1
    return True

def prodPrimeNumbers(n):
    qtd = 0
    x = 2
    prod = 1
    while qtd < n:
        if isPrime(x):
            prod = prod * x
            qtd += 1
        x += 1
    return prod

""" utils para gerar matrizes aleatorias para teste """
A, B = 3, 3
min_value, max_value = -2, 30
mat = []
for i in range(A):
    rows = []
    for j in range(B):
        rows.append(random.randint(min_value, max_value))
    mat.append(rows)

""" Matrizes estaticas """
A, B = 3, 3
mat = [[7, -2, 4], [1, 10, 3], [6, 5, 0]]

"""run"""
fun1 = concatAndSort([2,4,9,1],[7,3,5,6])
fun1I = concatAndSortInsertion([2,4,9,1],[7,3,5,6])
fun2 = findSubstringIndex("ababc", "abc")
fun3 = fibonacci(7)
fun4 = getMaxInMatrix(mat, A, B)
fun5 = prodPrimeNumbers(4)

print('Arrays concatenados e ordenados: ', fun1)
print('Arrays concatenados e ordenados insertion: ', fun1I)
print('Encontrar posição na string: ', fun2)
print('Sequencia fibonnaci: ', fun3)
print('Maximo numero em uma matrix: ', fun4)
print('Total da multiplicação de N números primos seguidos: ', fun5)
