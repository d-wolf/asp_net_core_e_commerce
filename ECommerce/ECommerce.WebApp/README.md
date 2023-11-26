# ASP.NET Core MVC WebApp 
A Generic ASP.NET Core E-Commerce Web App.

## Geting Started
[here](https://learn.microsoft.com/en-us/aspnet/core/getting-started/)

### Build
* run with hot reload `dotnet watch`
* run a profile from [launchSettings.json](ECommerce/ECommerce.WebApp/Properties/launchSettings.json) with `dotnet run --launch-profile "<PROFILE_NAME>"` e.g. `dotnet run --launch-profile "http"`

## DB
* uses postgresql db
* [install](https://www.postgresql.org/)
* DB name: `ecommerce`
* DB user: `postgres`
* DB user pw: `postgres`
* create db `CREATE DATABASE ecommerce OWNER postgres;`

## EF Core Migration
To make migrations work in multi project solution go through [this guide](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli) first.

1. add migration `dotnet ef migrations add <MIGRATION_NAME> -o Data/Migrations --project ../ECommerce.DataAccess`
2. create schema from migration `dotnet ef database update`

## Server Deployment
### Ubuntu 20.04 LTS
#### Publish & Update
* run `dotnet publish --configuration Release` in project dir 
* copy the files to the server `scp -r bin/Release/net8.0/publish/* username@remotecomputer:/var/www/ecommerce` locally
* login to the server & [install the .NET runtime](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-2004)

#### Install & Configure Postgresql
* (the following steps will all be on the server side)
* [howto](https://ubuntu.com/server/docs/databases-postgresql)
* intall `apt install postgresql`
* check status `service postgresql status`
* login `sudo -u postgres psql`
* change password for username postgres with `ALTER USER postgres with encrypted password 'postgres';`
* create db `CREATE DATABASE ecommerce OWNER postgres;`
* `sudo -u postgres psql`
* `\i /var/www/ecommerce/script.sql`

#### Miragtion
* create migration scrip `dotnet ef migrations script FromA FromB -o script.sql` or `dotnet ef migrations script -o script.sql`
* copy the file to the server `scp -r script.sql username@remotecomputer:/var/www/ecommerce`
* run the script `psql -U postgres -d ecommerce -a -f script.sql`

#### Install & Configure Nginx
* (the following steps will all be on the server side)
* [install nginx](https://www.nginx.com/resources/wiki/start/topics/tutorials/install/#official-debian-ubuntu-packages)
* get nginx status `service nginx status`
* run nginx `service nginx start`
* when we open the browser and navigate to the server ip, the nginx start page should be shown
* go to `cd /etc/nginx`
* `mkdir sites-available`
* `mkdir sites-enabled`
* `nano /etc/nginx/sites-available/default`
* add the following and save
```

server {
    listen        80;
    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```
* add symlink `sudo ln -s /etc/nginx/sites-available/default /etc/nginx/sites-enabled/default`
* `nano nano /etc/nginx/nginx.conf`
* add the line `include       /etc/nginx/sites-enabled/*;` to `http {}` and save
* `sudo nginx -s reload`
* run `sudo nginx -t` to verify the configuration
* reload nginx with `sudo nginx -s reload` to apply the changes

#### Test the site
* run `dotnet /var/www/ecommerce/ECommerce.WebApp.dll` on the server
* when we open the browser and navigate to the server ip the startpage of the site should be shown

#### Setup service
* ssh to server and run `chown -R www-data:www-data /var/www`
* run `scp -r ecommerce-app username@remotecomputer:/etc/systemd/system` on the local machine in project directory
* run `systemctl enable ecommerce-app.service` on server to enable the service