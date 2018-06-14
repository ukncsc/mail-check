SET SQL_SAFE_UPDATES = 0;

DELETE FROM certificate_mapping;
DELETE FROM certificate;

ALTER TABLE certificate DROP issuer;
ALTER TABLE certificate DROP subject;
ALTER TABLE certificate DROP start_date;
ALTER TABLE certificate DROP end_date;
ALTER TABLE certificate DROP key_length;
ALTER TABLE certificate DROP algorithm;
ALTER TABLE certificate DROP serial_number;
ALTER TABLE certificate DROP version;
ALTER TABLE certificate DROP valid;

ALTER TABLE certificate ADD COLUMN raw_data BLOB NULL AFTER `thumb_print`;

SET SQL_SAFE_UPDATES = 1;


