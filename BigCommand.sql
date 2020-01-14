/* SELECT * FROM gtin JOIN stock ON gtin.p_id = stock.p_id AND stock.hld < gtin.l_th JOIN gcp ON gtin.gcp_cd = gcp.gcp_cd */

SELECT * FROM stock 
JOIN gtin ON gtin.p_id = stock.p_id AND stock.hld < gtin.l_th AND NOT gtin.ds = 1 
JOIN gcp ON gtin.gcp_cd = gcp.gcp_cd where stock.w_id = 1