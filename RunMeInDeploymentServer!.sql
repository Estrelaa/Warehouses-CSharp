CREATE INDEX CompanyID ON gcp(gcp_cd);
CREATE INDEX nameAtThisWarehouse ON em(name, w_id);
CREATE INDEX productComanyLink ON gtin(p_id, gcp_cd);
CREATE INDEX ProductIDAndNAme ON gtin(p_id, gcd_cd);
CREATE INDEX ProductToWarehouse ON stock(w_id, p_id);