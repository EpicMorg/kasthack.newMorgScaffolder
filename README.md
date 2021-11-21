# new scaffolder for EpicMorg atlassian containers

less bullshit & more features

## Usage


````bash
cd src
./build.sh
````

## Writing templates

* Create src/atlassian/templates/\<product\>/directory
* Create \<major_version\> subdirectory
* Write a dockerfile, entrypoint, etc. Following variables will be passed via .env:
    * `RELEASE` -- application version
    * `DOWNLOAD_URL` -- installer URL
    * `JDK_VERSION` -- JDK version for custom JDK packages, "11" currenlty
    * `PRODUCT` -- template name
* [Preferably] use a shared docker-compose from /src/atlassian/templates/shared directory by symlinking it
* [Preferably] use a shared Makefile from /src/atlassian/templates/shared directory by symlinking it
* Do __not__ commit symlinks to shared files. Use a scaffolder script instead / invoke it from `src/20_PREPARE_TEMPLATES.sh`.
* Update `src/30_SCAFFOLD_DIRECTORIES.cs` if 
* Stage your changes to git
* Run `build.sh` to test your changes.