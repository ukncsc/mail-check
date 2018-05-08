

DROP PROCEDURE IF EXISTS import_new_domains;



delimiter |

CREATE PROCEDURE import_new_domains()
BEGIN
INSERT INTO domain (name,monitor) 
 SELECT DISTINCT lcase(header_from) AS name ,b'0' AS monitor FROM record r LEFT JOIN domain ON domain.name = lcase(r.header_from) WHERE r.header_from_domain_id is null and domain.id IS NULL AND r.header_from IS NOT NULL
 UNION
 SELECT DISTINCT lcase(reported_domain) AS name ,b'0' AS monitor FROM forensic_report fr LEFT JOIN domain ON domain.name = lcase(fr.reported_domain) WHERE fr.reported_domain_id IS NULL AND domain.id IS NULL AND fr.reported_domain != "";
UPDATE record r SET header_from_domain_id = (SELECT id FROM domain WHERE name = lcase(r.header_from)) WHERE header_from_domain_id IS NULL;
UPDATE forensic_report fr SET reported_domain_id = (SELECT id FROM domain WHERE name = lcase(fr.reported_domain)) WHERE reported_domain_id IS null;
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