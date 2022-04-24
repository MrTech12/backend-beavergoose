#!/bin/bash

# Script workflow
# 1. Spin up a Localstack Docker environmnet
# 2. Wait for Localstack to boot up
# 3. Make requests to Localstack to check the status
# 4. Configure an S3 bucket
# 4. Run the Integration tests

who="qwerty"
printf "Hello, $who! \n\n"

printf "Executing helper script for running Integration tests. \n"

printf "\n"

printf "Running Docker compose file to pull up & boot up Localstack container. \n"
printf "Command that is executed: 'docker-compose up' \n"
# echo
printf "\n"

printf "Waiting for Localstack to be pulled down & to boot up... \n"
printf "Command that is executed: 'sleep 30' \n"
sleep 5s
printf "\n"

printf "Making HTTP requests to check the status of Localstack.\n"
printf "Command that is executed: 'curl http:/localhost:4566' \n"
# running curl
printf "\n"

printf "Localstack is running, now configuring a new S3 Bucket. \n"
printf "Command one that is executed: 'aws --endpoint-url=http://localhost:4566 s3 mb s3://test-bucket' \n"
# echo
printf "\n"
printf "Command two that is executed: 'aws --endpoint-url=http://localhost:4566 s3api put-bucket-acl --bucket test-bucket --acl public-read' \n" 
# echo
printf "\n"

printf "The environment is ready. Time to run the integration tests. \n"
printf "Command that executed: {dotnet test $1 --logger 'trx;LogFileName=test-results.trx' --collect:'XPlat Code Coverage'} \n"
# echo
printf "\n"