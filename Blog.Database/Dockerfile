FROM mariadb:latest

# Create root password
ENV MARIADB_ROOT_PASSWORD secret

# Create user with password
ENV MARIADB_USER user
ENV MARIADB_PASSWORD password

# Create database
ENV MARIADB_DATABASE blog

# Create post table using script
ADD /CreateDatabase.sql /docker-entrypoint-initdb.d/

