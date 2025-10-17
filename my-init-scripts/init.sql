CREATE DATABASE test;

use test;

CREATE TABLE tbPatients 
(
	id varchar(36) NOT NULL PRIMARY KEY, 
	json text, 
	birthDate datetime
);