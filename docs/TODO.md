# TODO items for projects

~~1.~~
## Request body
* Make sure that the API can retrieve elements from the request body.
* Currently it retrieved elements from the url, which isn't convenient.
* Project: FileMicroservice
* Subject: maintainability
<br><br>

~~2.~~
## File size
* Create logic to allows files smaller than or equal to 500MB.
* If a file comes in that is bigger than 500MB, return a 400 error.
* Project: FileMicroservice
* Subject: storage
<br><br>

3.
## Integration tests database
* Create integration tests for the project that makes use of a database.
* A Docker container needs to be specified for running the test database.
* There are needs to be a `run-integration-tests.sh` bash script for starting the test database & running the integration tests for the CI workflow.
* Project: LinkMicroservice
* Subject: testing
<br><br>

4.
## Create frontend
* There needs to be an Angular frontend, that makes use of the API endpoints.
* For the frontend, a new endpoint needs to be created for the **LinkMicroservice** that retrieves links based on `receiverID`.
* Project: LinkMicroservice
* Subject: usability
<br><br>

5.
## File extensions
* Create an allow list of file extensions.
* If a file comes in that is not on the list, return a 400 error.
* Project: FileMicroservice
* Subject: security
<br><br>

6.

## Encrypt files
* Encrypt files that come into the service.
* Make use of an hash, that is also stored into the File storage location as a **metadata** field.
* Project: FileMicroservice
* Subject: security
