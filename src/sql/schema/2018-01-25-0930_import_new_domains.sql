

DROP PROCEDURE IF EXISTS import_new_domains;



delimiter |

CREATE PROCEDURE import_new_domains()
BEGIN
INSERT INTO domain (name,monitor) 
 SELECT DISTINCT lcase(header_from) AS name ,b'0' AS monitor FROM record r LEFT JOIN domain ON domain.name = lcase(r.header_from) WHERE r.header_from_domain_id is null and domain.id IS NULL AND r.header_from IS NOT NULL;
UPDATE record r SET header_from_domain_id = (SELECT id FROM domain WHERE name = lcase(r.header_from)) WHERE header_from_domain_id IS NULL;

END |

delimiter ;

DROP EVENT IF EXISTS import_new_domains_daily;

delimiter |

CREATE EVENT import_new_domains_daily
  ON SCHEDULE
    EVERY 1 DAY
DO BEGIN
CALL import_new_domains();
END |

delimiter ;
CALL import_new_domains();