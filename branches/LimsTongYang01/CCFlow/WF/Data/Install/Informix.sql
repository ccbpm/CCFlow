create table dual(onerow int);
insert into dual(onerow) values(1);
select * from dual;
--3.创建MySubString函数（三个参数）；
CREATE FUNCTION MySubString(SourceString lvarchar(1000), start integer, length integer)
RETURNING lvarchar(1000);
	DEFINE TargetString lvarchar(1000);
	LET TargetString = 
	(
		SELECT SUBSTRING(SourceString FROM start FOR length) 
		FROM dual
	);
	RETURN TargetString;
END FUNCTION;

select MySubString('123456789', 2, 4)
from dual;

--4.创建MySubString函数（两个参数）；
CREATE FUNCTION MySubString(SourceString lvarchar(1000), start integer)
RETURNING lvarchar(1000);
	DEFINE TargetString lvarchar(1000);

	LET TargetString = 
	(
		SELECT SUBSTRING(SourceString FROM start FOR 1000) 
		FROM dual
	);

	RETURN TargetString;
END FUNCTION;
select MySubString('123456789', 2)
from dual;
