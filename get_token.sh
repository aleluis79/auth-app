#!/bin/sh
export TOKEN=$(curl -s --data "grant_type=password&client_id=Prueba-Client&username=alejandro&password=123456&client_secret=04W37ykgye3Q2TUDVZyvzY3KqMHT95oG" localhost:8080/realms/prueba/protocol/openid-connect/token | jq -r .access_token)
