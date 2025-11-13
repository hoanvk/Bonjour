```sql
CREATE USER 'bonjour'@'%' IDENTIFIED BY 'changeit';
create database bonjour;
GRANT ALL PRIVILEGES ON bonjour.* TO 'bonjour'@'%';
```
