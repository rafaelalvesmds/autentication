# Autentication

# OAuth 2

- OAuth 2 é um protocolo aberto que permie autorizacao de forma simples e padronizada entre aplicações web, mobile e aplicações desktop;
- OAuth 2 utiliza um acess token e a aplicação cliente usa esse token para acessar uma API ou endpoits;
- OAuth 2 determina como os endpoits serão usados em diferentes tipos de aplicações;

# OpenID Connect

- OpenID Connect é simplesmente uma camada de identificação no topo do protocolo OAuth 2;
- Através do OpenID Connect uma aplicação pode receber um Identity Token além de um access token se for o caso;
- OpenID Connect define como os diferentes tipos de aplicações cliente podem obter de forma segura um token do Identity Server;

# Identity Server 5 - Duende Identity Server

- https://duendesoftware.com/products/identityserver
- Identity Server é uma implementação do OpenID Connect e OAuth 2 e é altamente otimizado para resolver problemas de segurança comuns em aplicações atuais, sejam elas mobile, nativas ou mesmo aplicações web.

![image](https://user-images.githubusercontent.com/84939473/151229112-5edff638-4cb4-4287-ac8a-72b2241a54c1.png)

- Client: É um componente de software que requisita um token à um Identity Server - as vezes para autenticar um usuário ou para acessar um recurso;
- API Resource: Normalmente representam uma funcionalidade que um client precisa invocar - normalmente implementados através de Web API's;
- Identity Resource (Claims): Informações relacionadas à identidade do usuário. Ex: nome, e-mail etc;

- A resposta à um processo de autenticação;
- Access Token: Possibilita o acesso do usuário a um API Resource.

# JSON Web Token (JWT)

![image](https://user-images.githubusercontent.com/84939473/151230237-a82f8f3a-6073-4224-914b-a11c5cd111ab.png)
![image](https://user-images.githubusercontent.com/84939473/151230408-5ea5d9a8-2095-409f-bbe6-1fdae4e2b049.png)

----------------------------------------------------------------------------------------------------------------
Install Duende
````bash
dotnet new --install Duende.IdentityServer.Templates
````

````bash
dotnet new isui
````
