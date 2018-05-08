ALTER TABLE domain ALTER publish SET DEFAULT b'0';

update domain set publish = b'0';

update domain set publish=b'1' where exists (select 1 from group_domain_mapping gdm where domain.id=gdm.domain_id);