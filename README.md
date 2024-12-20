# API de Autenticación LDAP

## Configuración Inicial
1. Clona el repositorio.
2. Configura las credenciales LDAP en `appsettings.json`.
3. Tener en cuenta si en el LDAP hay 'organizationUnits'.

## Ejemplo de Configuración de Servidor LDAP
Puedes usar un contenedor Docker con OpenLDAP. En esta configuración se uso OpenLDAP for Windows, versión 2.8 del 17 de agosto de 2003

* Archivo de configuración de slapd.conf
```bash
# LDIF Backend configuration file
# See slapd.conf(5) for details on configuration options.
# This file should NOT be world readable.
ucdata-path	./ucdata
include		./schema/core.schema
include		./schema/cosine.schema
include		./schema/nis.schema
include		./schema/inetorgperson.schema
include		./schema/openldap.schema
include		./schema/dyngroup.schema

pidfile		./run/slapd.pid
argsfile	./run/slapd.args

# Enable TLS if port is defined for ldaps
TLSVerifyClient never
TLSCipherSuite ECDHE-RSA-AES256-SHA384:AES256-SHA256:!RC4:HIGH:!MD5:!aNULL:!EDH:!EXP:!SSLV2:!eNULL
TLSProtocolMin 3.3
TLSCertificateFile ./secure/certs/maxcrc.cert.pem
TLSCertificateKeyFile ./secure/certs/maxcrc.key.pem
TLSCACertificateFile ./secure/certs/maxcrc.cert.pem

#######################################################################
# ldif database definitions
#######################################################################

database	ldif
directory ./ldifdata
suffix		"dc=maxcrc,dc=com"
rootdn		"cn=Manager,dc=maxcrc,dc=com"
# Cleartext passwords, especially for the rootdn, should
# be avoid.  See slappasswd(8) and slapd.conf(5) for details.
# Use of strong authentication encouraged.
rootpw    {SSHA}l+MoymkISf6NMSDnltPl+PRAnjocIXIO
```
## Ejecutar el servidor LDAP en Windows desde una ventana del Símbolo del Sistema
```bash
net start OpenLDAP~Service
```
## Comandos para Ejecutar la API
```bash
dotnet build
dotnet run
```
___	
## Métodos
### Obtener el token
#### Endpoint
```bash
https://localhost:xxxx/api/token
```
##### Metodo
* GET
##### Parámetros
* Subject
* SecretKey
##### Request
```bash
{
  "Subject": "baseWebApiSubject",
  "SecretKey": "your-secret-key-in-base64-encode"
}
```
##### Responses
Algunas de las posibles respuestas:
* 200:
  ```bash
  {
    "status": "success",
    "message": "Token successfully generated",
    "result": {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiYXNlV2ViQXBpU3ViamVjdCIsImp0aSI6ImM5NWMzZjE5LWE0ZDAtNDYxNy04MjFlLTIyZmVjN2FjNmY4YyIsImlhdCI6IjIwLzEyLzIwMjQgMTI6MTc6NDMgYS4gbS4iLCJzdWJqZWN0IjoiYmFzZVdlYkFwaVN1YmplY3QiLCJrZXkiOiJPVEpqTVdZeE1XWmpNekF4TmpSa01tSmhPVGt5TVRsaU9HVXlPREZpWW1NPSIsImV4cGlyZSI6IjE5LzEyLzIwMjQgMDg6MTc6NDMgcC4gbS4iLCJleHAiOjE3MzQ2NTc0NjMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMzUvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAzNSJ9.LnVMFWhdfribZrRacv8vCN7DG6iFmaYf1CKTdvuT1hA",
        "expires": "19/12/2024 08:17:43 p. m."
    }
  }
  ```
* 400: En caso de que el request este mal escrito, o que falte uno de los parámetros
  ```bash
  {
    "status": "error",
    "message": "Invalid subject or secret key"
  }
  ```
* 500: Error en el servidor

___	
### Autenticar ante LDAP
#### Endpoint
```bash
https://localhost:xxxx/api/authenticate
```
##### Metodo
* POST
##### Autenticación
* Bearer Token
##### Parámetros
* Username
* Password
##### Request
```bash
{
    "Username": "usuario.uno",
    "Password": "A6v-f+"
}
```
##### Responses
Algunas de las posibles respuestas:
* 200:
  ```bash
  {
    "status": "success",
    "user": {
        "username": "usuario.uno",
        "email": "usuario.uno@example.com",
        "displayName": "Usuario Uno",
        "department": "IT",
        "title": "Software Engineer"
    }
  }
  ```
* 400: En caso de que el request este mal escrito, o que falte uno de los parámetros
  ```bash
  {
    "status": "error",
    "message": "Invalid credentials or user not found."
  }
  ```
* 500: Error en el servidor
