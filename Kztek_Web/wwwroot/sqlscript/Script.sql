

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbl_Controller' AND COLUMN_NAME = 'Inactive')
BEGIN
	ALTER TABLE [tbl_Controller] ADD Inactive bit not null default(0)
END