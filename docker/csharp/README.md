Aplicação em C#
===============

Esta aplicação em C# é um pequeno exemplo para utilizar em Docker e Kubernetes, pois além de utilizar variáveis de ambiente, ela se comunica com um banco de dados MySQL.

Provisionar
-----------

Para provioinar esta aplicação precisaremos primeiro de um banco de dados compatível com MySQL:

```bash
docker container run -d --name mariadb \
-e MYSQL_ROOT_PASSWORD=Abc123_ \
-e MYSQL_USER=container \
-e MYSQL_PASSWORD=d0ck3r \
-e MYSQL_DATABASE=container \
-v dados:/var/lib/mysql mariadb
```

> **Observação:** Para os exemplos abaixo, o endereço IP do banco foi considerado como 172.17.0.2.

Com o banco no ar podemos iniciar um contêiner com a aplicação montada em um volume e instalar as dependências. Note que a porta que precisamos expor precisa ser a porta indicada no arquivo `Properties/launchSettings.json`:

```bash
git clone https://github.com/Cloud3-SA/unimed-dockerk8s dockerk8s
cd dockerk8s/apps/csharp
docker container run -dti --name app -v $PWD:/opt/app -p 8080:8080 alpine sh
apk add dotnet8-sdk
cd /opt/app
DB_HOST=172.17.0.2 DB_USER=container DB_PASS=d0ck3r DB_DATABASE=container dotnet watch
```
