#!/bin/bash

# Script workflow
# 1. Spin up a Localstack Docker environmnet
# 2. Wait for Localstack to boot up
# 3. Make requests to Localstack to check the status
# 4. Configure an S3 bucket
# 4. Run the Integration tests

printf "Message: Executing helper script for running Integration tests. \n\n"

printf "Message: Going to the working directory of the Integration tests."
cd $1
printf '\n'

printf "Message: New working directory: "
echo $PWD
printf '\n'

printf "Message: Running Docker compose file to pull up & boot up Localstack container. \n"
printf "Message: Command that is executed: 'docker-compose up -d' \n"
docker-compose up -d
printf "\n"

printf "Message: Waiting for Localstack to boot up \n"
printf "Message: Command that is executed: 'docker logs localstack' \n"
until [[ $(docker logs localstack) == *"Ready."* ]]
do
  printf "Message: still waiting on Localstack to boot..."
  sleep 30s
done
printf "\n"

printf "Message: Making HTTP requests to check the status of Localstack.\n"
printf "Message: Command that is executed: 'curl http:/localhost:4566' \n"
RESULT=$(curl http://localhost:4566/)
if [[ "$RESULT" == *"running"* ]]; then
  printf "localstack is running \n"
fi
printf "\n"

printf "Message: Configuring the AWS CLI with dummy data. \n"
printf "Message: Command one that is executed: aws configure set aws_access_key_id AAAAA \n"
printf "Message: Command two that is executed: aws configure set aws_secret_access_key AAAAA \n"
printf "Message: Command three that is executed: aws configure set default.region us-west-2 \n"
aws configure set aws_access_key_id AAAAA
aws configure set aws_secret_access_key AAAAA
aws configure set default.region us-west-2
printf "\n"

printf "Message: Localstack is running, now configuring a new S3 Bucket. \n"
printf "Message: Command one that is executed: 'aws --endpoint-url=http://localhost:4566 s3 mb s3://test-bucket' \n"
aws --endpoint-url=http://localhost:4566 s3 mb s3://test-bucket
printf "\n"
printf "Message: Command two that is executed: 'aws --endpoint-url=http://localhost:4566 s3api put-bucket-acl --bucket test-bucket --acl public-read' \n" 
aws --endpoint-url=http://localhost:4566 s3api put-bucket-acl --bucket test-bucket --acl public-read
printf "\n"

printf "Message: The environment is ready. Time to run the integration tests. \n"
printf "Message: Command that executed: {dotnet test --logger 'trx;LogFileName=test-results.trx' --collect:'XPlat Code Coverage'} \n"
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"
printf "\n"