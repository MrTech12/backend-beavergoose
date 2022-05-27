#!/bin/bash

# Script Explanation
# The 'FileService' class expects the presence of an 'appsettings.json' file to retrieve values 
# for connecting with a file storage service.
# During the tests, the logic for retrieving these values gets called. The file that contains the values
# is not stored in source control.
# As a result, the test break because of it.
# This script create a mock 'appsettings.json' file with fake values, to make the tests run through the regular logic
# without stumbling onto a non existent file.

printf "Message: Executing script for creating 'appsettings.json' file to running FileMicroservice tests. \n\n"
printf "Message: Creating the file... \n"

cat > appsettings.json << ENDOFFILE
{
  "DigitalOcean": {
    "ServiceURL": "https://***.digitaloceanspaces.com",
    "BucketName": "***",
    "AccessKey": "***",
    "SecretAccessKey": "***"
  }
}
ENDOFFILE

printf "Message: File created \n"