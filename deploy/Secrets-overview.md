# Secrets Overview

* File that contains information about the secrets that need to be created and accessed, per deployment.

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
    - password

# LinkMicroservice - PostgreSQL
* Name: postgresql-linkmicroservice-credentials
* Value:
    - linkcontext

# Seq
* Name: seq-credentials
* Value:
    - adminpasswordhash