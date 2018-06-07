ALTER TABLE dns_record_mx_tls_evaluator_results ADD COLUMN last_checked DATETIME NOT NULL;

SET SQL_SAFE_UPDATES = 0;

UPDATE dns_record_mx_tls_evaluator_results drmter
JOIN dns_record_mx_tls_profile_2 drmtp
ON drmter.mx_record_id = drmtp.mx_record_id
SET drmter.last_checked = drmtp.last_checked
WHERE drmter.mx_record_id = drmtp.mx_record_id
AND drmtp.end_date IS NULL;

SET SQL_SAFE_UPDATES = 1;
