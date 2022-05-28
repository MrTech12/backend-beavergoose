# Secrets Overview

* File that contains information about the secrets that need to be created and accessed, per deployment.

# AccountMicroservice
* Name: accountmicroservice-dbconnectionstring
  - Value:
    - accountcontext
* Name: jwt
  - Values:
    - issuer
    - secret
    - expirationindays
* Name: seq-logging
  - Values:
    - serverurl
    - apikey

# AccountMicroservice - dbmigration-job
* Name: accountmicroservice-dbconnectionstring
  - Value:
    - accountcontext

# AccountMicroservice - PostgreSQL
* Name: postgresql-accountmicroservice-credentials
  - Value:
    - password

## FileMicroservice
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
    - expirationindays
* Name: seq-logging
  - Values:
    - serverurl
    - apikey

# LinkMicroservice
* Name: linkmicroservice-dbconnectionstring
  - Value:
    - linkcontext
* Name: jwt
  - Values:
    - issuer
    - secret
    - expirationindays
* Name: seq-logging
  - Values:
    - serverurl
    - apikey

# LinkMicroservice - dbmigration-job
* Name: linkmicroservice-dbconnectionstring
  - Value:
    - linkcontext

# LinkMicroservice - PostgreSQL
* Name: postgresql-linkmicroservice-credentials
  - Value:
    - password

# Seq
* Name: seq-credentials
  - Value:
    - adminpasswordhash