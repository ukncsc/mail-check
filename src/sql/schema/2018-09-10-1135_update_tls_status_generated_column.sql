ALTER TABLE `dmarc`.`dns_record_mx_tls_evaluator_results` 
DROP COLUMN `tls_status`;

ALTER TABLE `dmarc`.`dns_record_mx_tls_evaluator_results` 
ADD COLUMN `tls_status` INT(11) GENERATED ALWAYS AS (coalesce(greatest(cast(json_extract(`data`,'$.tls12AvailableWithBestCipherSuiteSelected.result') as signed),cast(json_extract(`data`,'$.tls12AvailableWithBestCipherSuiteSelectedFromReverseList.result') as signed),cast(json_extract(`data`,'$.tls12AvailableWithSha2HashFunctionSelected.result') as signed),cast(json_extract(`data`,'$.tls12AvailableWithWeakCipherSuiteNotSelected.result') as signed),cast(json_extract(`data`,'$.tls11AvailableWithBestCipherSuiteSelected.result') as signed),cast(json_extract(`data`,'$.tls11AvailableWithWeakCipherSuiteNotSelected.result') as signed),cast(json_extract(`data`,'$.tls10AvailableWithBestCipherSuiteSelected.result') as signed),cast(json_extract(`data`,'$.tls10AvailableWithWeakCipherSuiteNotSelected.result') as signed),cast(json_extract(`data`,'$.ssl3FailsWithBadCipherSuite.result') as signed),cast(json_extract(`data`,'$.tlsSecureEllipticCurveSelected.result') as signed),cast(json_extract(`data`,'$.tlsSecureDiffieHellmanGroupSelected.result') as signed),cast(json_extract(`data`,'$.tlsWeakCipherSuitesRejected.result') as signed),0))) VIRTUAL AFTER `last_checked`;

SET SQL_SAFE_UPDATES = 0;
UPDATE dns_record_mx_tls_evaluator_results SET data = REPLACE(data, '"Result"', '"result"') WHERE INSTR(data, '"Result"') > 0;
UPDATE dns_record_mx_tls_evaluator_results SET data = REPLACE(data, '"Description"', '"description"') WHERE INSTR(data, '"Description"') > 0;
SET SQL_SAFE_UPDATES = 1;

