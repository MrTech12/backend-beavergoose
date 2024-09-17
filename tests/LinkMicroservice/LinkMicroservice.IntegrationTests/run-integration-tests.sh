#!/bin/bash

# Script workflow
# 1. Spin up a PostgreSQL Docker environmnet
# 2. Wait for PostgreSQL to boot up
# 3. Run the Integration tests

printf "Message: Executing helper script for running Integration tests. \n"

printf "Message: Going to the working directory of the Integration tests."
cd $1
printf '\n'

printf "Message: New working directory: "
echo $PWD
printf '\n'

printf "Message: Running Docker compose file to pull up & boot up PostgreSQL container. \n"
printf "Message: Command that is executed: 'docker compose up -d' \n"
docker compose up -d
printf "\n"

printf "Message: Waiting for PostgreSQL to boot up \n"
printf "Message: Command that is executed: 'docker logs postgresql' \n"
until [[ $(docker logs postgresql) == *"database system is ready to accept connections"* ]]
do
  printf "Message: still waiting on PostgreSQL to boot..."
  sleep 5s
done
printf "\n"

printf "Message: The environment is ready. Time to run the integration tests. \n"
printf "Message: Command that executed: {dotnet test --logger 'trx;LogFileName=test-results.trx' --collect:'XPlat Code Coverage'} \n"
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"
printf "\n"