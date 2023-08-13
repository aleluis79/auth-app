## Auth APP - Aplicación de ejemplo en c# webapi (dotnet7) con swagger y login en Keycloak

### Iniciar keycloak con docker:

./keycloak.sh

### Configurar keycloak:
- entrar a la url:
http://localhost:8080

- ingresar con el usuario admin y contraseña admin

- crear el realm prueba
- crear el client Prueba-Client
- crear un usuario
- configurar el client como Standar flow, Implicit flow y OAuth 2.0 Device Authorization Grant

### Crear un token

. ./get_token.sh

### Ejecutar el endpoint con curl

curl -i -X 'GET'  'http://localhost:5176/home/ping' -H "Authorization: Bearer $TOKEN"