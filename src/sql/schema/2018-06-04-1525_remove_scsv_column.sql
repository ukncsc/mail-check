ALTER TABLE dns_record_mx_tls_profile_2 DROP test5_tls_version;
ALTER TABLE dns_record_mx_tls_profile_2 DROP test5_cipher_suite;
ALTER TABLE dns_record_mx_tls_profile_2 DROP test5_curve_group;
ALTER TABLE dns_record_mx_tls_profile_2 DROP test5_signature_hash_alg;
ALTER TABLE dns_record_mx_tls_profile_2 DROP test5_error;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test6_tls_version test5_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test6_cipher_suite test5_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test6_curve_group test5_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test6_signature_hash_alg test5_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test6_error test5_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test7_tls_version test6_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test7_cipher_suite test6_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test7_curve_group test6_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test7_signature_hash_alg test6_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test7_error test6_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test8_tls_version test7_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test8_cipher_suite test7_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test8_curve_group test7_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test8_signature_hash_alg test7_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test8_error test7_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test9_tls_version test8_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test9_cipher_suite test8_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test9_curve_group test8_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test9_signature_hash_alg test8_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test9_error test8_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test10_tls_version test9_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test10_cipher_suite test9_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test10_curve_group test9_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test10_signature_hash_alg test9_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test10_error test9_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test11_tls_version test10_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test11_cipher_suite test10_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test11_curve_group test10_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test11_signature_hash_alg test10_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test11_error test10_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test12_tls_version test11_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test12_cipher_suite test11_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test12_curve_group test11_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test12_signature_hash_alg test11_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test12_error test11_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test13_tls_version test12_tls_version int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test13_cipher_suite test12_cipher_suite int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test13_curve_group test12_curve_group int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test13_signature_hash_alg test12_signature_hash_alg int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_profile_2 CHANGE COLUMN test13_error test12_error int(11) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results DROP test5_result;
ALTER TABLE dns_record_mx_tls_evaluator_results DROP test5_description;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test6_result test5_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test6_description test5_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test7_result test6_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test7_description test6_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test8_result test7_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test8_description test7_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test9_result test8_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test9_description test8_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test10_result test9_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test10_description test9_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test11_result test10_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test11_description test10_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test12_result test11_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test12_description test11_description varchar(512) DEFAULT NULL;

ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test13_result test12_result int(11) DEFAULT NULL;
ALTER TABLE dns_record_mx_tls_evaluator_results CHANGE COLUMN test13_description test12_description varchar(512) DEFAULT NULL;
