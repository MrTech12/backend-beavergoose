# Secrets Overview

* File that contains information about the secrets that need to be created and accessed, per deployment.

# APIGateway
* Name: jwt-config
  - Values:
    - issuer
    - secret

# AccountMicroservice
* Name: accountmicroservice-dbconnectionstring
  - Value:
    - accountcontext
* Name: jwt-config
  - Values:
    - issuer
    - secret
* Name: seq-apikeys
  - Values:
    - accountmicroservice

# AccountMicroservice - dbmigration-job
* Name: accountmicroservice-dbconnectionstring
  - Value:
    - accountcontext

# AccountMicroservice - PostgreSQL
* Name: postgresql-accountmicroservice-credentials
  - Value:
    - password

## FileMicroservice
* Name: jwt-config
  - Values:
    - issuer
    - secret
* Name: seq-apikeys
  - Values:
    - filemicroservice
* Name: deletefileapp
  - Value:
    - endpoint
* Name: filedatastore
  - Value:
    - serviceurl
    - bucketname
    - accesskey
    - secretaccesskey

# LinkMicroservice
* Name: linkmicroservice-dbconnectionstring
  - Value:
    - linkcontext
* Name: jwt
  - Values:
    - issuer
    - secret
* Name: seq-apikeys
  - Values:
    - linkmicroservice

# LinkMicroservice - dbmigration-job
* Name: linkmicroservice-dbconnectionstring
  - Value:
    - linkcontext

# LinkMicroservice - PostgreSQL
* Name: postgresql-linkmicroservice-credentials
  - Value:
    - password

# DeleteFileApp
* Name: digitalocean-spaces
  - Values:
    - serviceurl
    - bucketname
    - accesskey
    - secretaccesskey
* Name: jwt
  - Values:
    - issuer
    - secret
* Name: seq-logging
  - Values:
    - serverurl
    - apikey