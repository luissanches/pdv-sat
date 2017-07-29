# PDV-SAT

### Descrição ###
Ponto de Venda de Produtos com integração SAT.
Transmite as informações das operações comerciais dos contribuintes varejistas do estado de São Paulo,
utilizando Internet, através de equipamento SAT.
 



























### Setup for development ###

```
#!bash
$ npm install
```

### Migrations For Relational Database ###
Utilizamos [knex.js](http://knexjs.org/#Migrations-CLI) com Postgresql. 
Para processar as migrations, installe o CLI do knex e depois o seguinte comando
IMPORTANTE: Development está apontando para base de dados local, 
já o Stage é base onde estamos homologando o sistema. 

[Knex Install]
```
npm install knex -g
knex migrate:latest
knex seed:run
```

[Knexfile Sample]
```
development: {
    client: 'postgresql',
    connection: {
      host: 'localhost',
      port: 5432,
      database: 'legisnote',
      user: 'admin',
      password: 'spread123'
    },
    pool: {
      min: 2,
      max: 10
    },
    migrations: {
      tableName: 'migrations',
      directory: './knex/migrations/dev/'
    },
    seeds: {
      directory: './knex/seeds/dev/'
    }
  },stage: {
    client: 'postgresql',
    connection: {
      host: '10.10.2.211',
      port: 5432,
      database: 'legisnote',
      user: 'admin',
      password: 'spread123'
    },
    pool: {
      min: 2,
      max: 10
    },
    migrations: {
      tableName: 'migrations',
      directory: './knex/migrations/dev/'
    },
    seeds: {
      directory: './knex/seeds/dev/'
    }
  },

```

### Documents Database ###
Utilizamos o [MongoDB](https://mongodb.com/) para armazenar os dados de sessão do usuário.

[Development]
mongodb: {
    host: 'localhost:27017',
    name: 'legisnote',
    debug: true
}

[Stage]]
mongodb: {
    host: '10.10.2.211:27017',
    name: 'legisnote',
    debug: false
}

### Run ###

```
#!bash
$ npm start #production
```

```
#!bash
$ npm run dev #development
```

### Standards ###
Estamos utilizando [eslint](https://www.npmjs.com/package/eslint) com os padrões do [npm standard](https://www.npmjs.com/package/standard), então é possível verificar o padrão no próprio doc do package.

### Tests ###
Estamos utilizando [mocha](https://mochajs.org/) para a cobertura de testes
O arquivo responsável por chamar os suites de teste ./test.js
Ao rodar npm test, ele será o intermediário dos testes, ou podemos chamar arquivos específicos pelo CLI do mocha.

```
cd .
npm test 
(ou  mocha --compilers js:babel-core/register test/*.test.js)
```

### Stage Endpoint ###
[ec2-54-232-216-88.sa-east-1.compute.amazonaws.com](ec2-54-232-216-88.sa-east-1.compute.amazonaws.com)

### Deploy ###
Copie sua chave pública 
```
#!bash
cat ~/.ssh/id_rsa.pub
```

Adicione sua chave na ultima linha do arquivo "authorized_keys" no server
```
#!bash
vim ~/.ssh/authorized_keys
```

Configure o remote no seu git local
```
#!bash
git remote add staging ubuntu@ec2-54-232-216-88.sa-east-1.compute.amazonaws.com:~/psf
git push staging master
```


#Production#
```
#!bash

cd
sudo locale-gen pt_BR.UTF-8
sudo apt-get update
sudo apt-get install -y git nginx build-essential libssl-dev

#Install NVM (Node Js versioning)
curl -o- https://raw.githubusercontent.com/creationix/nvm/v0.33.2/install.sh | bash
source .bashrc

#Install Node 7.7.3
nvm install v7.7.3

#Install PM2 (Lifecycle Node Js process manager)
npm install pm2 -g

#Install Knex (For Database access)
npm install knex -g

#Install MongoDB (Authentication tokens handler)
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv EA312927
echo "deb http://repo.mongodb.org/apt/ubuntu xenial/mongodb-org/3.2 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-3.2.list
sudo apt-get update
sudo apt-get install -y mongodb-org

#Setup MongoDB to autostart
#Open the file
sudo vim /etc/systemd/system/mongodb.service

#Paste in, save and close
[Unit]
Description=High-performance, schema-free document-oriented database
After=network.target

[Service]
User=mongodb
ExecStart=/usr/bin/mongod --quiet --config /etc/mongod.conf

[Install]
WantedBy=multi-user.target

#Start MongoDB
sudo systemctl start mongodb

#Enable MongoDB to autostart
sudo systemctl enable mongodb

#Add symbolic links for global use node and npm
sudo ln -s /home/ubuntu/.nvm/versions/node/v7.7.3/bin/node /usr/bin/node
sudo ln -s /home/ubuntu/.nvm/versions/node/v7.7.3/bin/npm /usr/bin/npm
sudo ln -s /home/ubuntu/.nvm/versions/node/v7.7.3/bin/knex /usr/bin/knex

```