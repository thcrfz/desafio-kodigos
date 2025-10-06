-- CRIANDO O DATABASE
CREATE DATABASE IF NOT EXISTS estoque CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE estoque;

-- CRIANDO A TABELA PRODUTO
CREATE TABLE if NOT EXISTS Produto(
  id INT PRIMARY KEY,
  descricao VARCHAR(255) NOT NULL,
  um VARCHAR(10),
  valor DECIMAL(12,2),
  tam DECIMAL(12,2)
) ENGINE=InnoDB;

-- CRIANDO A TABELA ARMAZEM
CREATE TABLE IF NOT EXISTS Armazem(
  id INT PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  espacoTotal DECIMAL(14,2)
) ENGINE=InnoDB;

-- CRIANDO A TABELA DE RELAÇÃO PRODUTO X ARMAZEM
CREATE TABLE IF NOT EXISTS ProdArmazem(
  idProduto INT NOT NULL,
  idArmazem INT NOT NULL,
  qtd DECIMAL(14, 2) NOT NULL,
  PRIMARY KEY (idProduto, idArmazem),
  CONSTRAINT fk_pa_prod FOREIGN KEY (idProduto) REFERENCES Produto(id)
    ON UPDATE CASCADE ON DELETE RESTRICT,
  CONSTRAINT fk_pa_arm FOREIGN KEY (idArmazem) REFERENCES Armazem(id)
    ON UPDATE CASCADE ON DELETE RESTRICT,
  INDEX ix_pa_armazem (idArmazem)
) ENGINE=InnoDB;

-- Consulta: todos os armazéns com total ocupado
SELECT 
	a.id,
  a.nome,
  COALESCE(SUM(pa.qtd * p.tam),0) AS ocupado,
  a.espacoTotal                   AS capacidade,
  CASE WHEN a.espacoTotal - COALESCE(SUM(pa.qtd * p.tam),0) > 0
        THEN a.espacoTotal - COALESCE(SUM(pa.qtd * p.tam),0)
       ELSE 0
  END AS livre,
  CASE WHEN a.espacoTotal > 0
        THEN COALESCE(SUM(pa.qtd * p.tam), 0) / a.espacoTotal
      ELSE 0
  END AS ocupacao_pct         
FROM Armazem a 
LEFT JOIN ProdArmazem pa ON pa.idArmazem = a.id 
LEFT JOIN Produto p ON p.id = pa.idProduto
GROUP BY a.id, a.nome, a.espacoTotal
ORDER BY a.nome;

--5 armazéns com maior quantidade de um produto
DELIMITER $$
CREATE PROCEDURE cinco_armazens_produto (IN produto_id INT)
BEGIN
  SELECT
    a.id   AS armazem_id,
    a.nome AS armazem,
    pa.qtd AS quantidade,
    (pa.qtd * p.tam) AS ocupado
  FROM ProdArmazem pa
  JOIN Armazem a ON a.id = pa.idArmazem
  JOIN Produto p ON p.id = pa.idProduto
  WHERE pa.idProduto = produto_id
  ORDER BY pa.qtd DESC, a.nome
  LIMIT 5;
END $$
DELIMITER ;

CALL cinco_armazens_produto(1);

-- relatorio de produtos que estão em mais armazéns
SELECT
  p.id,
  p.descricao,
  COUNT(DISTINCT pa.idArmazem) AS qtd_armazens
FROM Produto p
LEFT JOIN ProdArmazem pa ON pa.idProduto = p.id
GROUP BY p.id, p.descricao
ORDER BY qtd_armazens DESC, p.descricao;

--consulta que mostre os produtos sem armazém alocados
SELECT
  p.id,
  p.descricao
FROM Produto p
LEFT JOIN ProdArmazem pa ON pa.idProduto = p.id
WHERE pa.idProduto IS NULL
ORDER BY p.descricao;


--relatório que mostra a lista de armazém com maior valor financeiro para empresa em ordem decrescente.
SELECT
  a.id,
  a.nome,
  COALESCE(SUM(pa.qtd * p.valor), 0) AS valor_total
FROM Armazem a
LEFT JOIN ProdArmazem pa ON pa.idArmazem = a.id
LEFT JOIN Produto p      ON p.id = pa.idProduto
GROUP BY a.id, a.nome
ORDER BY valor_total DESC, a.nome;