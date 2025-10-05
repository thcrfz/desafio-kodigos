CREATE TABLE if NOT EXISTS Produto(
  id INT PRIMARY KEY,
  descricao NVARCHAR(255) NOT NULL,
  um VARCHAR(10),
  valor DECIMAL(12,2),
  tam DECIMAL(12,2)
);

CREATE TABLE IF NOT EXISTS Armazem(
  id INT PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  espacoTotal DECIMAL(14,2)
);

CREATE TABLE IF NOT EXISTS ProdArmazem(
  idProduto INT NOT NULL,
  idArmazem INT NOT NULL,
  qtd DECIMAL(14, 2) NOT NULL,
  PRIMARY KEY (idProduto, idArmazem),
  FOREIGN KEY (idProduto) REFERENCES Produto(id),
  FOREIGN KEY (idArmazem) REFERENCES Armazem(id)
);


SELECT 
	a.id,
    a.nome,
    COALESCE(SUM(pa.qtd * p.tam),0) AS ocupado,
    a.espacoTotal AS capacidade,
    CASE
    	WHEN a.espacoTotal - COALESCE(SUM(pa.qtd * p.tam),0) > 0
        	THEN a.espacoTotal - COALESCE(SUM(pa.qtd * p.tam),0)
            ELSE 0
   END AS livre,
   CASE
    WHEN a.espacoTotal > 0
      THEN COALESCE(SUM(pa.qtd * p.tam), 0) / a.espacoTotal
      ELSE 0
   END AS ocupacao_pct         
FROM Armazem a 
LEFT JOIN ProdArmazem pa ON pa.idArmazem = a.id 
LEFT JOIN Produto p ON p.id = pa.idProduto
GROUP BY a.id, a.nome, a.espacoTotal
ORDER BY a.nome;

DELIMITER $$

CREATE PROCEDURE Top5ArmazensPorProduto(IN p_produto_id INT)
BEGIN
  SELECT
    a.id,
    a.nome,
    SUM(pa.qtd) AS total_qtd
  FROM ProdArmazem pa
  JOIN Armazem a ON a.id = pa.idArmazem
  WHERE pa.idProduto = p_produto_id
  GROUP BY a.id, a.nome
  ORDER BY total_qtd DESC, a.nome ASC
  LIMIT 5;
END$$

DELIMITER ;