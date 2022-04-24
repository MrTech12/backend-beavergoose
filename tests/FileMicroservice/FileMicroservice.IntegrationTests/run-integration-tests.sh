#!/bin/bash

# Script workflow
# 1. Spin up a Localstack Docker environmnet
# 2. Wait for Localstack to boot up
# 3. Make requests to Localstack to check the status
# 4. Configure an S3 bucket
# 4. Run the Integration tests

printf "Executing helper script for running Integration tests. \n"
printf "\n"

printf "Going to the working directory of the Integration tests."
cd $1
printf '\n'

printf "New working directory: "
echo $PWD
printf '\n'

printf "Running Docker compose file to pull up & boot up Localstack container. \n"
printf "Command that is executed: 'docker-compose up -d' \n"
docker-compose up -d
printf "\n"

printf "Waiting for Localstack to be pulled down & to boot up, for 4.5 minutes \n"
printf "Command that is executed: 'sleep 4.5m' \n"
sleep 4.5m
printf "\n"

printf "Making HTTP requests to check the status of Localstack.\n"
printf "Command that is executed: 'curl http:/localhost:4566' \n"

RESULT=$(curl http://localhost:4566/)
if [[ "$RESULT" == *"running"* ]]; then
  printf "localstack is running \n"
fi
printf "\n"

printf "Configuring the AWS CLI with dummy data. \n"
printf "Command one that is executed: aws configure set aws_access_key_id AAAAA \n"
printf "Command two that is executed: aws configure set aws_secret_access_key AAAAA \n"
printf "Command three that is executed: aws configure set default.region us-west-2 \n"
aws configure set aws_access_key_id AAAAA
aws configure set aws_secret_access_key AAAAA
aws configure set default.region us-west-2
printf "\n"

printf "Localstack is running, now configuring a new S3 Bucket. \n"
printf "Command one that is executed: 'aws --endpoint-url=http://localhost:4566 s3 mb s3://test-bucket' \n"
aws --endpoint-url=http://localhost:4566 s3 mb s3://test-bucket
printf "\n"
printf "Command two that is executed: 'aws --endpoint-url=http://localhost:4566 s3api put-bucket-acl --bucket test-bucket --acl public-read' \n" 
aws --endpoint-url=http://localhost:4566 s3api put-bucket-acl --bucket test-bucket --acl public-read
printf "\n"

printf "The environment is ready. Time to run the integration tests. \n"
printf "Command that executed: {dotnet test --logger 'trx;LogFileName=test-results.trx' --collect:'XPlat Code Coverage'} \n"
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"
printf "\n"