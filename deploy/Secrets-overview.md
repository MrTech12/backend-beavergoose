# Secrets Overview

* File that contains information about the secrets that need to be created and accessed, per deployment.

# AccountMicroservice
* Name: accountmicroservice-dbconnectionstring
* Value:
    - accountcontext
* Name: seq-logging
* Value: 
    - apikey

# AccountMicroservice - dbmigration-job
* Name: accountmicroservice-dbconnectionstring
* Value:
    - accountcontext

# AccountMicroservice - PostgreSQL
* Name: accountmicroservice-postgresql-credentials
* Value:
    - password

## FileMicroservice
* Name: digitalocean-spaces
* Values:
    - serviceurl
    - bucketname
    - accesskey
    - secretaccesskey
* Name: seq-logging
* Value: 
    - apikey

# LinkMicroservice
* Name: linkmicroservice-dbconnectionstring
* Value:
    - linkcontext
* Name: seq-logging
* Value: 
    - apikey

# LinkMicroservice - dbmigration-job
* Name: linkmicroservice-dbconnectionstring
* Value:
    - linkcontext

# LinkMicroservice - PostgreSQL
* Name: linkmicroservice-postgresql-credentials
* Value:
    - password

# Seq
* Name: seq-credentials
* Value:
    - adminpasswordhash