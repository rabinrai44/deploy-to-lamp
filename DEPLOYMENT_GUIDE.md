# DEPLOYMENT GUIDE

## LINUX SERVER SETUP USING A NEWLY CREATED DIGITAL OCEAN LAMP SERVER

## Prerequests (Required)

- Have an account with [Digital Ocean](https://digitalocean.com) - if you haven't create one.
- Have a newly created droplet with Ubuntu 20.04 (Linux) OS

## Update/Upgrade Package

Update the system package to ensure you are installing the latest MySQL release.

- Connect/Lunch a your linux droplet from dashboard or with ssh
- With SSH follow commands below:

    ```
        ssh root@ipaddressOfLinuxServer
        sudo apt update
    ```

## Install MySql
    - Enter the following command
    ```
    sudo apt install mysql-server
    ```

## Connect & Setup/Securing MySql

1. Secure your MySQL user account with password authentication by running the included security script:

    ```
    root@yourDroplet:~# sudo mysql
    ```

2. Create a new database user (follow the following commands)
    - Enter the following commands
   ```
    CREATE USER 'appuser'@'localhost' IDENTIFIED BY 'Pa$$w0rd';
    GRANT ALL PRIVILEGES ON *.* TO 'appuser'@'localhost' WITH GRANT OPTION;
    FLUSH PRIVILEGES;
    ```
    - Exist mysql enter `exit` and press enter

4. Install the dotnet runtime - follow instructions from [here](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu)
5. Configure Apache
    Enter the following two commands

    ```
    a2enmod proxy proxy_http proxy_html rewrite

    apt install apache2 (only if not install already or ask by system)

    systemctl restart apache2
    ```

6. Configure the virtual host

    ```
    sudo nano /etc/apache2/sites-available/deploytolinux.conf
    ```

7. Copy and paste below text over into editor

    ```
    <VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://127.0.0.1:5000/
    ProxyPassReverse / http://127.0.0.1:5000/

    ErrorLog /var/log/apache2/deploytolinux-error.log
    CustomLog /var/log/apache2/deploytolinux-access.log common

    </VirtualHost>
    ```

8. Enable the site

```
a2ensite deploytolinux
```

9. Disable the default Apache site:

    ```
    a2dissite 000-default
    ```
10. Restart apache
    ```
    systemctl reload apache2
    ```
11. Add the deploy.reloaded extension to VS Code
12. Add a settings.json file to the .vscode folder and add the following:

    ```
    {
        "deploy.reloaded": {
            "packages": [
                {
                    "name": "Version 1.0.0",
                    "description": "Package version 1.0.0",

                    "files": [
                        "publish/**"
                    ]
                }
            ],

            "targets": [
                {
                    "type": "sftp",
                    "name": "Linux",
                    "description": "SFTP folder",

                    "host": "IP Address", "port": 22,
                    "user": "root", "password": "Your Linux password",

                    "dir": "/var/deploytolinux",
                    "mappings": {
                        "publish/**": "/"
                    }
                }
            ]
        }
    }
    ```

13. Publish the dotnet application locally from the solutions folder:
    - update the appsettings.json file and change the ApiUrl to match your server IP address e.g:

    `"ApiUrl": "http://128.199.203.224/Content/"`
    - `dotnet publish -c Release -o publish yourappname.sln`
14. Deploy the package using deploy reloaded

## == Back to the Linux server ==
15. Restart the journalctl service as this has been not working on fresh installs and is very useful to get information about the service:

    ```
    systemctl restart systemd-journald
16. Set up the service that will run the kestrel web server
   - Enter the following command
    ```
    sudo nano /etc/systemd/system/deploytolinux-web.service
    ```
    - Paste in the following:
    ```
    [Unit]
    Description=Kestrel service running on Ubuntu 20.04
    [Service]
    WorkingDirectory=/var/deploytolinux
    ExecStart=/usr/bin/dotnet /var/deploytolinux/API.dll
    Restart=always
    RestartSec=10
    SyslogIdentifier=deploytolinux
    User=www-data
    Environment=ASPNETCORE_ENVIRONMENT=Production
    Environment='Token__Key=CHANGE ME TO SOMETHING SECURE'
    Environment='Token__Issuer=https://deploytolinux'
    [Install]
    WantedBy=multi-user.target

    ```

17. Enable and start web services

    ```
    sudo systemctl enable deploytolinux-web.service
    sudo systemctl start deploytolinux-web.service
    ```
18. Ensure the server is running:
    `netstat -ntpl` (if not install install net-tools `apt install net-tools`)

19. Check the journal logs
    Enter the following commands:
    ```
    journalctl -u deploytolinux-web.service
    journalctl -u deploytolinux-web.service | tail -n 300
    journalctl -u deploytolinux-web.service --since "5 min ago"
    ```
