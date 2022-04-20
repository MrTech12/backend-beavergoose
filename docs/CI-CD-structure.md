# CI/CD Structure

## Provider
GitHub Actions is being used as the CI/CD pipeline provider, because the code is already hosted on GitHub.
I already made use of GitLab's CI/CD solution in semester 3 and I wanted to try the offering of GitHub.

## Structure

* GitHub Actions make use of workflows, YAML files that are in a specific folder structure (.github/workflows folders). <br> 
In these files, triggers are defined for running a workflow, like a push to a branch, the creation of a pull request or issue. <br>
These files also contain jobs which houses steps. These steps can be the execution of a command or the execution an action, that is created by another person/team. <br><br>

* This repository makes use of the microservices architecture, which means that there are different projects for specific use cases. These project are all created with .Net Core, which makes it so each project requires the same steps for building, testing and packaging. This result in a lot of duplicate code across YAML files.
* In order to reduce duplication, the repository uses template workflows. [Docs](https://docs.github.com/en/actions/using-workflows/reusing-workflows) <br> These are ordinary YAML files that house common steps which every .Net Core project follows. The templates can be called by the workflows of a specific project. The template workflows can accept input values, to specify which project will be used with the steps.
* There are three template workflows, each with their own purpose.

### analyse
* Purpose: running code analysis tools, like Sonarcloud.
* Uses: project and test files of the microservice.

### build-test
* Purpose: building and runing tests of a project, which includes displaying the test results and code coverage data.
* Uses: project and test files of the microservice.
* Special information: 
    - The workflow creates a Test Report of the passed and failed tests, which is available in the GitHub Actions interface.
    - The code coverage metric is retrieved from the generated XML file and displayed in the console. The data is also displayed as a comment of a pull request, if the workflow runs in the context of one.

### deploy
* Purpose: creating Docker images, pushing them onto DockerHub and deploying a new release onto a Kubernetes cluster.
* Uses: project files of the microservice.
* Special information
    - the doctl (DigitalOcean CLI) is used to retrieve a kubeconfig file, for interacting with the Kubernetes cluster.