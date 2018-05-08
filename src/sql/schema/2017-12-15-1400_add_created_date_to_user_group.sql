alter table `user` add column created_date datetime default CURRENT_TIMESTAMP;
alter table `group` add column created_date datetime default CURRENT_TIMESTAMP;
alter table `group_domain_mapping` add column created_date datetime default CURRENT_TIMESTAMP;
alter table `group_user_mapping` add column created_date datetime default CURRENT_TIMESTAMP;
